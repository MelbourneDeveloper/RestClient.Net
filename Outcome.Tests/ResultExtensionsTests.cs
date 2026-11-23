namespace Outcome.Tests;

[TestClass]
public class ResultExtensionsTests
{
    [TestMethod]
    public void Sequence_WithAllOk_ReturnsOkWithList()
    {
        var results = new[]
        {
            new Result<int, string>.Ok<int, string>(1),
            new Result<int, string>.Ok<int, string>(2),
            new Result<int, string>.Ok<int, string>(3),
        };

        var sequenced = results.Sequence();

        Assert.IsTrue(sequenced.IsOk);
        var list = +sequenced;
        Assert.AreEqual(3, list.Count);
        Assert.AreEqual(1, list[0]);
        Assert.AreEqual(2, list[1]);
        Assert.AreEqual(3, list[2]);
    }

    [TestMethod]
    public void Sequence_WithOneError_ReturnsFirstError()
    {
        var results = new[]
        {
            new Result<int, string>.Ok<int, string>(1),
            Result<int, string>.Failure("first error"),
            Result<int, string>.Failure("second error"),
        };

        var sequenced = results.Sequence();

        Assert.IsTrue(sequenced.IsError);
        var error = !sequenced;
        Assert.AreEqual("first error", error);
    }

    [TestMethod]
    public void Sequence_WithEmptyEnumerable_ReturnsEmptyList()
    {
        var results = Array.Empty<Result<int, string>>();

        var sequenced = results.Sequence();

        Assert.IsTrue(sequenced.IsOk);
        var list = +sequenced;
        Assert.AreEqual(0, list.Count);
    }

    [TestMethod]
    public void Sequence_ReturnsReadOnlyList()
    {
        var results = new[]
        {
            new Result<int, string>.Ok<int, string>(1),
            new Result<int, string>.Ok<int, string>(2),
        };

        var sequenced = results.Sequence();
        var list = +sequenced;

        Assert.IsInstanceOfType<IReadOnlyList<int>>(list);
    }

    [TestMethod]
    public void Flatten_WithOkOk_ReturnsOk()
    {
        var nested = new Result<Result<int, string>, string>.Ok<Result<int, string>, string>(
            new Result<int, string>.Ok<int, string>(42)
        );

        var flattened = nested.Flatten();

        Assert.IsTrue(flattened.IsOk);
        Assert.AreEqual(42, +flattened);
    }

    [TestMethod]
    public void Flatten_WithOkError_ReturnsError()
    {
        var nested = new Result<Result<int, string>, string>.Ok<Result<int, string>, string>(
            Result<int, string>.Failure("inner error")
        );

        var flattened = nested.Flatten();

        Assert.IsTrue(flattened.IsError);
        Assert.AreEqual("inner error", !flattened);
    }

    [TestMethod]
    public void Flatten_WithError_ReturnsError()
    {
        var nested = Result<Result<int, string>, string>.Failure("outer error");

        var flattened = nested.Flatten();

        Assert.IsTrue(flattened.IsError);
        Assert.AreEqual("outer error", !flattened);
    }

    [TestMethod]
    public void Combine_WithTwoOk_CallsCombiner()
    {
        var result1 = new Result<int, string>.Ok<int, string>(5);
        var result2 = new Result<int, string>.Ok<int, string>(3);

        var combined = result1.Combine(result2, static (a, b) => a + b);

        Assert.IsTrue(combined.IsOk);
        Assert.AreEqual(8, +combined);
    }

    [TestMethod]
    public void Combine_WithFirstError_ReturnsFirstError()
    {
        var result1 = Result<int, string>.Failure("error 1");
        var result2 = new Result<int, string>.Ok<int, string>(3);

        var combined = result1.Combine(result2, static (a, b) => a + b);

        Assert.IsTrue(combined.IsError);
        Assert.AreEqual("error 1", !combined);
    }

    [TestMethod]
    public void Combine_WithSecondError_ReturnsSecondError()
    {
        var result1 = new Result<int, string>.Ok<int, string>(5);
        var result2 = Result<int, string>.Failure("error 2");

        var combined = result1.Combine(result2, static (a, b) => a + b);

        Assert.IsTrue(combined.IsError);
        Assert.AreEqual("error 2", !combined);
    }

    [TestMethod]
    public void Combine_WithBothErrors_ReturnsFirstError()
    {
        var result1 = Result<int, string>.Failure("error 1");
        var result2 = Result<int, string>.Failure("error 2");

        var combined = result1.Combine(result2, static (a, b) => a + b);

        Assert.IsTrue(combined.IsError);
        Assert.AreEqual("error 1", !combined);
    }

    [TestMethod]
    public void Combine_WithDifferentTypes_Works()
    {
        var result1 = new Result<int, string>.Ok<int, string>(5);
        var result2 = new Result<string, string>.Ok<string, string>("test");

        var combined = result1.Combine(result2, static (num, str) => $"{str}: {num}");

        Assert.IsTrue(combined.IsOk);
        Assert.AreEqual("test: 5", +combined);
    }

    [TestMethod]
    public void Filter_WithPredicateTrue_ReturnsOriginal()
    {
        var result = new Result<int, string>.Ok<int, string>(42);

        var filtered = result.Filter(static x => x > 10, "too small");

        Assert.IsTrue(filtered.IsOk);
        Assert.AreEqual(42, +filtered);
    }

    [TestMethod]
    public void Filter_WithPredicateFalse_ReturnsError()
    {
        var result = new Result<int, string>.Ok<int, string>(5);

        var filtered = result.Filter(static x => x > 10, "too small");

        Assert.IsTrue(filtered.IsError);
        Assert.AreEqual("too small", !filtered);
    }

    [TestMethod]
    public void Filter_OnError_PropagatesError()
    {
        var result = Result<int, string>.Failure("original error");

        var filtered = result.Filter(static x => x > 10, "predicate error");

        Assert.IsTrue(filtered.IsError);
        Assert.AreEqual("original error", !filtered);
    }

    [TestMethod]
    public void GetValueOrThrow_OnOk_ReturnsValue()
    {
        var result = new Result<int, string>.Ok<int, string>(42);

        var value = result.GetValueOrThrow();

        Assert.AreEqual(42, value);
    }

    [TestMethod]
    public void GetValueOrThrow_OnError_ThrowsWithDefaultMessage()
    {
        var result = Result<int, string>.Failure("error");

        var exception = Assert.ThrowsException<InvalidOperationException>(
            () => result.GetValueOrThrow()
        );
        Assert.AreEqual("Expected success result", exception.Message);
    }

    [TestMethod]
    public void GetValueOrThrow_OnError_ThrowsWithCustomMessage()
    {
        var result = Result<int, string>.Failure("error");

        var exception = Assert.ThrowsException<InvalidOperationException>(
            () => result.GetValueOrThrow("Custom error message")
        );
        Assert.AreEqual("Custom error message", exception.Message);
    }

    [TestMethod]
    public void GetErrorOrThrow_OnError_ReturnsError()
    {
        var result = Result<int, string>.Failure("test error");

        var error = result.GetErrorOrThrow();

        Assert.AreEqual("test error", error);
    }

    [TestMethod]
    public void GetErrorOrThrow_OnOk_ThrowsWithDefaultMessage()
    {
        var result = new Result<int, string>.Ok<int, string>(42);

        var exception = Assert.ThrowsException<InvalidOperationException>(
            () => result.GetErrorOrThrow()
        );
        Assert.AreEqual("Expected error result", exception.Message);
    }

    [TestMethod]
    public void GetErrorOrThrow_OnOk_ThrowsWithCustomMessage()
    {
        var result = new Result<int, string>.Ok<int, string>(42);

        var exception = Assert.ThrowsException<InvalidOperationException>(
            () => result.GetErrorOrThrow("Custom error message")
        );
        Assert.AreEqual("Custom error message", exception.Message);
    }

    [TestMethod]
    public void Sequence_PreservesOrder()
    {
        var results = new[]
        {
            new Result<int, string>.Ok<int, string>(3),
            new Result<int, string>.Ok<int, string>(1),
            new Result<int, string>.Ok<int, string>(2),
        };

        var sequenced = results.Sequence();
        var list = +sequenced;

        Assert.AreEqual(3, list[0]);
        Assert.AreEqual(1, list[1]);
        Assert.AreEqual(2, list[2]);
    }

    [TestMethod]
    public void Sequence_StopsAtFirstError()
    {
        var callCount = 0;
        var results = new[]
        {
            new Result<int, string>.Ok<int, string>(1),
            Result<int, string>.Failure("error"),
            new Result<int, string>.Ok<int, string>(3),
        }.Select(r =>
        {
            callCount++;
            return r;
        });

        var sequenced = results.Sequence();

        Assert.IsTrue(sequenced.IsError);
        Assert.AreEqual(2, callCount); // Sequence stops at first error
    }

    [TestMethod]
    public void Filter_ChainedWithOtherOperations_Works()
    {
        var result = new Result<int, string>.Ok<int, string>(5)
            .Map(static x => x * 2)
            .Filter(static x => x > 5, "too small")
            .Map(static x => x + 1);

        Assert.IsTrue(result.IsOk);
        Assert.AreEqual(11, +result);
    }

    [TestMethod]
    public void Combine_ChainedWithMap_Works()
    {
        var result1 = new Result<int, string>.Ok<int, string>(5);
        var result2 = new Result<int, string>.Ok<int, string>(3);

        var combined = result1.Combine(result2, static (a, b) => a + b).Map(static sum => sum * 2);

        Assert.IsTrue(combined.IsOk);
        Assert.AreEqual(16, +combined);
    }

    [TestMethod]
    public void Flatten_TripleNested_CanBeFlattenedTwice()
    {
        var tripleNested = new Result<Result<Result<int, string>, string>, string>.Ok<
            Result<Result<int, string>, string>,
            string
        >(
            new Result<Result<int, string>, string>.Ok<Result<int, string>, string>(
                new Result<int, string>.Ok<int, string>(42)
            )
        );

        var onceFlatted = tripleNested.Flatten();
        var twiceFlattened = onceFlatted.Flatten();

        Assert.IsTrue(twiceFlattened.IsOk);
        Assert.AreEqual(42, +twiceFlattened);
    }

    [TestMethod]
    public void Extensions_WorkWithComplexTypes()
    {
        var results = new[]
        {
            new Result<(int, string), Exception>.Ok<(int, string), Exception>((1, "one")),
            new Result<(int, string), Exception>.Ok<(int, string), Exception>((2, "two")),
        };

        var sequenced = results.Sequence();

        Assert.IsTrue(sequenced.IsOk);
        var list = +sequenced;
        Assert.AreEqual((1, "one"), list[0]);
        Assert.AreEqual((2, "two"), list[1]);
    }

    [TestMethod]
    public void Sequence_WithMixedResults_StopsOnFirstError()
    {
        var results = new List<Result<int, string>>
        {
            new Result<int, string>.Ok<int, string>(1),
            new Result<int, string>.Ok<int, string>(2),
            Result<int, string>.Failure("middle error"),
            new Result<int, string>.Ok<int, string>(4),
            Result<int, string>.Failure("later error"),
        };

        var sequenced = results.Sequence();

        Assert.IsTrue(sequenced.IsError);
        Assert.AreEqual("middle error", !sequenced);
    }
}
