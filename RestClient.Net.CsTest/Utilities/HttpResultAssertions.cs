using Outcome;

namespace RestClient.Net.CsTest.Utilities;

internal static class HttpResultAssertions
{
    internal static void Success<TSuccess, TError>(Result<TSuccess, HttpError<TError>> result)
    {
        Assert.IsTrue(result.IsOk);
        Assert.IsFalse(result.IsError);
        var value = +result;
        var successCalls = 0;
        var errorCalls = 0;
        var matched = result.Match(
            actual =>
            {
                successCalls++;
                Assert.AreEqual(value, actual);
                return actual;
            },
            _ =>
            {
                errorCalls++;
                Assert.Fail("Success must not invoke the error callback.");
                return default!;
            }
        );
        Assert.AreEqual(value, matched);
        Assert.AreEqual(1, successCalls);
        Assert.AreEqual(0, errorCalls);

        var tapped = result.Tap(
            actual =>
            {
                successCalls++;
                Assert.AreEqual(value, actual);
            },
            _ =>
            {
                errorCalls++;
                Assert.Fail("Success must not invoke the error observer.");
            }
        );
        Assert.AreSame(result, tapped);
        Assert.AreEqual(2, successCalls);
        Assert.AreEqual(0, errorCalls);

        var mapped = result.Map(actual =>
        {
            successCalls++;
            Assert.AreEqual(value, actual);
            return actual;
        });
        Assert.IsTrue(mapped.IsOk);
        Assert.IsFalse(mapped.IsError);
        Assert.AreEqual(value, +mapped);
        Assert.AreEqual(3, successCalls);
        var bound = result.Bind(actual =>
        {
            successCalls++;
            Assert.AreEqual(value, actual);
            return result;
        });
        Assert.AreSame(result, bound);
        Assert.AreEqual(4, successCalls);
        var mappedError = result.MapError(error =>
        {
            errorCalls++;
            return error;
        });
        Assert.IsTrue(mappedError.IsOk);
        Assert.AreEqual(value, +mappedError);
        Assert.AreEqual(0, errorCalls);

        var fallbackCalls = 0;
        Assert.AreEqual(
            value,
            result.GetValueOrDefault(() =>
            {
                fallbackCalls++;
                return default!;
            })
        );
        Assert.AreEqual(0, fallbackCalls);
        var wrongBranch = Assert.ThrowsException<InvalidOperationException>(() =>
        {
            _ = !result;
        });
        Assert.AreEqual("Expected error result", wrongBranch.Message);
        Assert.IsTrue(result.IsOk);
        Assert.IsFalse(result.IsError);
        Assert.AreEqual(value, +result);
    }

    internal static void Failure<TSuccess, TError>(Result<TSuccess, HttpError<TError>> result)
    {
        Assert.IsFalse(result.IsOk);
        Assert.IsTrue(result.IsError);
        var error = !result;
        Assert.IsNotNull(error);
        var successCalls = 0;
        var errorCalls = 0;
        var matched = result.Match(
            _ =>
            {
                successCalls++;
                Assert.Fail("Failure must not invoke the success callback.");
                return error;
            },
            actual =>
            {
                errorCalls++;
                Assert.AreSame(error, actual);
                return actual;
            }
        );
        Assert.AreSame(error, matched);
        Assert.AreEqual(0, successCalls);
        Assert.AreEqual(1, errorCalls);

        var tapped = result.Tap(
            _ =>
            {
                successCalls++;
                Assert.Fail("Failure must not invoke the success observer.");
            },
            actual =>
            {
                errorCalls++;
                Assert.AreSame(error, actual);
            }
        );
        Assert.AreSame(result, tapped);
        Assert.AreEqual(0, successCalls);
        Assert.AreEqual(2, errorCalls);
        var mapped = result.Map(value =>
        {
            successCalls++;
            return value;
        });
        Assert.IsTrue(mapped.IsError);
        Assert.IsFalse(mapped.IsOk);
        Assert.AreSame(error, !mapped);
        var bound = result.Bind(_ =>
        {
            successCalls++;
            return result;
        });
        Assert.IsTrue(bound.IsError);
        Assert.AreSame(error, !bound);
        Assert.AreEqual(0, successCalls);

        var mappedError = result.MapError(actual =>
        {
            errorCalls++;
            Assert.AreSame(error, actual);
            return actual;
        });
        Assert.IsTrue(mappedError.IsError);
        Assert.AreSame(error, !mappedError);
        Assert.AreEqual(3, errorCalls);
        var fallbackCalls = 0;
        Assert.AreEqual(
            default,
            result.GetValueOrDefault(() =>
            {
                fallbackCalls++;
                return default!;
            })
        );
        Assert.AreEqual(1, fallbackCalls);
        var wrongBranch = Assert.ThrowsException<InvalidOperationException>(() =>
        {
            _ = +result;
        });
        Assert.AreEqual("Expected success result", wrongBranch.Message);
        Assert.IsTrue(result.IsError);
        Assert.AreSame(error, !result);
    }
}
