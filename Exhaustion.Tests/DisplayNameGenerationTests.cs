using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Exhaustion.Tests;

#pragma warning disable CA1515
#pragma warning disable SA1600
#pragma warning disable CA1506



/// <summary>
/// Tests for the DisplayNameGeneration module.
/// </summary>
[TestClass]
public sealed class DisplayNameGenerationTests
{
    private static INamedTypeSymbol GetTypeSymbol(string code, string typeName)
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

        // Find the type declaration
        var typeDecl = root.DescendantNodes()
            .OfType<Microsoft.CodeAnalysis.CSharp.Syntax.TypeDeclarationSyntax>()
            .First(t => t.Identifier.Text == typeName);

        var symbol = semanticModel.GetDeclaredSymbol(typeDecl);
        return symbol ?? throw new InvalidOperationException($"Could not find type {typeName}");
    }

    private static void AssertDisplayNameInteractions(
        ITypeSymbol type,
        string expectedDisplay,
        string expectedUnbound,
        Dictionary<int, INamedTypeSymbol>? variants = null
    )
    {
        var namedType = type as INamedTypeSymbol;
        Assert.IsNotNull(namedType);
        variants ??= [];
        var variantSnapshot = variants.ToArray();
        var typeArguments = namedType.TypeArguments.ToArray();
        var budget = new AnalysisBudget();

        Assert.AreEqual(
            expectedDisplay,
            DisplayNameGeneration.GetDisplayName(namedType, budget),
            "Repeated formatting must preserve the complete display name."
        );
        Assert.AreEqual(
            expectedDisplay,
            DisplayNameGeneration.GetDisplayNameWithParameters(namedType, [], budget),
            "An empty selection must preserve the base display name."
        );
        Assert.AreEqual(
            expectedUnbound,
            DisplayNameGeneration.GetUnboundTypeName(namedType),
            "The unbound name must identify the same type without arguments."
        );
        Assert.AreEqual(
            expectedUnbound,
            DisplayNameGeneration.GetUnboundTypeName(namedType.OriginalDefinition),
            "Construction must not change the unbound type name."
        );

        var withVariants = DisplayNameGeneration.GetDisplayNameWithParameters(
            namedType,
            variants,
            budget
        );
        Assert.AreEqual(
            withVariants,
            DisplayNameGeneration.GetDisplayNameWithParameters(namedType, variants),
            "Fresh and shared budgets must produce the same selected-variant name."
        );
        CollectionAssert.AreEqual(
            variantSnapshot,
            variants.ToArray(),
            "Formatting must not reorder or replace selected parameter variants."
        );
        CollectionAssert.AreEqual(
            typeArguments,
            namedType.TypeArguments.ToArray(),
            "Formatting must preserve the constructed type arguments."
        );
        Assert.IsFalse(
            expectedUnbound.Contains('<', StringComparison.Ordinal),
            "Unbound names must omit generic arguments."
        );
        Assert.IsFalse(
            expectedUnbound.Contains('`', StringComparison.Ordinal),
            "Unbound names must omit generic arity suffixes."
        );
    }

    [TestMethod]
    public void GetDisplayName_SimpleNonGenericType_ReturnsTypeName()
    {
        // Arrange
        var code =
            @"
namespace Test
{
    public class SimpleClass { }
}";
        var typeSymbol = GetTypeSymbol(code, "SimpleClass");

        // Act
        var result = DisplayNameGeneration.GetDisplayName(typeSymbol);

        // Assert
        Assert.AreEqual(
            "SimpleClass",
            result,
            "Simple non-generic type should return just the type name"
        );

        AssertDisplayNameInteractions(typeSymbol, "SimpleClass", "SimpleClass");
    }

    [TestMethod]
    public void GetDisplayName_GenericType_ReturnsNameWithTypeArguments()
    {
        // Arrange
        var code =
            @"
namespace Test
{
    public class GenericClass<T, U> { }

    public class Consumer
    {
        public GenericClass<int, string> Field;
    }
}";
        var syntaxTree = CSharpSyntaxTree.ParseText(code);
        var compilation = CSharpCompilation.Create(
            "TestAssembly",
            [syntaxTree],
            [MetadataReference.CreateFromFile(typeof(object).Assembly.Location)],
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary)
        );

        var semanticModel = compilation.GetSemanticModel(syntaxTree);
        var root = syntaxTree.GetRoot();
        var fieldDecl = root.DescendantNodes()
            .OfType<Microsoft.CodeAnalysis.CSharp.Syntax.FieldDeclarationSyntax>()
            .First();
        var typeSymbol =
            semanticModel.GetTypeInfo(fieldDecl.Declaration.Type).Type as INamedTypeSymbol;

        Assert.IsNotNull(typeSymbol, "Should have type symbol");

        // Act
        var result = DisplayNameGeneration.GetDisplayName(typeSymbol);

        // Assert
        Assert.AreEqual(
            "GenericClass<Int32, String>",
            result,
            "Generic type should return name with type arguments"
        );

        AssertDisplayNameInteractions(typeSymbol, "GenericClass<Int32, String>", "GenericClass");
    }

    [TestMethod]
    public void GetDisplayName_NestedGenericType_ReturnsNameWithTypeArguments()
    {
        // Arrange
        var code =
            @"
namespace Test
{
    public abstract class Outer<T>
    {
        private Outer() { }

        public sealed class Inner : Outer<T> { }
    }

    public class Consumer
    {
        public Outer<string>.Inner Field;
    }
}";
        var syntaxTree = CSharpSyntaxTree.ParseText(code);
        var compilation = CSharpCompilation.Create(
            "TestAssembly",
            [syntaxTree],
            [MetadataReference.CreateFromFile(typeof(object).Assembly.Location)],
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary)
        );

        var semanticModel = compilation.GetSemanticModel(syntaxTree);
        var root = syntaxTree.GetRoot();
        var fieldDecl = root.DescendantNodes()
            .OfType<Microsoft.CodeAnalysis.CSharp.Syntax.FieldDeclarationSyntax>()
            .First();
        var typeSymbol =
            semanticModel.GetTypeInfo(fieldDecl.Declaration.Type).Type as INamedTypeSymbol;

        Assert.IsNotNull(typeSymbol, "Should have type symbol");
        Assert.IsNotNull(typeSymbol.ContainingType, "Should have containing type");
        Assert.IsTrue(typeSymbol.ContainingType.IsGenericType, "Containing type should be generic");

        // Act
        var result = DisplayNameGeneration.GetDisplayName(typeSymbol);

        // Assert
        Assert.AreEqual(
            "Inner<String>",
            result,
            "Nested type in generic parent should return name with parent's type arguments"
        );

        AssertDisplayNameInteractions(typeSymbol, "Inner<String>", "Inner");
    }

    [TestMethod]
    public void GetDisplayName_GenericTypeWithNestedGenericArgument_ReturnsFullyQualifiedName()
    {
        // Arrange
        var code =
            @"
namespace Test
{
    public class Outer<T> { }
    public class Inner<U> { }

    public class Consumer
    {
        public Outer<Inner<int>> Field;
    }
}";
        var syntaxTree = CSharpSyntaxTree.ParseText(code);
        var compilation = CSharpCompilation.Create(
            "TestAssembly",
            [syntaxTree],
            [MetadataReference.CreateFromFile(typeof(object).Assembly.Location)],
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary)
        );

        var semanticModel = compilation.GetSemanticModel(syntaxTree);
        var root = syntaxTree.GetRoot();
        var fieldDecl = root.DescendantNodes()
            .OfType<Microsoft.CodeAnalysis.CSharp.Syntax.FieldDeclarationSyntax>()
            .First();
        var typeSymbol =
            semanticModel.GetTypeInfo(fieldDecl.Declaration.Type).Type as INamedTypeSymbol;

        Assert.IsNotNull(typeSymbol, "Should have type symbol");
        Assert.IsTrue(typeSymbol.IsGenericType, "Outer type should be generic");
        Assert.AreEqual(1, typeSymbol.TypeArguments.Length, "Outer should have one type argument");

        var innerTypeArg = typeSymbol.TypeArguments[0] as INamedTypeSymbol;
        Assert.IsNotNull(innerTypeArg, "Type argument should be INamedTypeSymbol");
        Assert.IsTrue(innerTypeArg.IsGenericType, "Inner type should be generic");
        Assert.AreEqual(
            1,
            innerTypeArg.TypeArguments.Length,
            "Inner should have one type argument"
        );

        var intTypeArg = innerTypeArg.TypeArguments[0];
        Assert.IsFalse(
            intTypeArg is INamedTypeSymbol namedInt && namedInt.IsGenericType,
            "Int32 should not be a generic type"
        );

        // Act
        var result = DisplayNameGeneration.GetDisplayName(typeSymbol);

        // Assert
        Assert.AreEqual(
            "Outer<Inner<Int32>>",
            result,
            "Generic type with nested generic argument should return fully qualified name"
        );
        Assert.IsTrue(
            result.Contains("Inner<Int32>", StringComparison.Ordinal),
            "Result should contain Inner with Int32 type argument"
        );

        AssertDisplayNameInteractions(typeSymbol, "Outer<Inner<Int32>>", "Outer");
    }

    [TestMethod]
    public void GetDisplayNameWithParameters_NoVariants_ReturnsBaseName()
    {
        // Arrange
        var code =
            @"
namespace Test
{
    public class SimpleClass { }
}";
        var typeSymbol = GetTypeSymbol(code, "SimpleClass");
        var emptyVariants = new Dictionary<int, INamedTypeSymbol>();

        // Act
        var result = DisplayNameGeneration.GetDisplayNameWithParameters(typeSymbol, emptyVariants);

        // Assert
        Assert.AreEqual(
            "SimpleClass",
            result,
            "Type with no parameter variants should return just the base name"
        );

        AssertDisplayNameInteractions(typeSymbol, "SimpleClass", "SimpleClass");
    }

    [TestMethod]
    public void GetDisplayNameWithParameters_WithVariants_ReturnsNameWithVariants()
    {
        // Arrange
        var code =
            @"
namespace Test
{
    public class Container { }
    public class VariantA { }
    public class VariantB { }
}";
        var containerSymbol = GetTypeSymbol(code, "Container");
        var variantA = GetTypeSymbol(code, "VariantA");
        var variantB = GetTypeSymbol(code, "VariantB");

        var variants = new Dictionary<int, INamedTypeSymbol> { [0] = variantA, [1] = variantB };

        // Act
        var result = DisplayNameGeneration.GetDisplayNameWithParameters(containerSymbol, variants);

        // Assert
        Assert.AreEqual(
            "Container with VariantA, VariantB",
            result,
            "Type with parameter variants should include 'with' clause"
        );
        Assert.AreSame(
            variantA,
            variants[0],
            "Formatting must preserve the first selected symbol."
        );
        Assert.AreSame(
            variantB,
            variants[1],
            "Formatting must preserve the second selected symbol."
        );
        _ = variants.Remove(1);
        Assert.AreEqual(
            "Container with VariantA",
            DisplayNameGeneration.GetDisplayNameWithParameters(containerSymbol, variants),
            "Changing selected coverage must update the formatted variants."
        );
        Assert.AreEqual(
            "Container with VariantA, VariantB",
            result,
            "A later formatting call must not change an earlier result."
        );
        variants.Clear();
        Assert.AreEqual(
            "Container",
            DisplayNameGeneration.GetDisplayNameWithParameters(containerSymbol, variants),
            "Removing all selections must restore the base display name."
        );
        variants.Add(0, variantA);
        variants.Add(1, variantB);

        AssertDisplayNameInteractions(containerSymbol, "Container", "Container", variants);
    }

    [TestMethod]
    public void GetUnboundTypeName_GenericType_ReturnsNameWithoutArity()
    {
        // Arrange
        var code =
            @"
namespace Test
{
    public class GenericClass<T, U> { }

    public class Consumer
    {
        public GenericClass<int, string> Field;
    }
}";
        var syntaxTree = CSharpSyntaxTree.ParseText(code);
        var compilation = CSharpCompilation.Create(
            "TestAssembly",
            [syntaxTree],
            [MetadataReference.CreateFromFile(typeof(object).Assembly.Location)],
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary)
        );

        var semanticModel = compilation.GetSemanticModel(syntaxTree);
        var root = syntaxTree.GetRoot();
        var fieldDecl = root.DescendantNodes()
            .OfType<Microsoft.CodeAnalysis.CSharp.Syntax.FieldDeclarationSyntax>()
            .First();
        var typeSymbol = semanticModel.GetTypeInfo(fieldDecl.Declaration.Type).Type;

        Assert.IsNotNull(typeSymbol, "Should have type symbol");

        // Act
        var result = DisplayNameGeneration.GetUnboundTypeName(typeSymbol);

        // Assert
        Assert.AreEqual(
            "GenericClass",
            result,
            "Unbound generic type name should not include arity suffix"
        );
        Assert.IsFalse(
            result.Contains('`', StringComparison.Ordinal),
            "Result should not contain backtick"
        );
        Assert.IsFalse(
            result.Contains('<', StringComparison.Ordinal),
            "Result should not contain type arguments"
        );

        AssertDisplayNameInteractions(typeSymbol, "GenericClass<Int32, String>", "GenericClass");
    }

    [TestMethod]
    public void GetUnboundTypeName_NonGenericType_ReturnsTypeName()
    {
        // Arrange
        var code =
            @"
namespace Test
{
    public class SimpleClass { }
}";
        var typeSymbol = GetTypeSymbol(code, "SimpleClass");

        // Act
        var result = DisplayNameGeneration.GetUnboundTypeName(typeSymbol);

        // Assert
        Assert.AreEqual("SimpleClass", result, "Non-generic type should return just the type name");
        Assert.AreEqual(
            typeSymbol.Name,
            result,
            "Result should equal type.Name for non-generic types"
        );

        AssertDisplayNameInteractions(typeSymbol, "SimpleClass", "SimpleClass");
    }

    [TestMethod]
    public void GetDisplayName_GenericTypeWithPrimitiveTypeArgument_DoesNotRecurse()
    {
        // Arrange - Test type argument that is NOT INamedTypeSymbol or NOT IsGenericType
        var code =
            @"
namespace Test
{
    public class Container<T> { }

    public class Consumer
    {
        public Container<int> Field;
    }
}";
        var syntaxTree = CSharpSyntaxTree.ParseText(code);
        var compilation = CSharpCompilation.Create(
            "TestAssembly",
            [syntaxTree],
            [MetadataReference.CreateFromFile(typeof(object).Assembly.Location)],
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary)
        );

        var semanticModel = compilation.GetSemanticModel(syntaxTree);
        var root = syntaxTree.GetRoot();
        var fieldDecl = root.DescendantNodes()
            .OfType<Microsoft.CodeAnalysis.CSharp.Syntax.FieldDeclarationSyntax>()
            .First();
        var typeSymbol =
            semanticModel.GetTypeInfo(fieldDecl.Declaration.Type).Type as INamedTypeSymbol;

        Assert.IsNotNull(typeSymbol, "Should have type symbol");

        // Act
        var result = DisplayNameGeneration.GetDisplayName(typeSymbol);

        // Assert
        Assert.AreEqual("Container<Int32>", result, "Should show primitive type without nesting");
        Assert.IsTrue(
            result.EndsWith("Int32>", StringComparison.Ordinal),
            "Should end with Int32>"
        );
        Assert.IsFalse(
            result.Contains("Int32<", StringComparison.Ordinal),
            "Int32 should not have generic parameters"
        );

        AssertDisplayNameInteractions(typeSymbol, "Container<Int32>", "Container");
    }
}
