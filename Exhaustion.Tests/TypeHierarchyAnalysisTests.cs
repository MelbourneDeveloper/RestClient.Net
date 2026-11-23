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

    private static void AssertHierarchyOperations(
        INamedTypeSymbol type,
        bool? expectedClosed = null,
        List<INamedTypeSymbol>? expectedDerived = null,
        HashSet<string>? expectedNames = null
    )
    {
        var budget = new AnalysisBudget();
        var closed = TypeHierarchyAnalysis.IsClosedHierarchy(type, budget);
        var derived = TypeHierarchyAnalysis.GetImmediateDerivedTypes(type, budget);
        var names = TypeHierarchyAnalysis.GetRequiredTypeNames(type, budget);
        var derivedSnapshot = derived.ToArray();
        var nameSnapshot = names.ToArray();
        if (expectedClosed.HasValue)
        {
            Assert.AreEqual(
                expectedClosed.Value,
                closed,
                "A shared budget must preserve hierarchy classification."
            );
        }

        if (expectedDerived != null)
        {
            Assert.AreNotSame(
                expectedDerived,
                derived,
                "Separate traversals must return independently mutable child lists."
            );
            Assert.IsTrue(
                new HashSet<INamedTypeSymbol>(
                    expectedDerived,
                    SymbolEqualityComparer.Default
                ).SetEquals(derived),
                "Repeated traversal must return the exact immediate child symbols."
            );
        }

        if (expectedNames != null)
        {
            Assert.AreNotSame(
                expectedNames,
                names,
                "Separate analysis calls must return independently mutable coverage sets."
            );
            Assert.IsTrue(
                names.SetEquals(expectedNames),
                "Repeated coverage analysis must return the exact required names."
            );
        }

        Assert.AreEqual(
            derived.Count,
            derived.Distinct(SymbolEqualityComparer.Default).Count(),
            "Immediate children must not contain duplicate symbols."
        );
        foreach (var child in derived)
        {
            Assert.IsNotNull(child.BaseType);
            Assert.IsTrue(
                SymbolEqualityComparer.Default.Equals(
                    type.OriginalDefinition,
                    child.BaseType.OriginalDefinition
                ),
                "Every returned child must directly inherit the requested hierarchy."
            );
            Assert.IsTrue(
                SymbolEqualityComparer.Default.Equals(
                    type.OriginalDefinition,
                    child.ContainingType?.OriginalDefinition
                ),
                "Immediate children must belong to the requested nested hierarchy."
            );
        }

        var flattened = new HashSet<string>();
        TypeNameCollection.GetAllTypeNames(type, flattened);
        if (closed)
        {
            Assert.IsTrue(
                flattened.SetEquals(names),
                "Flattening a closed hierarchy must agree with required switch coverage."
            );
            Assert.IsTrue(names.Count > 0, "A closed hierarchy must expose required coverage.");
        }
        else
        {
            Assert.AreEqual(
                0,
                names.Count,
                "Open hierarchies must not claim a finite required coverage set."
            );
            var leaves = TypeNameCollection.GetAllLeafTypes(type);
            Assert.AreEqual(
                1,
                leaves.Count,
                "An open hierarchy must be retained as a single unexpanded type."
            );
            Assert.IsTrue(
                SymbolEqualityComparer.Default.Equals(type, leaves[0]),
                "Leaf collection must preserve the open type's identity."
            );
        }

        derived.Add(type);
        _ = names.Add("Unrelated coverage");
        CollectionAssert.AreEqual(
            derivedSnapshot,
            TypeHierarchyAnalysis.GetImmediateDerivedTypes(type).ToArray(),
            "Changing a returned child list must not affect a later traversal."
        );
        Assert.IsTrue(
            TypeHierarchyAnalysis.GetRequiredTypeNames(type).SetEquals(nameSnapshot),
            "Changing a returned coverage set must not affect later analysis."
        );
    }

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

        AssertHierarchyOperations(typeSymbol, expectedClosed: result);
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

        AssertHierarchyOperations(typeSymbol, expectedClosed: result);
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

        AssertHierarchyOperations(typeSymbol, expectedClosed: result);
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

        AssertHierarchyOperations(typeSymbol, expectedDerived: derived);
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

        AssertHierarchyOperations(typeSymbol, expectedDerived: derived);
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

        AssertHierarchyOperations(constructedType, expectedNames: typeNames);
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

        AssertHierarchyOperations(typeSymbol, expectedNames: typeNames);
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

        AssertHierarchyOperations(constructedType, expectedNames: typeNames);
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

        AssertHierarchyOperations(typeSymbol, expectedClosed: result);
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

        AssertHierarchyOperations(constructedType, expectedNames: typeNames);
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

        AssertHierarchyOperations(typeSymbol, expectedDerived: derived);
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

        AssertHierarchyOperations(typeSymbol, expectedClosed: result);
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

        AssertHierarchyOperations(typeSymbol, expectedClosed: result);
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

        AssertHierarchyOperations(unboundType, expectedNames: typeNames);
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

        AssertHierarchyOperations(constructedType, expectedNames: typeNames);
    }
}
