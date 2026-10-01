namespace Exhaustion.Tests;

#pragma warning disable CA1515
#pragma warning disable SA1600

/// <summary>
/// Tests for the DiagnosticMessages module.
/// </summary>
[TestClass]
public sealed class DiagnosticMessagesTests
{
    private static void AssertMessageInteractions(
        string baseType,
        HashSet<string> matched,
        HashSet<string> missing,
        string expectedMain,
        string expectedDetail
    )
    {
        var matchedSnapshot = matched.ToArray();
        var missingSnapshot = missing.ToArray();
        var (reorderedMain, reorderedDetail) = DiagnosticMessages.BuildDetailMessage(
            baseType,
            [.. matched.Reverse()],
            [.. missing.Reverse()]
        );
        Assert.AreEqual(
            expectedMain,
            reorderedMain,
            "Changing insertion order must not change the coverage verdict."
        );
        Assert.AreEqual(
            expectedDetail,
            reorderedDetail,
            "Changing insertion order must not change the sorted detail."
        );
        CollectionAssert.AreEqual(
            matchedSnapshot,
            matched.ToArray(),
            "Formatting must preserve the matched input set."
        );
        CollectionAssert.AreEqual(
            missingSnapshot,
            missing.ToArray(),
            "Formatting must preserve the missing input set."
        );

        var (renamedMain, renamedDetail) = DiagnosticMessages.BuildDetailMessage(
            "RenamedType",
            matched,
            missing
        );
        Assert.AreEqual(
            expectedMain.Replace(
                $"Switch on {baseType}",
                "Switch on RenamedType",
                StringComparison.Ordinal
            ),
            renamedMain,
            "Changing the switched type must update the main message."
        );
        Assert.AreEqual(
            expectedDetail,
            renamedDetail,
            "Renaming the switched type must preserve its coverage detail."
        );

        HashSet<string> updatedMatched = [.. matched, "NewlyCovered"];
        HashSet<string> updatedMissing = [.. missing, "StillMissing"];
        var (incompleteMain, incompleteDetail) = DiagnosticMessages.BuildDetailMessage(
            baseType,
            updatedMatched,
            updatedMissing
        );
        Assert.AreEqual(
            $"Switch on {baseType} is not exhaustive",
            incompleteMain,
            "New missing coverage must produce the incomplete verdict."
        );
        Assert.IsTrue(
            incompleteDetail.Contains("NewlyCovered", StringComparison.Ordinal),
            "New coverage must appear in the detail."
        );
        Assert.IsTrue(
            incompleteDetail.Contains("StillMissing", StringComparison.Ordinal),
            "Uncovered variants must appear in the detail."
        );
        Assert.IsTrue(
            incompleteDetail.Contains("; Missing: ", StringComparison.Ordinal),
            "Matched and missing coverage must remain separate sections."
        );

        updatedMatched.UnionWith(updatedMissing);
        updatedMissing.Clear();
        var (completeMain, completeDetail) = DiagnosticMessages.BuildDetailMessage(
            baseType,
            updatedMatched,
            updatedMissing
        );
        Assert.AreEqual(
            $"Switch on {baseType} has redundant default arm",
            completeMain,
            "Covering every missing variant must change the verdict."
        );
        Assert.IsFalse(
            completeDetail.Contains("Missing:", StringComparison.Ordinal),
            "Completed coverage must omit the missing section."
        );
        Assert.IsTrue(
            completeDetail.Contains("StillMissing", StringComparison.Ordinal),
            "The formerly missing variant must now appear as matched coverage."
        );
        CollectionAssert.AreEqual(
            matchedSnapshot,
            matched.ToArray(),
            "Follow-up calls must not mutate the original matched set."
        );
        CollectionAssert.AreEqual(
            missingSnapshot,
            missing.ToArray(),
            "Follow-up calls must not mutate the original missing set."
        );
    }

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

        AssertMessageInteractions(
            "BaseType",
            matchedTypes,
            missingTypes,
            mainMessage,
            detailMessage
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

        AssertMessageInteractions(
            "SomeType",
            matchedTypes,
            missingTypes,
            mainMessage,
            detailMessage
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

        AssertMessageInteractions("MyType", matchedTypes, missingTypes, mainMessage, detailMessage);
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

        AssertMessageInteractions(
            "EmptyType",
            matchedTypes,
            missingTypes,
            mainMessage,
            detailMessage
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

        AssertMessageInteractions("Result", matchedTypes, missingTypes, mainMessage, detailMessage);
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

        AssertMessageInteractions("Animal", matchedTypes, missingTypes, mainMessage, detailMessage);
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

        AssertMessageInteractions("Result", matchedTypes, missingTypes, mainMessage, detailMessage);
    }
}
