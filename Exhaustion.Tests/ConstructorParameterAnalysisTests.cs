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
    }
}
