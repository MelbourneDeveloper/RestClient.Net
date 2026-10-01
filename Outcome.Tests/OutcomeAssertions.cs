using System.Net;
using System.Net.Http.Headers;

namespace Outcome.Tests;

internal static class OutcomeAssertions
{
    internal static void Success<TValue, TError>(Result<TValue, TError> result, TValue expected)
    {
        Assert.IsInstanceOfType<Result<TValue, TError>.Ok<TValue, TError>>(result);
        Assert.IsTrue(result.IsOk);
        Assert.IsFalse(result.IsError);
        Assert.AreEqual(expected, +result);

        var matches = 0;
        var selected = result.Match(
            value =>
            {
                matches++;
                Assert.AreEqual(expected, value);
                return "success";
            },
            _ => throw new AssertFailedException("Success invoked the error callback.")
        );
        Assert.AreEqual("success", selected);
        Assert.AreEqual(1, matches);

        var taps = 0;
        var tapped = result.Tap(
            value =>
            {
                taps++;
                Assert.AreEqual(expected, value);
            },
            _ => Assert.Fail("Success invoked the error tap.")
        );
        Assert.AreSame(result, tapped);
        Assert.AreEqual(1, taps);

        var maps = 0;
        var mapped = result.Map(value =>
        {
            maps++;
            Assert.AreEqual(expected, value);
            return (value, "mapped");
        });
        Assert.IsTrue(mapped.IsOk);
        Assert.IsFalse(mapped.IsError);
        Assert.AreEqual((expected, "mapped"), +mapped);
        Assert.AreEqual(1, maps);

        var binds = 0;
        var bound = mapped.Bind(value =>
        {
            binds++;
            Assert.AreEqual((expected, "mapped"), value);
            return Result<string, TError>.Failure(default!);
        });
        Assert.IsFalse(bound.IsOk);
        Assert.IsTrue(bound.IsError);
        Assert.AreEqual(default, !bound);
        Assert.AreEqual(1, binds);

        var unchanged = result.MapError<string>(_ =>
            throw new AssertFailedException("Success invoked the error mapper.")
        );
        Assert.IsTrue(unchanged.IsOk);
        Assert.AreEqual(expected, +unchanged);
        Assert.AreEqual(
            expected,
            result.GetValueOrDefault(
                () => throw new AssertFailedException("Success invoked the fallback provider.")
            )
        );
        Assert.AreEqual(expected, +result, "Chaining must preserve the original success.");
    }

    internal static void Error<TValue, TError>(Result<TValue, TError> result, TError expected)
    {
        Assert.IsInstanceOfType<Result<TValue, TError>.Error<TValue, TError>>(result);
        Assert.IsFalse(result.IsOk);
        Assert.IsTrue(result.IsError);
        Assert.AreEqual(expected, !result);

        var matches = 0;
        var selected = result.Match(
            _ => throw new AssertFailedException("Error invoked the success callback."),
            error =>
            {
                matches++;
                Assert.AreEqual(expected, error);
                return "error";
            }
        );
        Assert.AreEqual("error", selected);
        Assert.AreEqual(1, matches);

        var taps = 0;
        var tapped = result.Tap(
            _ => Assert.Fail("Error invoked the success tap."),
            error =>
            {
                taps++;
                Assert.AreEqual(expected, error);
            }
        );
        Assert.AreSame(result, tapped);
        Assert.AreEqual(1, taps);

        var mapped = result.Map<string>(_ =>
            throw new AssertFailedException("Error invoked the success mapper.")
        );
        var bound = mapped.Bind<int>(_ =>
            throw new AssertFailedException("Error invoked the binder.")
        );
        Assert.IsFalse(mapped.IsOk);
        Assert.IsTrue(mapped.IsError);
        Assert.AreEqual(expected, !mapped);
        Assert.IsFalse(bound.IsOk);
        Assert.IsTrue(bound.IsError);
        Assert.AreEqual(expected, !bound);

        var maps = 0;
        var mappedError = result.MapError(error =>
        {
            maps++;
            Assert.AreEqual(expected, error);
            return (error, "mapped");
        });
        Assert.IsFalse(mappedError.IsOk);
        Assert.IsTrue(mappedError.IsError);
        Assert.AreEqual((expected, "mapped"), !mappedError);
        Assert.AreEqual(1, maps);

        var fallbacks = 0;
        var fallback = mapped.GetValueOrDefault(() =>
        {
            fallbacks++;
            return "recovered";
        });
        Assert.AreEqual("recovered", fallback);
        Assert.AreEqual(1, fallbacks);
        Assert.AreEqual(expected, !result, "Chaining must preserve the original failure.");
    }

    internal static void Response<T>(
        HttpError<T> error,
        T expectedBody,
        HttpStatusCode expectedStatus,
        HttpResponseHeaders expectedHeaders
    )
    {
        Assert.IsInstanceOfType<HttpError<T>.ErrorResponseError>(error);
        Assert.IsTrue(error.IsErrorResponse);
        Assert.IsFalse(error.IsExceptionError);
        var calls = 0;
        var matched = error.Match(
            _ => throw new AssertFailedException("HTTP response invoked the exception callback."),
            (body, status, headers) =>
            {
                calls++;
                Assert.AreEqual(expectedBody, body);
                Assert.AreEqual(expectedStatus, status);
                Assert.AreSame(expectedHeaders, headers);
                return body;
            }
        );
        Assert.AreEqual(1, calls);
        Assert.AreEqual(expectedBody, matched);
        Assert.IsTrue(
            error.IsErrorResponseError(
                out var actualBody,
                out var actualStatus,
                out var actualHeaders
            )
        );
        Assert.AreEqual(expectedBody, actualBody);
        Assert.AreEqual(expectedStatus, actualStatus);
        Assert.AreSame(expectedHeaders, actualHeaders);
        var wrapped = Result<int, HttpError<T>>.Failure(error);
        Error(wrapped, error);
        Assert.AreSame(error, !wrapped);
    }

    internal static void Exception<T>(HttpError<T> error, string expectedMessage, Type expectedType)
    {
        Assert.IsInstanceOfType<HttpError<T>.ExceptionError>(error);
        Assert.IsTrue(error.IsExceptionError);
        Assert.IsFalse(error.IsErrorResponse);
        var calls = 0;
        var matched = error.Match(
            exception =>
            {
                calls++;
                Assert.AreEqual(expectedMessage, exception.Message);
                Assert.AreEqual(expectedType, exception.GetType());
                Assert.AreSame(((HttpError<T>.ExceptionError)error).Exception, exception);
                return default!;
            },
            (_, _, _) =>
                throw new AssertFailedException("Exception invoked the HTTP response callback.")
        );
        Assert.AreEqual(1, calls);
        Assert.AreEqual(default, matched);
        Assert.IsFalse(error.IsErrorResponseError(out var body, out var status, out var headers));
        Assert.AreEqual(default, body);
        Assert.AreEqual(default, status);
        Assert.IsNull(headers);
        var wrapped = Result<int, HttpError<T>>.Failure(error);
        Error(wrapped, error);
        Assert.AreSame(error, !wrapped);
    }
}
