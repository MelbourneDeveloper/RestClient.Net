using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Exhaustion.Tests;

#pragma warning disable CA1515
#pragma warning disable SA1600
#pragma warning disable CA1506

/// <summary>
/// Tests for the ConstructorParameterAnalysis module.
/// </summary>
[TestClass]
public sealed class ConstructorParameterAnalysisTests
{
    private const string IsExternalInitPolyfill =
        @"
namespace System.Runtime.CompilerServices
{
    internal static class IsExternalInit {}
}
";

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

        var typeDecl = root.DescendantNodes()
            .OfType<Microsoft.CodeAnalysis.CSharp.Syntax.TypeDeclarationSyntax>()
            .First(t => t.Identifier.Text == typeName);

        var symbol = semanticModel.GetDeclaredSymbol(typeDecl);
        return symbol ?? throw new InvalidOperationException($"Could not find type {typeName}");
    }

    private static void AssertConstructorHierarchyOperations(
        INamedTypeSymbol type,
        List<(int Index, List<INamedTypeSymbol> Variants)> expected
    )
    {
        var repeated = ConstructorParameterAnalysis.GetConstructorParameterHierarchies(
            type,
            new AnalysisBudget()
        );
        Assert.AreNotSame(
            expected,
            repeated,
            "Separate analysis calls must return independently mutable lists."
        );
        CollectionAssert.AreEqual(
            expected.Select(item => item.Index).ToArray(),
            repeated.Select(item => item.Index).ToArray(),
            "Repeated analysis must preserve parameter positions and order."
        );
        Assert.AreEqual(
            expected.Count,
            expected.Select(item => item.Index).Distinct().Count(),
            "A parameter must appear once."
        );

        foreach (var item in expected)
        {
            var repeatedItem = repeated.Single(candidate => candidate.Index == item.Index);
            Assert.AreNotSame(
                item.Variants,
                repeatedItem.Variants,
                "Returned parameter variants must not share mutable lists between calls."
            );
            Assert.IsTrue(
                new HashSet<INamedTypeSymbol>(
                    item.Variants,
                    SymbolEqualityComparer.Default
                ).SetEquals(repeatedItem.Variants),
                "Repeated analysis must return the same variant symbols."
            );
            Assert.AreEqual(
                item.Variants.Count,
                item.Variants.Distinct(SymbolEqualityComparer.Default).Count(),
                "Variants must not contain duplicate symbols."
            );
            Assert.IsTrue(
                item.Variants.All(variant => !TypeHierarchyAnalysis.IsClosedHierarchy(variant)),
                "Constructor variants must be leaves."
            );

            var constructor = type
                .InstanceConstructors.OrderByDescending(candidate => candidate.Parameters.Length)
                .First();
            Assert.IsTrue(
                item.Index < constructor.Parameters.Length,
                "Every returned index must identify a constructor parameter."
            );
            var parameterType = constructor.Parameters[item.Index].Type as INamedTypeSymbol;
            Assert.IsNotNull(parameterType);
            var leaves = TypeNameCollection.GetAllLeafTypes(parameterType);
            Assert.IsTrue(
                new HashSet<INamedTypeSymbol>(leaves, SymbolEqualityComparer.Default).SetEquals(
                    item.Variants
                ),
                "The selected parameter's hierarchy must account for every variant and no others."
            );
        }

        repeated.Add((int.MaxValue, [type]));
        var fresh = ConstructorParameterAnalysis.GetConstructorParameterHierarchies(type);
        CollectionAssert.AreEqual(
            expected.Select(item => item.Index).ToArray(),
            fresh.Select(item => item.Index).ToArray(),
            "Changing a returned list must not affect subsequent analysis."
        );
    }

    private static void AssertExpansionPreservesInputs(
        INamedTypeSymbol type,
        List<(int Index, List<INamedTypeSymbol> Variants)> hierarchies,
        HashSet<string> expected
    )
    {
        var indices = hierarchies.Select(item => item.Index).ToArray();
        var variants = hierarchies.Select(item => item.Variants.ToArray()).ToArray();
        var selections = new Dictionary<int, INamedTypeSymbol>();
        var repeated = new HashSet<string> { "Existing coverage" };
        ConstructorParameterAnalysis.ExpandParameterCombinations(
            type,
            hierarchies,
            0,
            selections,
            repeated,
            new AnalysisBudget()
        );
        Assert.AreEqual(
            0,
            selections.Count,
            "Expansion must not leave assignments in the caller's dictionary."
        );
        Assert.IsTrue(
            repeated.SetEquals(expected.Append("Existing coverage")),
            "Expansion must append the exact combinations and preserve existing coverage."
        );
        CollectionAssert.AreEqual(
            indices,
            hierarchies.Select(item => item.Index).ToArray(),
            "Expansion must preserve parameter positions."
        );
        for (var index = 0; index < hierarchies.Count; index++)
        {
            CollectionAssert.AreEqual(
                variants[index],
                hierarchies[index].Variants.ToArray(),
                "Expansion must not reorder or replace input variant symbols."
            );
        }

        ConstructorParameterAnalysis.ExpandParameterCombinations(
            type,
            hierarchies,
            0,
            selections,
            repeated
        );
        Assert.IsTrue(
            repeated.SetEquals(expected.Append("Existing coverage")),
            "Repeating expansion into a set must remain idempotent."
        );
        var collected = new HashSet<string>();
        TypeNameCollection.GetAllTypeNames(type, collected);
        Assert.IsTrue(
            collected.SetEquals(expected),
            "Hierarchy name collection must agree with direct constructor expansion."
        );
    }

    [TestMethod]
    public void GetConstructorParameterHierarchies_RecordWithClosedHierarchyParameter_ReturnsParameterInfo()
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

        // Act
        var result = ConstructorParameterAnalysis.GetConstructorParameterHierarchies(typeSymbol);

        // Assert
        Assert.AreEqual(1, result.Count, "Should find one parameter with closed hierarchy");
        Assert.AreEqual(0, result[0].Index, "Parameter should be at index 0");
        Assert.AreEqual(2, result[0].Variants.Count, "Parameter should have two variants");
        Assert.IsTrue(
            result[0].Variants.Any(v => v.Name == "Active"),
            "Variants should include Active"
        );
        Assert.IsTrue(
            result[0].Variants.Any(v => v.Name == "Inactive"),
            "Variants should include Inactive"
        );

        AssertConstructorHierarchyOperations(typeSymbol, result);
    }

    [TestMethod]
    public void GetConstructorParameterHierarchies_RecordWithMultipleClosedHierarchyParameters_ReturnsAllParameters()
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

    public abstract record Priority
    {
        private Priority() { }

        public sealed record High : Priority;
        public sealed record Low : Priority;
    }

    public record Task(Status Status, Priority Priority);
}";
        var typeSymbol = GetTypeSymbol(code, "Task");

        // Act
        var result = ConstructorParameterAnalysis.GetConstructorParameterHierarchies(typeSymbol);

        // Assert
        Assert.AreEqual(2, result.Count, "Should find two parameters with closed hierarchies");
        Assert.AreEqual(0, result[0].Index, "First parameter should be at index 0");
        Assert.AreEqual(1, result[1].Index, "Second parameter should be at index 1");
        Assert.AreEqual(2, result[0].Variants.Count, "First parameter should have two variants");
        Assert.AreEqual(2, result[1].Variants.Count, "Second parameter should have two variants");
        CollectionAssert.AreEquivalent(
            (string[])["Active", "Inactive"],
            result[0].Variants.Select(variant => variant.Name).ToArray(),
            "Status variants must not be confused with Priority variants."
        );
        CollectionAssert.AreEquivalent(
            (string[])["High", "Low"],
            result[1].Variants.Select(variant => variant.Name).ToArray(),
            "Priority must retain its own exact variant set."
        );

        AssertConstructorHierarchyOperations(typeSymbol, result);
    }

    [TestMethod]
    public void GetConstructorParameterHierarchies_RecordWithNoClosedHierarchyParameters_ReturnsEmptyList()
    {
        // Arrange
        var code =
            IsExternalInitPolyfill
            + @"
namespace Test
{
    public record SimpleRecord(int Value, string Name);
}";
        var typeSymbol = GetTypeSymbol(code, "SimpleRecord");

        // Act
        var result = ConstructorParameterAnalysis.GetConstructorParameterHierarchies(typeSymbol);

        // Assert
        Assert.AreEqual(
            0,
            result.Count,
            "Should return empty list for record with no closed hierarchy parameters"
        );

        AssertConstructorHierarchyOperations(typeSymbol, result);
    }

    [TestMethod]
    public void GetConstructorParameterHierarchies_NonRecord_ReturnsEmptyList()
    {
        // Arrange
        var code =
            @"
namespace Test
{
    public class RegularClass
    {
        public RegularClass(int value) { }
    }
}";
        var typeSymbol = GetTypeSymbol(code, "RegularClass");

        // Act
        var result = ConstructorParameterAnalysis.GetConstructorParameterHierarchies(typeSymbol);

        // Assert
        Assert.AreEqual(0, result.Count, "Should return empty list for non-record types");

        AssertConstructorHierarchyOperations(typeSymbol, result);
    }

    [TestMethod]
    public void ExpandParameterCombinations_SingleParameter_GeneratesAllCombinations()
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
        var paramHierarchies = ConstructorParameterAnalysis.GetConstructorParameterHierarchies(
            typeSymbol
        );
        var result = new HashSet<string>();

        // Act
        ConstructorParameterAnalysis.ExpandParameterCombinations(
            typeSymbol,
            paramHierarchies,
            0,
            [],
            result
        );

        // Assert
        Assert.AreEqual(2, result.Count, "Should generate two combinations");
        Assert.IsTrue(
            result.Contains("Container with Active"),
            "Should include Container with Active"
        );
        Assert.IsTrue(
            result.Contains("Container with Inactive"),
            "Should include Container with Inactive"
        );

        AssertExpansionPreservesInputs(typeSymbol, paramHierarchies, result);
    }

    [TestMethod]
    public void ExpandParameterCombinations_MultipleParameters_GeneratesCartesianProduct()
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

    public abstract record Priority
    {
        private Priority() { }

        public sealed record High : Priority;
        public sealed record Low : Priority;
    }

    public record Task(Status Status, Priority Priority);
}";
        var typeSymbol = GetTypeSymbol(code, "Task");
        var paramHierarchies = ConstructorParameterAnalysis.GetConstructorParameterHierarchies(
            typeSymbol
        );
        var result = new HashSet<string>();

        // Act
        ConstructorParameterAnalysis.ExpandParameterCombinations(
            typeSymbol,
            paramHierarchies,
            0,
            [],
            result
        );

        // Assert
        Assert.AreEqual(4, result.Count, "Should generate four combinations (2×2)");
        Assert.IsTrue(
            result.Contains("Task with Active, High"),
            "Should include Task with Active, High"
        );
        Assert.IsTrue(
            result.Contains("Task with Active, Low"),
            "Should include Task with Active, Low"
        );
        Assert.IsTrue(
            result.Contains("Task with Inactive, High"),
            "Should include Task with Inactive, High"
        );
        Assert.IsTrue(
            result.Contains("Task with Inactive, Low"),
            "Should include Task with Inactive, Low"
        );

        AssertExpansionPreservesInputs(typeSymbol, paramHierarchies, result);
    }

    [TestMethod]
    public void ExpandParameterCombinations_EmptyParameterList_AddsBaseName()
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
        var emptyHierarchies = new List<(int Index, List<INamedTypeSymbol> Variants)>();
        var result = new HashSet<string>();

        // Act
        ConstructorParameterAnalysis.ExpandParameterCombinations(
            typeSymbol,
            emptyHierarchies,
            0,
            [],
            result
        );

        // Assert
        Assert.AreEqual(1, result.Count, "Should add one entry for the base name");
        Assert.IsTrue(result.Contains("SimpleRecord"), "Should include the simple type name");

        AssertExpansionPreservesInputs(typeSymbol, emptyHierarchies, result);
    }

    [TestMethod]
    public void GetConstructorParameterHierarchies_GenericRecordWithTypeParameterClosure_ResolvesTypeParameters()
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

    public record GenericContainer<T>(T Value);
}";
        var typeSymbol = GetTypeSymbol(code, "GenericContainer");
        var statusType = GetTypeSymbol(code, "Status");
        var constructedType = typeSymbol.Construct(statusType);

        // Act
        var result = ConstructorParameterAnalysis.GetConstructorParameterHierarchies(
            constructedType
        );

        // Assert
        Assert.AreEqual(
            1,
            result.Count,
            "Should find one parameter with closed hierarchy after type parameter resolution"
        );
        Assert.AreEqual(2, result[0].Variants.Count, "Should resolve to two variants");
        Assert.AreEqual(
            0,
            result[0].Index,
            "The resolved Value parameter must remain at position zero."
        );
        Assert.IsTrue(
            SymbolEqualityComparer.Default.Equals(
                statusType,
                constructedType
                    .InstanceConstructors.Single(constructor =>
                        constructor.Parameters.Length == 1
                        && constructor.Parameters[0].Name == "Value"
                    )
                    .Parameters[0]
                    .Type
            ),
            "The generic constructor parameter must resolve to the requested Status type."
        );
        CollectionAssert.AreEquivalent(
            (string[])["Active", "Inactive"],
            result[0].Variants.Select(variant => variant.Name).ToArray()
        );

        AssertConstructorHierarchyOperations(constructedType, result);
    }

    [TestMethod]
    public void GetConstructorParameterHierarchies_RecordWithParameterlessConstructor_ReturnsEmptyList()
    {
        // Arrange - Record with parameterless constructor
        var code =
            IsExternalInitPolyfill
            + @"
namespace Test
{
    public abstract record EmptyRecord
    {
        private EmptyRecord() { }
    }
}";
        var typeSymbol = GetTypeSymbol(code, "EmptyRecord");

        // Act
        var result = ConstructorParameterAnalysis.GetConstructorParameterHierarchies(typeSymbol);

        // Assert
        Assert.AreEqual(
            0,
            result.Count,
            "Should return empty list when record has only parameterless constructor"
        );

        AssertConstructorHierarchyOperations(typeSymbol, result);
    }

    [TestMethod]
    public void GetConstructorParameterHierarchies_RecordWithArrayParameter_ReturnsEmptyList()
    {
        // Arrange - Test with array parameter (not an INamedTypeSymbol)
        var code =
            IsExternalInitPolyfill
            + @"
namespace Test
{
    public record RecordWithArray(int[] Values);
}";
        var typeSymbol = GetTypeSymbol(code, "RecordWithArray");

        // Act
        var result = ConstructorParameterAnalysis.GetConstructorParameterHierarchies(typeSymbol);

        // Assert
        Assert.AreEqual(
            0,
            result.Count,
            "Should return empty list when parameter is an array (not INamedTypeSymbol)"
        );

        AssertConstructorHierarchyOperations(typeSymbol, result);
    }

    [TestMethod]
    public void GetConstructorParameterHierarchies_RecordWithSingleVariantParameter_ReturnsEmptyList()
    {
        // Arrange - Test with single variant (no closed hierarchy)
        var code =
            IsExternalInitPolyfill
            + @"
namespace Test
{
    public abstract record Status
    {
        private Status() { }

        public sealed record Active : Status;
    }

    public record Container(Status Status);
}";
        var typeSymbol = GetTypeSymbol(code, "Container");

        // Act
        var result = ConstructorParameterAnalysis.GetConstructorParameterHierarchies(typeSymbol);

        // Assert
        Assert.AreEqual(
            0,
            result.Count,
            "Should return empty list when parameter has only one variant (variants.Count <= 1)"
        );

        AssertConstructorHierarchyOperations(typeSymbol, result);
    }

    [TestMethod]
    public void GetConstructorParameterHierarchies_RecordWithMultipleConstructors_UsesLongestConstructor()
    {
        // Arrange - Record with multiple constructors
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

    public record Container
    {
        public Container(Status Status, int Value)
        {
            this.Status = Status;
            this.Value = Value;
        }

        public Container(Status Status) : this(Status, 0) { }

        public Status Status { get; init; }
        public int Value { get; init; }
    }
}";
        var typeSymbol = GetTypeSymbol(code, "Container");

        // Act
        var result = ConstructorParameterAnalysis.GetConstructorParameterHierarchies(typeSymbol);

        // Assert
        Assert.AreEqual(1, result.Count, "Should find one parameter with closed hierarchy");
        Assert.AreEqual(
            0,
            result[0].Index,
            "Status parameter should be at index 0 (first parameter in longest constructor)"
        );
        Assert.AreEqual(2, result[0].Variants.Count, "Status parameter should have two variants");

        AssertConstructorHierarchyOperations(typeSymbol, result);
    }
}
