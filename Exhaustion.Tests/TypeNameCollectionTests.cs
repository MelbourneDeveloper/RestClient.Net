using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Exhaustion.Tests;

#pragma warning disable CA1515
#pragma warning disable SA1600
#pragma warning disable CA1506

/// <summary>
/// Tests for the TypeNameCollection module.
/// </summary>
[TestClass]
public sealed class TypeNameCollectionTests
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
    public void GetAllTypeNames_SimpleLeafType_AddsTypeName()
    {
        // Arrange
        var code =
            IsExternalInitPolyfill
            + @"
namespace Test
{
    public sealed record SimpleType(int Value);
}";
        var typeSymbol = GetTypeSymbol(code, "SimpleType");
        var result = new HashSet<string>();

        // Act
        TypeNameCollection.GetAllTypeNames(typeSymbol, result);

        // Assert
        Assert.AreEqual(1, result.Count, "Should add one type name");
        Assert.IsTrue(result.Contains("SimpleType"), "Should contain the simple type name");
    }

    [TestMethod]
    public void GetAllTypeNames_ClosedHierarchy_AddsAllDerivedTypeNames()
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
        var stringType = compilation.GetSpecialType(SpecialType.System_String);
        var constructedType = typeSymbol.Construct(stringType, stringType);
        var result = new HashSet<string>();

        // Act
        TypeNameCollection.GetAllTypeNames(constructedType, result);

        // Assert
        Assert.AreEqual(2, result.Count, "Should add two type names from closed hierarchy");
        Assert.IsTrue(result.Contains("Ok<String, String>"), "Should contain Ok type");
        Assert.IsTrue(result.Contains("Error<String, String>"), "Should contain Error type");
    }

    [TestMethod]
    public void GetAllLeafTypes_ClosedHierarchy_ReturnsAllLeafTypes()
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
        var leafTypes = TypeNameCollection.GetAllLeafTypes(typeSymbol);

        // Assert
        Assert.AreEqual(2, leafTypes.Count, "Should return two leaf types");
        Assert.IsTrue(leafTypes.Any(t => t.Name == "Ok"), "Should include Ok type");
        Assert.IsTrue(leafTypes.Any(t => t.Name == "Error"), "Should include Error type");
    }

    [TestMethod]
    public void GetAllLeafTypes_SimpleType_ReturnsSelf()
    {
        // Arrange
        var code =
            IsExternalInitPolyfill
            + @"
namespace Test
{
    public sealed record SimpleType(int Value);
}";
        var typeSymbol = GetTypeSymbol(code, "SimpleType");

        // Act
        var leafTypes = TypeNameCollection.GetAllLeafTypes(typeSymbol);

        // Assert
        Assert.AreEqual(1, leafTypes.Count, "Should return the type itself as a leaf");
        Assert.AreEqual("SimpleType", leafTypes[0].Name, "Leaf type should be SimpleType");
    }

    [TestMethod]
    public void CollectLeafTypes_NestedClosedHierarchy_CollectsAllLeaves()
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
        var typeSymbol = GetTypeSymbol(code, "HttpError");
        var result = new List<INamedTypeSymbol>();

        // Act
        TypeNameCollection.CollectLeafTypes(typeSymbol, result);

        // Assert
        Assert.AreEqual(2, result.Count, "Should collect two leaf types");
        Assert.IsTrue(result.Any(t => t.Name == "ExceptionError"), "Should include ExceptionError");
        Assert.IsTrue(
            result.Any(t => t.Name == "ErrorResponseError"),
            "Should include ErrorResponseError"
        );
    }

    [TestMethod]
    public void CollectLeafTypes_GenericTypeWithTypeArguments_PreservesTypeArguments()
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
        var stringType = compilation.GetSpecialType(SpecialType.System_String);
        var constructedType = typeSymbol.Construct(stringType, stringType);
        var result = new List<INamedTypeSymbol>();

        // Act
        TypeNameCollection.CollectLeafTypes(constructedType, result);

        // Assert
        Assert.AreEqual(2, result.Count, "Should collect two leaf types");

        // The leaf types should maintain the generic nature from the parent
        var ok = result.First(t => t.Name == "Ok");
        var error = result.First(t => t.Name == "Error");

        Assert.IsTrue(
            ok.IsGenericType || ok.TypeArguments.Length == 2,
            "Ok should be generic or have type arguments"
        );
        Assert.IsTrue(
            error.IsGenericType || error.TypeArguments.Length == 2,
            "Error should be generic or have type arguments"
        );
    }

    [TestMethod]
    public void GetAllTypeNames_TypeWithClosedHierarchyParameter_ExpandsCombinations()
    {
        // Arrange
        var code =
            IsExternalInitPolyfill
            + @"
namespace Test
{
    public abstract record Status
    {
        private Status() { }

        public sealed record Active : Status;
        public sealed record Inactive : Status;
    }

    public record Container(Status Status);
}";
        var typeSymbol = GetTypeSymbol(code, "Container");
        var result = new HashSet<string>();

        // Act
        TypeNameCollection.GetAllTypeNames(typeSymbol, result);

        // Assert
        Assert.AreEqual(2, result.Count, "Should expand to two combinations");
        Assert.IsTrue(
            result.Contains("Container with Active"),
            "Should include Container with Active"
        );
        Assert.IsTrue(
            result.Contains("Container with Inactive"),
            "Should include Container with Inactive"
        );
    }

    [TestMethod]
    public void CollectLeafTypes_SealedLeafTypeInClosedHierarchy_AddsTypeDirectly()
    {
        // This test verifies that sealed leaf types in a closed hierarchy are added directly,
        // not recursed into (since they have no derived types).
        // Original logic: derived.Count > 0 && IsClosedHierarchy(type) - leaf types go to else branch
        // Mutant logic:   derived.Count > 0 || IsClosedHierarchy(type) - would try to recurse
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
        var stringType = compilation.GetSpecialType(SpecialType.System_String);
        var constructedType = typeSymbol.Construct(stringType, stringType);

        // Get the Ok<string, string> leaf type
        var derivedTypes = TypeHierarchyAnalysis.GetImmediateDerivedTypes(constructedType);
        var okType = derivedTypes.First(t => t.Name == "Ok");

        var result = new List<INamedTypeSymbol>();

        // Act - call CollectLeafTypes on the LEAF TYPE itself (not the parent)
        TypeNameCollection.CollectLeafTypes(okType, result);

        // Assert - the sealed leaf type should be added directly to the result
        // With the original code (&&), this works: derived.Count==0 && isClosedHierarchy==true → false → else → add
        // With the mutant (||), this fails: derived.Count==0 || isClosedHierarchy==true → true → recurse → nothing added
        Assert.AreEqual(1, result.Count, "Sealed leaf type should be added to result");
        Assert.AreEqual("Ok", result[0].Name, "The added type should be Ok");
    }

    [TestMethod]
    public void GetAllTypeNames_SealedLeafTypeInClosedHierarchy_AddsTypeName()
    {
        // CRITICAL TEST: Kills mutant that changes && to || on line 25 of TypeNameCollection.cs
        // This test verifies that sealed leaf types in a closed hierarchy have their name added,
        // not recursed into (since they have no derived types).
        // Original logic: nestedDerived.Count > 0 && IsClosedHierarchy(type) - leaf types go to else branch
        // Mutant logic:   nestedDerived.Count > 0 || IsClosedHierarchy(type) - would try to recurse
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
        var stringType = compilation.GetSpecialType(SpecialType.System_String);
        var constructedType = typeSymbol.Construct(stringType, stringType);

        // Get the Ok<string, string> leaf type
        var derivedTypes = TypeHierarchyAnalysis.GetImmediateDerivedTypes(constructedType);
        var okType = derivedTypes.First(t => t.Name == "Ok");

        var result = new HashSet<string>();

        // Act - call GetAllTypeNames on the LEAF TYPE itself (not the parent)
        TypeNameCollection.GetAllTypeNames(okType, result);

        // Assert - the sealed leaf type's name should be added to the result
        Assert.AreEqual(1, result.Count, "Sealed leaf type name should be added to result");
        Assert.IsTrue(
            result.Contains("Ok<String, String>"),
            "Result should contain Ok<String, String>"
        );
    }

    [TestMethod]
    public void GetAllTypeNames_LeafTypeWithNoClosedHierarchyParameters_AddsBasicDisplayName()
    {
        // CRITICAL TEST: Kills mutant on line 43 (paramHierarchies.Count > 0)
        // This test verifies that when a leaf type has NO constructor parameters with closed hierarchies,
        // the basic display name is added directly (else branch on lines 58-63).
        // Original: paramHierarchies.Count > 0 → false → else → add display name
        // Mutants like >= 0, > 1, etc. would incorrectly take the expansion path
        var code =
            IsExternalInitPolyfill
            + @"
namespace Test
{
    public sealed record SimpleType(int Value, string Name);
}";
        var typeSymbol = GetTypeSymbol(code, "SimpleType");
        var result = new HashSet<string>();

        // Act
        TypeNameCollection.GetAllTypeNames(typeSymbol, result);

        // Assert - should add exactly one simple display name
        // This test MUST fail if the mutant changes > 0 to >= 0 or another condition
        // because paramHierarchies.Count == 0, so only > 0 will correctly go to else branch
        Assert.AreEqual(1, result.Count, "Should add exactly one type name");
        Assert.IsTrue(
            result.Contains("SimpleType"),
            "Should contain basic display name SimpleType"
        );
    }

    [TestMethod]
    public void GetAllTypeNames_LeafTypeWithNoClosedHierarchyParameters_DoesNotExpandCombinations()
    {
        // Additional test to ensure the else branch (lines 58-63) is executed
        // and ExpandParameterCombinations is NOT called when paramHierarchies.Count == 0
        var code =
            IsExternalInitPolyfill
            + @"
namespace Test
{
    public sealed record Person(string FirstName, string LastName, int Age);
}";
        var typeSymbol = GetTypeSymbol(code, "Person");
        var result = new HashSet<string>();

        // Act
        TypeNameCollection.GetAllTypeNames(typeSymbol, result);

        // Assert - with no closed hierarchy parameters, should get exactly one name
        // If mutant incorrectly triggers expansion path, test would fail
        Assert.AreEqual(
            1,
            result.Count,
            "Should not expand combinations when no closed hierarchies"
        );
        Assert.IsTrue(result.Contains("Person"), "Should contain simple Person type");
    }

    [TestMethod]
    public void CollectLeafTypes_NonGenericParentType_DoesNotConstructChild()
    {
        // CRITICAL: Kills mutant that changes type.IsGenericType condition
        // Tests the case where parent type is NOT generic, so no construction needed
        var code =
            IsExternalInitPolyfill
            + @"
namespace Test
{
    public abstract record Status
    {
        private Status() { }

        public sealed record Active : Status;
        public sealed record Inactive : Status;
    }
}";
        var typeSymbol = GetTypeSymbol(code, "Status");
        var result = new List<INamedTypeSymbol>();

        // Act
        TypeNameCollection.CollectLeafTypes(typeSymbol, result);

        // Assert - should get 2 leaf types without any construction
        Assert.AreEqual(2, result.Count, "Should collect two leaf types");
        Assert.IsTrue(result.Any(t => t.Name == "Active"), "Should include Active");
        Assert.IsTrue(result.Any(t => t.Name == "Inactive"), "Should include Inactive");
    }

    [TestMethod]
    public void CollectLeafTypes_UnboundGenericType_DoesNotConstructChild()
    {
        // CRITICAL: Kills mutant that changes !type.IsUnboundGenericType condition
        // Tests the case where type IS unbound generic, so condition fails and no construction
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
        var result = new List<INamedTypeSymbol>();

        // Act - use UNBOUND generic type (no type arguments)
        TypeNameCollection.CollectLeafTypes(typeSymbol, result);

        // Assert - should get 2 unbound generic leaf types
        Assert.AreEqual(2, result.Count, "Should collect two leaf types");
        var ok = result.First(t => t.Name == "Ok");
        var error = result.First(t => t.Name == "Error");

        // Both should be generic types (unbound or with type parameters from parent)
        Assert.IsTrue(ok.IsGenericType, "Ok should be generic");
        Assert.IsTrue(error.IsGenericType, "Error should be generic");
    }

    [TestMethod]
    public void CollectLeafTypes_NonGenericChild_DoesNotConstructChild()
    {
        // CRITICAL: Kills mutant that changes child.IsGenericType condition
        // Tests case where child is NOT generic even though parent is
        var code =
            IsExternalInitPolyfill
            + @"
namespace Test
{
    public abstract record Container<T>
    {
        private Container() { }

        public sealed record EmptyContainer : Container<T>;
    }
}";
        var (typeSymbol, compilation) = GetTypeSymbolWithCompilation(code, "Container");
        var stringType = compilation.GetSpecialType(SpecialType.System_String);
        var constructedType = typeSymbol.Construct(stringType);
        var result = new List<INamedTypeSymbol>();

        // Act
        TypeNameCollection.CollectLeafTypes(constructedType, result);

        // Assert - should get the non-generic child without construction
        Assert.AreEqual(1, result.Count, "Should collect one leaf type");
        Assert.AreEqual("EmptyContainer", result[0].Name, "Should be EmptyContainer");
    }

    [TestMethod]
    public void CollectLeafTypes_ChildWithZeroArity_DoesNotConstructChild()
    {
        // CRITICAL: Kills mutant that changes child.Arity > 0 condition
        // Tests case where child.Arity is NOT > 0
        var code =
            IsExternalInitPolyfill
            + @"
namespace Test
{
    public abstract record Result<TSuccess, TFailure>
    {
        private Result() { }

        public sealed record Ok(TSuccess Value) : Result<TSuccess, TFailure>;
    }
}";
        var (typeSymbol, compilation) = GetTypeSymbolWithCompilation(code, "Result");
        var stringType = compilation.GetSpecialType(SpecialType.System_String);
        var constructedType = typeSymbol.Construct(stringType, stringType);

        var derivedTypes = TypeHierarchyAnalysis.GetImmediateDerivedTypes(constructedType);
        var okType = derivedTypes.First(t => t.Name == "Ok");

        // Verify that child.Arity is 0 (already constructed)
        Assert.AreEqual(0, okType.Arity, "Ok should have Arity 0 when already constructed");

        var result = new List<INamedTypeSymbol>();

        // Act
        TypeNameCollection.CollectLeafTypes(constructedType, result);

        // Assert - should still work correctly
        Assert.AreEqual(1, result.Count, "Should collect one leaf type");
        Assert.AreEqual("Ok", result[0].Name, "Should be Ok");
    }

    [TestMethod]
    public void CollectLeafTypes_NonGenericParent_ChildMustNotHaveTypeArgs()
    {
        // ULTRA CRITICAL: Kills mutant that inverts type.IsGenericType to !type.IsGenericType
        // Parent is NOT generic, so child MUST NOT get any type arguments applied
        // If mutant inverts, code would try Construct() on non-generic parent → CRASH or wrong result
        var code =
            IsExternalInitPolyfill
            + @"
namespace Test
{
    public abstract record Status
    {
        private Status() { }

        public sealed record Active(int Id) : Status;
    }
}";
        var typeSymbol = GetTypeSymbol(code, "Status");
        var result = new List<INamedTypeSymbol>();

        // Act
        TypeNameCollection.CollectLeafTypes(typeSymbol, result);

        // Assert - Active MUST have exactly 0 type arguments
        Assert.AreEqual(1, result.Count, "Should collect one leaf type");
        var active = result[0];
        Assert.AreEqual("Active", active.Name);
        Assert.AreEqual(
            0,
            active.TypeArguments.Length,
            "Active MUST have 0 type arguments - parent not generic!"
        );
        Assert.IsFalse(active.IsGenericType, "Active must not be generic");
    }

    [TestMethod]
    public void CollectLeafTypes_ProveConstructionCodeIsDeadOrWorking()
    {
        // CRITICAL TEST: Determine if lines 86-98 in TypeNameCollection.cs are DEAD CODE
        // The condition is: type.IsGenericType && !type.IsUnboundGenericType && child.IsGenericType && child.Arity > 0
        // This would only be true if you have a CONSTRUCTED parent AND child with its OWN unbound type params
        // This is EXTREMELY RARE in closed hierarchies!
        var code =
            IsExternalInitPolyfill
            + @"
namespace Test
{
    public abstract record Result<TSuccess, TFailure>
    {
        private Result() { }

        public sealed record Ok(TSuccess Value) : Result<TSuccess, TFailure>;
    }
}";
        var (typeSymbol, compilation) = GetTypeSymbolWithCompilation(code, "Result");
        var stringType = compilation.GetSpecialType(SpecialType.System_String);
        var intType = compilation.GetSpecialType(SpecialType.System_Int32);
        var constructedType = typeSymbol.Construct(stringType, intType);

        // Get derived types from CONSTRUCTED parent
        var derived = TypeHierarchyAnalysis.GetImmediateDerivedTypes(constructedType);
        var okType = derived.First(t => t.Name == "Ok");

        // Check if the condition would be true
        var conditionResult =
            constructedType.IsGenericType
            && !constructedType.IsUnboundGenericType
            && okType.IsGenericType
            && okType.Arity > 0;

        // If condition is FALSE, the code at lines 86-98 is NEVER EXECUTED for typical closed hierarchies
        // This means it's DEAD CODE that should be REMOVED!
        if (!conditionResult)
        {
            Assert.Inconclusive(
                $"DEAD CODE DETECTED! Lines 86-98 in CollectLeafTypes never execute. "
                    + $"Parent IsGeneric={constructedType.IsGenericType}, "
                    + $"IsUnbound={constructedType.IsUnboundGenericType}, "
                    + $"Child IsGeneric={okType.IsGenericType}, "
                    + $"Child.Arity={okType.Arity}. "
                    + "The condition is ALWAYS FALSE for normal closed hierarchies!"
            );
        }

        // If we get here, the code is NOT dead - test it works
        var result = new List<INamedTypeSymbol>();
        TypeNameCollection.CollectLeafTypes(constructedType, result);

        Assert.AreEqual(1, result.Count, "Should collect one leaf type");
        Assert.AreEqual("Ok", result[0].Name);
    }
}
