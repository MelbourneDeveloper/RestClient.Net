using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Exhaustion.Tests;

#pragma warning disable CA1515
#pragma warning disable SA1600
#pragma warning disable CA1506

/// <summary>
/// Tests for the TypeHierarchyAnalysis module.
/// </summary>
[TestClass]
public sealed class TypeHierarchyAnalysisTests
{
    private const string IsExternalInitPolyfill =
        @"
namespace System.Runtime.CompilerServices
{
    internal static class IsExternalInit {}
}
";

    private static (INamedTypeSymbol, CSharpCompilation) GetTypeSymbolWithCompilation(
        string code,
        string typeName
    )
    {
        var syntaxTree = CSharpSyntaxTree.ParseText(code);
        var compilation = CSharpCompilation.Create(
            "TestAssembly",
            [syntaxTree],
            [
                MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(Console).Assembly.Location),
            ],
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary)
        );

        var semanticModel = compilation.GetSemanticModel(syntaxTree);
        var root = syntaxTree.GetRoot();

        var typeDecl = root.DescendantNodes()
            .OfType<Microsoft.CodeAnalysis.CSharp.Syntax.TypeDeclarationSyntax>()
            .First(t => t.Identifier.Text == typeName);

        var symbol = semanticModel.GetDeclaredSymbol(typeDecl);
        return (
            symbol ?? throw new InvalidOperationException($"Could not find type {typeName}"),
            compilation
        );
    }

    private static INamedTypeSymbol GetTypeSymbol(string code, string typeName) =>
        GetTypeSymbolWithCompilation(code, typeName).Item1;

    [TestMethod]
    public void IsClosedHierarchy_AbstractRecordWithPrivateConstructor_ReturnsTrue()
    {
        // Arrange
        var code =
            IsExternalInitPolyfill
            + @"
namespace Test
{
    public abstract record Result<TSuccess, TFailure>
    {
        private Result() { }

        public sealed record Ok(TSuccess Value) : Result<TSuccess, TFailure>;
        public sealed record Error(TFailure Value) : Result<TSuccess, TFailure>;
    }
}";
        var typeSymbol = GetTypeSymbol(code, "Result");

        // Act
        var result = TypeHierarchyAnalysis.IsClosedHierarchy(typeSymbol);

        // Assert
        Assert.IsTrue(
            result,
            "Abstract record with private constructor and derived types should be a closed hierarchy"
        );
    }

    [TestMethod]
    public void IsClosedHierarchy_RecordWithPublicConstructor_ReturnsFalse()
    {
        // Arrange
        var code =
            IsExternalInitPolyfill
            + @"
namespace Test
{
    public record SimpleRecord(int Value);
}";
        var typeSymbol = GetTypeSymbol(code, "SimpleRecord");

        // Act
        var result = TypeHierarchyAnalysis.IsClosedHierarchy(typeSymbol);

        // Assert
        Assert.IsFalse(result, "Record with public constructor should not be a closed hierarchy");
    }

    [TestMethod]
    public void IsClosedHierarchy_AbstractClassWithNoDerivedTypes_ReturnsFalse()
    {
        // Arrange
        var code =
            @"
namespace Test
{
    public abstract class BaseClass
    {
        private BaseClass() { }
    }
}";
        var typeSymbol = GetTypeSymbol(code, "BaseClass");

        // Act
        var result = TypeHierarchyAnalysis.IsClosedHierarchy(typeSymbol);

        // Assert
        Assert.IsFalse(
            result,
            "Abstract class with no derived types should not be a closed hierarchy"
        );
    }

    [TestMethod]
    public void GetImmediateDerivedTypes_WithNestedDerivedTypes_ReturnsOnlyImmediateChildren()
    {
        // Arrange
        var code =
            IsExternalInitPolyfill
            + @"
namespace Test
{
    public abstract record Result<TSuccess, TFailure>
    {
        private Result() { }

        public sealed record Ok(TSuccess Value) : Result<TSuccess, TFailure>;
        public sealed record Error(TFailure Value) : Result<TSuccess, TFailure>;
    }
}";
        var typeSymbol = GetTypeSymbol(code, "Result");

        // Act
        var derived = TypeHierarchyAnalysis.GetImmediateDerivedTypes(typeSymbol);

        // Assert
        Assert.AreEqual(2, derived.Count, "Should find two immediate derived types");
        Assert.IsTrue(derived.Any(t => t.Name == "Ok"), "Should include Ok type");
        Assert.IsTrue(derived.Any(t => t.Name == "Error"), "Should include Error type");
    }

    [TestMethod]
    public void GetImmediateDerivedTypes_WithNoDerivedTypes_ReturnsEmptyList()
    {
        // Arrange
        var code =
            IsExternalInitPolyfill
            + @"
namespace Test
{
    public record SimpleRecord(int Value);
}";
        var typeSymbol = GetTypeSymbol(code, "SimpleRecord");

        // Act
        var derived = TypeHierarchyAnalysis.GetImmediateDerivedTypes(typeSymbol);

        // Assert
        Assert.AreEqual(0, derived.Count, "Should return empty list when no derived types exist");
    }

    [TestMethod]
    public void GetRequiredTypeNames_ClosedHierarchy_ReturnsAllDerivedTypeNames()
    {
        // Arrange
        var code =
            IsExternalInitPolyfill
            + @"
namespace Test
{
    public abstract record Result<TSuccess, TFailure>
    {
        private Result() { }

        public sealed record Ok(TSuccess Value) : Result<TSuccess, TFailure>;
        public sealed record Error(TFailure Value) : Result<TSuccess, TFailure>;
    }
}";
        var (typeSymbol, compilation) = GetTypeSymbolWithCompilation(code, "Result");

        // Construct the generic type with string parameters
        var stringType = compilation.GetSpecialType(SpecialType.System_String);
        var constructedType = typeSymbol.Construct(stringType, stringType);

        // Act
        var typeNames = TypeHierarchyAnalysis.GetRequiredTypeNames(constructedType);

        // Assert
        Assert.AreEqual(
            2,
            typeNames.Count,
            "Should return two type names for the closed hierarchy"
        );
        Assert.IsTrue(
            typeNames.Contains("Ok<String, String>"),
            "Should include Ok with type parameters"
        );
        Assert.IsTrue(
            typeNames.Contains("Error<String, String>"),
            "Should include Error with type parameters"
        );
    }

    [TestMethod]
    public void GetRequiredTypeNames_NonClosedHierarchy_ReturnsEmptySet()
    {
        // Arrange
        var code =
            IsExternalInitPolyfill
            + @"
namespace Test
{
    public record SimpleRecord(int Value);
}";
        var typeSymbol = GetTypeSymbol(code, "SimpleRecord");

        // Act
        var typeNames = TypeHierarchyAnalysis.GetRequiredTypeNames(typeSymbol);

        // Assert
        Assert.AreEqual(0, typeNames.Count, "Should return empty set for non-closed hierarchy");
    }

    [TestMethod]
    public void GetRequiredTypeNames_NestedClosedHierarchy_ReturnsAllLeafTypes()
    {
        // Arrange
        var code =
            IsExternalInitPolyfill
            + @"
namespace Test
{
    public abstract record HttpError<TError>
    {
        private HttpError() { }

        public sealed record ExceptionError(System.Exception Exception) : HttpError<TError>;
        public sealed record ErrorResponseError(TError Body, int StatusCode) : HttpError<TError>;
    }
}";
        var (typeSymbol, compilation) = GetTypeSymbolWithCompilation(code, "HttpError");

        // Construct with string
        var stringType = compilation.GetSpecialType(SpecialType.System_String);
        var constructedType = typeSymbol.Construct(stringType);

        // Act
        var typeNames = TypeHierarchyAnalysis.GetRequiredTypeNames(constructedType);

        // Assert
        Assert.AreEqual(2, typeNames.Count, "Should return two type names for nested hierarchy");
        Assert.IsTrue(
            typeNames.Contains("ExceptionError<String>"),
            "Should include ExceptionError with type parameter"
        );
        Assert.IsTrue(
            typeNames.Contains("ErrorResponseError<String>"),
            "Should include ErrorResponseError with type parameter"
        );
    }

    [TestMethod]
    public void IsClosedHierarchy_NonRecordAbstractClass_ReturnsFalse()
    {
        // Arrange
        var code =
            @"
namespace Test
{
    public abstract class NonRecord
    {
        protected NonRecord() { }
    }
}";
        var typeSymbol = GetTypeSymbol(code, "NonRecord");

        // Act
        var result = TypeHierarchyAnalysis.IsClosedHierarchy(typeSymbol);

        // Assert
        Assert.IsFalse(result, "Non-record abstract class should not be a closed hierarchy");
    }

    [TestMethod]
    public void GetRequiredTypeNames_GenericParentWithNonGenericDerivedType_InheritsParentTypeArgs()
    {
        // Arrange - Test the else branch where derived type is not generic
        var code =
            IsExternalInitPolyfill
            + @"
namespace Test
{
    public abstract record Container<T>
    {
        private Container() { }

        public sealed record SpecialCase : Container<T>;
        public sealed record GenericCase<T> : Container<T>;
    }
}";
        var (typeSymbol, compilation) = GetTypeSymbolWithCompilation(code, "Container");

        // Construct with int
        var intType = compilation.GetSpecialType(SpecialType.System_Int32);
        var constructedType = typeSymbol.Construct(intType);

        // Act
        var typeNames = TypeHierarchyAnalysis.GetRequiredTypeNames(constructedType);

        // Assert
        Assert.AreEqual(
            2,
            typeNames.Count,
            "Should return two type names with inherited type args"
        );
        Assert.IsTrue(
            typeNames.Contains("SpecialCase<Int32>"),
            "Should include SpecialCase with inherited type parameter from parent"
        );
        Assert.IsTrue(
            typeNames.Contains("GenericCase<Int32>"),
            "Should include GenericCase with type parameter"
        );
    }

    [TestMethod]
    public void GetImmediateDerivedTypes_WithNoBaseType_ReturnsEmptyList()
    {
        // Arrange - Test when member has no BaseType
        var code =
            IsExternalInitPolyfill
            + @"
namespace Test
{
    public abstract record Parent
    {
        private Parent() { }

        public interface INested { }
    }
}";
        var typeSymbol = GetTypeSymbol(code, "Parent");

        // Act
        var derived = TypeHierarchyAnalysis.GetImmediateDerivedTypes(typeSymbol);

        // Assert
        Assert.AreEqual(
            0,
            derived.Count,
            "Should return empty list when nested types are not derived types"
        );
    }

    [TestMethod]
    public void IsClosedHierarchy_SealedRecordWithPrivateConstructorAndDerivedTypes_ReturnsTrue()
    {
        // Arrange - Test when IsAbstract=false but IsRecord=true (sealed record parent)
        var code =
            IsExternalInitPolyfill
            + @"
namespace Test
{
    public abstract record Parent
    {
        private Parent() { }

        public sealed record Child1 : Parent;
        public sealed record Child2 : Parent;
    }
}";
        var typeSymbol = GetTypeSymbol(code, "Parent");

        // Act
        var result = TypeHierarchyAnalysis.IsClosedHierarchy(typeSymbol);

        // Assert
        Assert.IsTrue(
            result,
            "Record (even if not abstract) with private constructor and derived types should be closed hierarchy"
        );
        Assert.IsTrue(typeSymbol.IsAbstract, "Parent should be abstract");
        Assert.IsTrue(typeSymbol.IsRecord, "Parent should be a record");
    }

    [TestMethod]
    public void IsClosedHierarchy_RecordWithMixedAccessibility_OnlyChecksPublicConstructors()
    {
        // Arrange - Record with both private and protected constructors (no public)
        var code =
            IsExternalInitPolyfill
            + @"
namespace Test
{
    public abstract record Parent
    {
        private Parent() { }
        protected Parent(int value) { }

        public sealed record Child : Parent
        {
            public Child() : base(0) { }
        }
    }
}";
        var typeSymbol = GetTypeSymbol(code, "Parent");

        // Act
        var result = TypeHierarchyAnalysis.IsClosedHierarchy(typeSymbol);

        // Assert
        Assert.IsTrue(
            result,
            "Record with only private/protected constructors (no public) should be closed hierarchy"
        );
    }

    [TestMethod]
    public void GetRequiredTypeNames_UnboundGenericType_ReturnsEmptySet()
    {
        // Arrange - Test with unbound generic type
        var code =
            IsExternalInitPolyfill
            + @"
namespace Test
{
    public abstract record Result<TSuccess, TFailure>
    {
        private Result() { }

        public sealed record Ok(TSuccess Value) : Result<TSuccess, TFailure>;
        public sealed record Error(TFailure Value) : Result<TSuccess, TFailure>;
    }
}";
        var typeSymbol = GetTypeSymbol(code, "Result");

        // Don't construct the type - use the unbound generic
        var unboundType = typeSymbol.ConstructUnboundGenericType();

        // Act
        var typeNames = TypeHierarchyAnalysis.GetRequiredTypeNames(unboundType);

        // Assert
        Assert.AreEqual(
            0,
            typeNames.Count,
            "Unbound generic type should return empty set (IsUnboundGenericType check)"
        );
    }

    [TestMethod]
    public void GetRequiredTypeNames_DerivedWithArityZero_HandlesCorrectly()
    {
        // Arrange - Non-generic derived type from generic parent
        var code =
            IsExternalInitPolyfill
            + @"
namespace Test
{
    public abstract record Container<T>
    {
        private Container() { }

        public sealed record NonGenericChild : Container<T>;
    }
}";
        var (typeSymbol, compilation) = GetTypeSymbolWithCompilation(code, "Container");

        var intType = compilation.GetSpecialType(SpecialType.System_Int32);
        var constructedType = typeSymbol.Construct(intType);

        // Act
        var typeNames = TypeHierarchyAnalysis.GetRequiredTypeNames(constructedType);

        // Assert
        Assert.AreEqual(1, typeNames.Count, "Should handle non-generic derived type (Arity=0)");
        Assert.IsTrue(
            typeNames.Contains("NonGenericChild<Int32>"),
            "Should include NonGenericChild with inherited type parameter"
        );
    }
}
