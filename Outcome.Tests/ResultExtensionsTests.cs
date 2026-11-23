namespace Outcome.Tests;

[TestClass]
public class ResultExtensionsTests
{
    private static readonly int[] ThreeValuesInOrder = [1, 2, 3];
    private static readonly int[] TwoValuesInOrder = [1, 2];
    private static readonly int[] ValuesInInsertionOrder = [3, 1, 2];

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

        OutcomeAssertions.Success(
            sequenced.Map(static values => string.Join(",", values)),
            "1,2,3"
        );
        CollectionAssert.AreEqual(ThreeValuesInOrder, (+results.Sequence()).ToArray());
        OutcomeAssertions.Success(results[0], 1);
        OutcomeAssertions.Success(results[1], 2);
        OutcomeAssertions.Success(results[2], 3);
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

        OutcomeAssertions.Error(sequenced, "first error");
        OutcomeAssertions.Success(results[0], 1);
        OutcomeAssertions.Error(results[2], "second error");
    }

    [TestMethod]
    public void Sequence_WithEmptyEnumerable_ReturnsEmptyList()
    {
        var results = Array.Empty<Result<int, string>>();

        var sequenced = results.Sequence();

        Assert.IsTrue(sequenced.IsOk);
        var list = +sequenced;
        Assert.AreEqual(0, list.Count);

        OutcomeAssertions.Success(sequenced.Map(static values => values.Count), 0);
        Assert.AreEqual(0, (+results.Sequence()).Count);
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

        OutcomeAssertions.Success(sequenced.Map(static values => string.Join(",", values)), "1,2");
        CollectionAssert.AreEqual(TwoValuesInOrder, (+results.Sequence()).ToArray());
        OutcomeAssertions.Success(results[0], 1);
        OutcomeAssertions.Success(results[1], 2);
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

        OutcomeAssertions.Success(flattened, 42);
        Assert.AreSame(+nested, flattened);
        Assert.AreSame(flattened, nested.Flatten());
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

        OutcomeAssertions.Error(flattened, "inner error");
        Assert.AreSame(+nested, flattened);
        Assert.AreSame(flattened, nested.Flatten());
    }

    [TestMethod]
    public void Flatten_WithError_ReturnsError()
    {
        var nested = Result<Result<int, string>, string>.Failure("outer error");

        var flattened = nested.Flatten();

        Assert.IsTrue(flattened.IsError);
        Assert.AreEqual("outer error", !flattened);

        OutcomeAssertions.Error(flattened, "outer error");
        OutcomeAssertions.Error(nested, "outer error");
    }

    [TestMethod]
    public void Combine_WithTwoOk_CallsCombiner()
    {
        var result1 = new Result<int, string>.Ok<int, string>(5);
        var result2 = new Result<int, string>.Ok<int, string>(3);

        var combineCalls = 0;
        var combined = result1.Combine(
            result2,
            (a, b) =>
            {
                combineCalls++;
                Assert.AreEqual(5, a);
                Assert.AreEqual(3, b);
                return a + b;
            }
        );

        Assert.IsTrue(combined.IsOk);
        Assert.AreEqual(8, +combined);

        OutcomeAssertions.Success(combined, 8);
        OutcomeAssertions.Success(result1, 5);
        OutcomeAssertions.Success(result2, 3);
        Assert.AreEqual(1, combineCalls);
    }

    [TestMethod]
    public void Combine_WithFirstError_ReturnsFirstError()
    {
        var result1 = Result<int, string>.Failure("error 1");
        var result2 = new Result<int, string>.Ok<int, string>(3);

        var combineCalls = 0;
        var combined = result1.Combine(
            result2,
            (a, b) =>
            {
                combineCalls++;
                Assert.AreEqual(5, a);
                Assert.AreEqual(3, b);
                return a + b;
            }
        );

        Assert.IsTrue(combined.IsError);
        Assert.AreEqual("error 1", !combined);

        OutcomeAssertions.Error(combined, "error 1");
        OutcomeAssertions.Error(result1, "error 1");
        OutcomeAssertions.Success(result2, 3);
        Assert.AreEqual(0, combineCalls);
    }

    [TestMethod]
    public void Combine_WithSecondError_ReturnsSecondError()
    {
        var result1 = new Result<int, string>.Ok<int, string>(5);
        var result2 = Result<int, string>.Failure("error 2");

        var combineCalls = 0;
        var combined = result1.Combine(
            result2,
            (a, b) =>
            {
                combineCalls++;
                Assert.AreEqual(5, a);
                Assert.AreEqual(3, b);
                return a + b;
            }
        );

        Assert.IsTrue(combined.IsError);
        Assert.AreEqual("error 2", !combined);

        OutcomeAssertions.Error(combined, "error 2");
        OutcomeAssertions.Success(result1, 5);
        OutcomeAssertions.Error(result2, "error 2");
        Assert.AreEqual(0, combineCalls);
    }

    [TestMethod]
    public void Combine_WithBothErrors_ReturnsFirstError()
    {
        var result1 = Result<int, string>.Failure("error 1");
        var result2 = Result<int, string>.Failure("error 2");

        var combineCalls = 0;
        var combined = result1.Combine(
            result2,
            (a, b) =>
            {
                combineCalls++;
                Assert.AreEqual(5, a);
                Assert.AreEqual(3, b);
                return a + b;
            }
        );

        Assert.IsTrue(combined.IsError);
        Assert.AreEqual("error 1", !combined);

        OutcomeAssertions.Error(combined, "error 1");
        OutcomeAssertions.Error(result1, "error 1");
        OutcomeAssertions.Error(result2, "error 2");
        Assert.AreEqual(0, combineCalls);
    }

    [TestMethod]
    public void Combine_WithDifferentTypes_Works()
    {
        var result1 = new Result<int, string>.Ok<int, string>(5);
        var result2 = new Result<string, string>.Ok<string, string>("test");

        var combineCalls = 0;
        var combined = result1.Combine(
            result2,
            (num, str) =>
            {
                combineCalls++;
                Assert.AreEqual(5, num);
                Assert.AreEqual("test", str);
                return $"{str}: {num}";
            }
        );

        Assert.IsTrue(combined.IsOk);
        Assert.AreEqual("test: 5", +combined);

        OutcomeAssertions.Success(combined, "test: 5");
        OutcomeAssertions.Success(result1, 5);
        OutcomeAssertions.Success(result2, "test");
        Assert.AreEqual(1, combineCalls);
    }

    [TestMethod]
    public void Filter_WithPredicateTrue_ReturnsOriginal()
    {
        var result = new Result<int, string>.Ok<int, string>(42);

        var predicateCalls = 0;
        var filtered = result.Filter(
            x =>
            {
                predicateCalls++;
                Assert.AreEqual(42, x);
                return x > 10;
            },
            "too small"
        );

        Assert.IsTrue(filtered.IsOk);
        Assert.AreEqual(42, +filtered);

        OutcomeAssertions.Success(filtered, 42);
        Assert.AreEqual(result, filtered);
        Assert.AreEqual(1, predicateCalls);
    }

    [TestMethod]
    public void Filter_WithPredicateFalse_ReturnsError()
    {
        var result = new Result<int, string>.Ok<int, string>(5);

        var predicateCalls = 0;
        var filtered = result.Filter(
            x =>
            {
                predicateCalls++;
                Assert.AreEqual(5, x);
                return x > 10;
            },
            "too small"
        );

        Assert.IsTrue(filtered.IsError);
        Assert.AreEqual("too small", !filtered);

        OutcomeAssertions.Error(filtered, "too small");
        OutcomeAssertions.Success(result, 5);
        Assert.AreEqual(1, predicateCalls);
    }

    [TestMethod]
    public void Filter_OnError_PropagatesError()
    {
        var result = Result<int, string>.Failure("original error");

        var predicateCalls = 0;
        var filtered = result.Filter(
            x =>
            {
                predicateCalls++;
                Assert.AreEqual(5, x);
                return x > 10;
            },
            "predicate error"
        );

        Assert.IsTrue(filtered.IsError);
        Assert.AreEqual("original error", !filtered);

        OutcomeAssertions.Error(filtered, "original error");
        Assert.AreEqual(result, filtered);
        Assert.AreEqual(0, predicateCalls);
    }

    [TestMethod]
    public void GetValueOrThrow_OnOk_ReturnsValue()
    {
        var result = new Result<int, string>.Ok<int, string>(42);

        var value = result.GetValueOrThrow();

        Assert.AreEqual(42, value);

        OutcomeAssertions.Success(result, 42);
    }

    [TestMethod]
    public void GetValueOrThrow_OnError_ThrowsWithDefaultMessage()
    {
        var result = Result<int, string>.Failure("error");

        var exception = Assert.ThrowsException<InvalidOperationException>(
            () => result.GetValueOrThrow()
        );
        Assert.AreEqual("Expected success result", exception.Message);

        OutcomeAssertions.Error(result, "error");
        Assert.IsNull(exception.InnerException);
    }

    [TestMethod]
    public void GetValueOrThrow_OnError_ThrowsWithCustomMessage()
    {
        var result = Result<int, string>.Failure("error");

        var exception = Assert.ThrowsException<InvalidOperationException>(
            () => result.GetValueOrThrow("Custom error message")
        );
        Assert.AreEqual("Custom error message", exception.Message);

        OutcomeAssertions.Error(result, "error");
        Assert.IsNull(exception.InnerException);
    }

    [TestMethod]
    public void GetErrorOrThrow_OnError_ReturnsError()
    {
        var result = Result<int, string>.Failure("test error");

        var error = result.GetErrorOrThrow();

        Assert.AreEqual("test error", error);

        OutcomeAssertions.Error(result, "test error");
    }

    [TestMethod]
    public void GetErrorOrThrow_OnOk_ThrowsWithDefaultMessage()
    {
        var result = new Result<int, string>.Ok<int, string>(42);

        var exception = Assert.ThrowsException<InvalidOperationException>(
            () => result.GetErrorOrThrow()
        );
        Assert.AreEqual("Expected error result", exception.Message);

        OutcomeAssertions.Success(result, 42);
        Assert.IsNull(exception.InnerException);
    }

    [TestMethod]
    public void GetErrorOrThrow_OnOk_ThrowsWithCustomMessage()
    {
        var result = new Result<int, string>.Ok<int, string>(42);

        var exception = Assert.ThrowsException<InvalidOperationException>(
            () => result.GetErrorOrThrow("Custom error message")
        );
        Assert.AreEqual("Custom error message", exception.Message);

        OutcomeAssertions.Success(result, 42);
        Assert.IsNull(exception.InnerException);
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

        OutcomeAssertions.Success(
            sequenced.Map(static values => string.Join(",", values)),
            "3,1,2"
        );
        CollectionAssert.AreEqual(ValuesInInsertionOrder, (+results.Sequence()).ToArray());
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

        OutcomeAssertions.Error(sequenced, "error");
        OutcomeAssertions.Error(results.Sequence(), "error");
        Assert.AreEqual(4, callCount, "Each enumeration must stop at the first error.");
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

        OutcomeAssertions.Success(result, 11);
    }

    [TestMethod]
    public void Combine_ChainedWithMap_Works()
    {
        var result1 = new Result<int, string>.Ok<int, string>(5);
        var result2 = new Result<int, string>.Ok<int, string>(3);

        var combineCalls = 0;
        var combined = result1
            .Combine(
                result2,
                (a, b) =>
                {
                    combineCalls++;
                    Assert.AreEqual(5, a);
                    Assert.AreEqual(3, b);
                    return a + b;
                }
            )
            .Map(static sum => sum * 2);

        Assert.IsTrue(combined.IsOk);
        Assert.AreEqual(16, +combined);

        OutcomeAssertions.Success(combined, 16);
        OutcomeAssertions.Success(result1, 5);
        OutcomeAssertions.Success(result2, 3);
        Assert.AreEqual(1, combineCalls);
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

        OutcomeAssertions.Success(twiceFlattened, 42);
        Assert.AreSame(+tripleNested, onceFlatted);
        Assert.AreSame(+onceFlatted, twiceFlattened);
        Assert.AreSame(twiceFlattened, tripleNested.Flatten().Flatten());
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

        OutcomeAssertions.Success(sequenced.Map(static values => values.Count), 2);
        OutcomeAssertions.Success(results[0], (1, "one"));
        OutcomeAssertions.Success(results[1], (2, "two"));
        CollectionAssert.AreEqual(
            new[] { (1, "one"), (2, "two") },
            (+results.Sequence()).ToArray()
        );
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

        OutcomeAssertions.Error(sequenced, "middle error");
        OutcomeAssertions.Error(results[4], "later error");
        OutcomeAssertions.Error(results.Sequence(), "middle error");
    }
}
