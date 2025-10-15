namespace Exhaustion.Tests;

#pragma warning disable CA1515
#pragma warning disable SA1600

/// <summary>
/// Tests for the DiagnosticMessages module.
/// </summary>
[TestClass]
public sealed class DiagnosticMessagesTests
{
    [TestMethod]
    public void BuildDetailMessage_WithMissingTypes_ReturnsNotExhaustiveMessage()
    {
        // Arrange
        var matchedTypes = new HashSet<string> { "Type1", "Type2" };
        var missingTypes = new HashSet<string> { "Type3", "Type4" };

        // Act
        var (mainMessage, detailMessage) = DiagnosticMessages.BuildDetailMessage(
            "BaseType",
            matchedTypes,
            missingTypes
        );

        // Assert
        Assert.AreEqual(
            "Switch on BaseType is not exhaustive",
            mainMessage,
            "Main message should indicate switch is not exhaustive"
        );
        Assert.AreEqual(
            "Matched: Type1, Type2; Missing: Type3, Type4",
            detailMessage,
            "Detail should include both matched and missing types separated by semicolon"
        );
    }

    [TestMethod]
    public void BuildDetailMessage_NoMissingTypes_ReturnsRedundantDefaultMessage()
    {
        // Arrange
        var matchedTypes = new HashSet<string> { "TypeA", "TypeB", "TypeC" };
        var missingTypes = new HashSet<string>();

        // Act
        var (mainMessage, detailMessage) = DiagnosticMessages.BuildDetailMessage(
            "SomeType",
            matchedTypes,
            missingTypes
        );

        // Assert
        Assert.AreEqual(
            "Switch on SomeType has redundant default arm",
            mainMessage,
            "Main message should indicate redundant default arm when no types are missing"
        );
        Assert.AreEqual(
            "Matched: TypeA, TypeB, TypeC",
            detailMessage,
            "Detail should include only matched types when there are no missing types"
        );
    }

    [TestMethod]
    public void BuildDetailMessage_OnlyMissingTypes_ReturnsNotExhaustiveWithMissingOnly()
    {
        // Arrange
        var matchedTypes = new HashSet<string>();
        var missingTypes = new HashSet<string> { "TypeX", "TypeY" };

        // Act
        var (mainMessage, detailMessage) = DiagnosticMessages.BuildDetailMessage(
            "MyType",
            matchedTypes,
            missingTypes
        );

        // Assert
        Assert.AreEqual(
            "Switch on MyType is not exhaustive",
            mainMessage,
            "Main message should indicate not exhaustive"
        );
        Assert.AreEqual(
            "Missing: TypeX, TypeY",
            detailMessage,
            "Detail should include only missing types when there are no matched types"
        );
    }

    [TestMethod]
    public void BuildDetailMessage_BothEmpty_ReturnsRedundantDefaultWithEmptyDetail()
    {
        // Arrange
        var matchedTypes = new HashSet<string>();
        var missingTypes = new HashSet<string>();

        // Act
        var (mainMessage, detailMessage) = DiagnosticMessages.BuildDetailMessage(
            "EmptyType",
            matchedTypes,
            missingTypes
        );

        // Assert
        Assert.AreEqual(
            "Switch on EmptyType has redundant default arm",
            mainMessage,
            "Main message should indicate redundant default when no missing types"
        );
        Assert.AreEqual(
            string.Empty,
            detailMessage,
            "Detail should be empty when both sets are empty"
        );
    }

    [TestMethod]
    public void BuildDetailMessage_SingleMatchedAndMissing_FormatsCorrectly()
    {
        // Arrange
        var matchedTypes = new HashSet<string> { "SingleMatched" };
        var missingTypes = new HashSet<string> { "SingleMissing" };

        // Act
        var (mainMessage, detailMessage) = DiagnosticMessages.BuildDetailMessage(
            "Result",
            matchedTypes,
            missingTypes
        );

        // Assert
        Assert.AreEqual(
            "Switch on Result is not exhaustive",
            mainMessage,
            "Main message should indicate not exhaustive when missing types exist"
        );
        Assert.AreEqual(
            "Matched: SingleMatched; Missing: SingleMissing",
            detailMessage,
            "Detail should format correctly with single item in each set"
        );
    }

    [TestMethod]
    public void BuildDetailMessage_TypesAreSortedAlphabetically()
    {
        // Arrange
        var matchedTypes = new HashSet<string> { "Zebra", "Apple", "Mango" };
        var missingTypes = new HashSet<string> { "Dog", "Cat", "Bird" };

        // Act
        var (mainMessage, detailMessage) = DiagnosticMessages.BuildDetailMessage(
            "Animal",
            matchedTypes,
            missingTypes
        );

        // Assert
        Assert.AreEqual(
            "Switch on Animal is not exhaustive",
            mainMessage,
            "Main message should indicate not exhaustive"
        );
        Assert.AreEqual(
            "Matched: Apple, Mango, Zebra; Missing: Bird, Cat, Dog",
            detailMessage,
            "Types should be sorted alphabetically in the output"
        );
    }

    [TestMethod]
    public void BuildDetailMessage_ResultWithBothArms_ShowsRedundantDefault()
    {
        // Arrange - Simulating Result<T,E> with both Ok and Error matched
        var matchedTypes = new HashSet<string> { "Ok<Int32, String>", "Error<Int32, String>" };
        var missingTypes = new HashSet<string>();

        // Act
        var (mainMessage, detailMessage) = DiagnosticMessages.BuildDetailMessage(
            "Result",
            matchedTypes,
            missingTypes
        );

        // Assert
        Assert.AreEqual(
            "Switch on Result has redundant default arm",
            mainMessage,
            "Main message should indicate redundant default when both Result arms are matched"
        );
        Assert.AreEqual(
            "Matched: Error<Int32, String>, Ok<Int32, String>",
            detailMessage,
            "Detail should show both Result types matched alphabetically"
        );
    }
}
