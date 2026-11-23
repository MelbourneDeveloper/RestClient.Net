using System.Net;

namespace Outcome.Tests;

[TestClass]
public class HttpErrorTests
{
    private static System.Net.Http.Headers.HttpResponseHeaders CreateHeaders()
    {
        var response = new HttpResponseMessage();
        var headers = response.Headers;
        response.Dispose();
        return headers;
    }

    [TestMethod]
    public void ExceptionError_IsExceptionError_ReturnsTrue()
    {
        var error = HttpError<string>.FromException(new InvalidOperationException("test"));
        Assert.IsTrue(error.IsExceptionError);
    }

    [TestMethod]
    public void ExceptionError_IsErrorResponse_ReturnsFalse()
    {
        var error = HttpError<string>.FromException(new InvalidOperationException("test"));
        Assert.IsFalse(error.IsErrorResponse);
    }

    [TestMethod]
    public void ErrorResponseError_IsExceptionError_ReturnsFalse()
    {
        var headers = CreateHeaders();
        var error = HttpError<string>.FromErrorResponse("body", HttpStatusCode.BadRequest, headers);
        Assert.IsFalse(error.IsExceptionError);
    }

    [TestMethod]
    public void ErrorResponseError_IsErrorResponse_ReturnsTrue()
    {
        var headers = CreateHeaders();
        var error = HttpError<string>.FromErrorResponse("body", HttpStatusCode.BadRequest, headers);
        Assert.IsTrue(error.IsErrorResponse);
    }

    [TestMethod]
    public void FromException_CreatesExceptionError()
    {
        var exception = new InvalidOperationException("test");
        var error = HttpError<string>.FromException(exception);

        Assert.IsTrue(error is ExceptionErrorString);
        var exceptionError = (ExceptionErrorString)error;
        Assert.AreSame(exception, exceptionError.Exception);
    }

    [TestMethod]
    public void FromErrorResponse_CreatesErrorResponseError()
    {
        var headers = CreateHeaders();
        var error = HttpError<string>.FromErrorResponse(
            "test body",
            HttpStatusCode.NotFound,
            headers
        );

        Assert.IsTrue(error is ResponseErrorString);
        var responseError = (ResponseErrorString)error;
        Assert.AreEqual("test body", responseError.Body);
        Assert.AreEqual(HttpStatusCode.NotFound, responseError.StatusCode);
        Assert.AreSame(headers, responseError.Headers);
    }

    [TestMethod]
    public void Match_OnExceptionError_CallsOnException()
    {
        var exception = new InvalidOperationException("test");
        var error = HttpError<string>.FromException(exception);

        var result = error.Match(
            onException: static ex => $"Exception: {ex.Message}",
            onErrorResponse: static (body, status, _) => $"Response: {body}"
        );

        Assert.AreEqual("Exception: test", result);
    }

    [TestMethod]
    public void Match_OnErrorResponseError_CallsOnErrorResponse()
    {
        var headers = CreateHeaders();
        var error = HttpError<string>.FromErrorResponse(
            "error body",
            HttpStatusCode.BadRequest,
            headers
        );

        var result = error.Match(
            onException: static ex => $"Exception: {ex.Message}",
            onErrorResponse: static (body, status, _) => $"Response: {body} ({status})"
        );

        Assert.AreEqual("Response: error body (BadRequest)", result);
    }

    [TestMethod]
    public void ExceptionError_ToString_FormatsCorrectly()
    {
        var exception = new InvalidOperationException("test message");
        var error = new ExceptionErrorString(exception);
        var str = error.ToString();

        Assert.AreEqual("ExceptionError(InvalidOperationException: test message)", str);
    }

    [TestMethod]
    public void ErrorResponseError_ToString_FormatsCorrectly()
    {
        var headers = CreateHeaders();
        var error = new ResponseErrorString("error body", HttpStatusCode.NotFound, headers);
        var str = error.ToString();

        Assert.AreEqual("ErrorResponseError(NotFound: error body)", str);
    }

    [TestMethod]
    public void ExceptionError_FromException_CreatesError()
    {
        var exception = new ArgumentNullException("param");
        var error = ExceptionErrorString.FromException(exception);

        Assert.AreSame(exception, error.Exception);
    }

    [TestMethod]
    public void IsErrorResponseError_OnErrorResponse_ReturnsTrueWithValues()
    {
        var headers = CreateHeaders();
        var error = HttpError<string>.FromErrorResponse("test", HttpStatusCode.BadRequest, headers);

        var isErrorResponse = error.IsErrorResponseError(
            out var body,
            out var statusCode,
            out var outHeaders
        );

        Assert.IsTrue(isErrorResponse);
        Assert.AreEqual("test", body);
        Assert.AreEqual(HttpStatusCode.BadRequest, statusCode);
        Assert.AreSame(headers, outHeaders);
    }

    [TestMethod]
    public void IsErrorResponseError_OnExceptionError_ReturnsFalseWithDefaults()
    {
        var error = HttpError<string>.FromException(new Exception("test"));

        var isErrorResponse = error.IsErrorResponseError(
            out var body,
            out var statusCode,
            out var headers
        );

        Assert.IsFalse(isErrorResponse);
        Assert.AreEqual(default, body);
        Assert.AreEqual(default, statusCode);
        Assert.IsNull(headers);
    }

    [TestMethod]
    public void HttpError_WithComplexErrorType_Works()
    {
        var headers = CreateHeaders();
        var errorBody = new { Code = 400, Message = "Bad Request" };
        var error = HttpError<object>.FromErrorResponse(
            errorBody,
            HttpStatusCode.BadRequest,
            headers
        );

        var result = error.Match(
            onException: static _ => null!,
            onErrorResponse: static (body, _, _) => body
        );

        Assert.AreSame(errorBody, result);
    }

    [TestMethod]
    public void HttpError_Equality_WorksCorrectly()
    {
        var headers1 = CreateHeaders();

        var error1 = HttpError<string>.FromErrorResponse(
            "test",
            HttpStatusCode.BadRequest,
            headers1
        );
        var error2 = HttpError<string>.FromErrorResponse(
            "test",
            HttpStatusCode.BadRequest,
            headers1
        );
        var error3 = HttpError<string>.FromErrorResponse(
            "different",
            HttpStatusCode.BadRequest,
            headers1
        );

        Assert.AreEqual(error1, error2);
        Assert.AreNotEqual(error1, error3);

        var exception = new InvalidOperationException("test");
        var exError1 = HttpError<string>.FromException(exception);
        var exError2 = HttpError<string>.FromException(exception);

        Assert.AreEqual(exError1, exError2);
    }

    [TestMethod]
    public void HttpError_WithNullableErrorType_Works()
    {
        var headers = CreateHeaders();
        var error = HttpError<string?>.FromErrorResponse(null, HttpStatusCode.BadRequest, headers);

        var result = error.Match(
            onException: static _ => "exception",
            onErrorResponse: static (body, _, _) => body ?? "null"
        );

        Assert.AreEqual("null", result);
    }

    [TestMethod]
    public void ErrorResponseError_WithDifferentStatusCodes_Works()
    {
        var headers = CreateHeaders();
        var statusCodes = new[]
        {
            HttpStatusCode.BadRequest,
            HttpStatusCode.Unauthorized,
            HttpStatusCode.Forbidden,
            HttpStatusCode.NotFound,
            HttpStatusCode.InternalServerError,
        };

        foreach (var statusCode in statusCodes)
        {
            var error = HttpError<string>.FromErrorResponse("error", statusCode, headers);
            Assert.IsTrue(error.IsErrorResponse);

            var responseError = (ResponseErrorString)error;
            Assert.AreEqual(statusCode, responseError.StatusCode);
        }
    }

    [TestMethod]
    public void HttpError_InResultContext_Works()
    {
        var headers = CreateHeaders();
        var error = HttpError<string>.FromErrorResponse(
            "api error",
            HttpStatusCode.BadRequest,
            headers
        );
        var result = Result<string, HttpError<string>>.Failure(error);

        Assert.IsTrue(result.IsError);
        var extractedError = !result;
        Assert.IsTrue(extractedError.IsErrorResponse);
    }

    [TestMethod]
    public void ErrorResponseError_Constructor_CreatesInstanceWithAllProperties()
    {
        var headers = CreateHeaders();
        headers.Add("X-Custom-Header", "test-value");
        var errorBody = "detailed error message";
        var statusCode = HttpStatusCode.Conflict;

        var error = new ResponseErrorString(errorBody, statusCode, headers);

        Assert.AreEqual(errorBody, error.Body);
        Assert.AreEqual(statusCode, error.StatusCode);
        Assert.AreSame(headers, error.Headers);
        Assert.IsTrue(error.Headers.Contains("X-Custom-Header"));
        Assert.AreEqual("test-value", error.Headers.GetValues("X-Custom-Header").First());
    }

    [TestMethod]
    public void ErrorResponseError_ToString_WithComplexBody_FormatsCorrectly()
    {
        var headers = CreateHeaders();
        var complexBody = new
        {
            ErrorCode = 1001,
            Message = "Validation failed",
            Details = "Field X is required",
        };
        var error = new HttpError<object>.ErrorResponseError(
            complexBody,
            HttpStatusCode.UnprocessableEntity,
            headers
        );

        var toString = error.ToString();

        Assert.IsTrue(toString.Contains("UnprocessableEntity", StringComparison.Ordinal));
        Assert.IsTrue(toString.Contains("ErrorCode", StringComparison.Ordinal));
        Assert.IsTrue(toString.Contains("1001", StringComparison.Ordinal));
    }

    [TestMethod]
    public void ErrorResponseError_WithEmptyBody_Works()
    {
        var headers = CreateHeaders();
        var error = new ResponseErrorString(string.Empty, HttpStatusCode.NoContent, headers);

        Assert.AreEqual(string.Empty, error.Body);
        Assert.AreEqual(HttpStatusCode.NoContent, error.StatusCode);
        Assert.AreEqual("ErrorResponseError(NoContent: )", error.ToString());
    }

    [TestMethod]
    public void ErrorResponseError_Record_WithExpression_CreatesNewInstance()
    {
        var headers = CreateHeaders();
        var error1 = new ResponseErrorString("original", HttpStatusCode.BadRequest, headers);
        var error2 = error1 with { Body = "modified" };

        Assert.AreEqual("original", error1.Body);
        Assert.AreEqual("modified", error2.Body);
        Assert.AreEqual(error1.StatusCode, error2.StatusCode);
        Assert.AreNotSame(error1, error2);
    }

    [TestMethod]
    public void ErrorResponseError_Deconstruct_ExtractsAllProperties()
    {
        var headers = CreateHeaders();
        var error = new ResponseErrorString("error message", HttpStatusCode.Forbidden, headers);

        var (body, statusCode, extractedHeaders) = error;

        Assert.AreEqual("error message", body);
        Assert.AreEqual(HttpStatusCode.Forbidden, statusCode);
        Assert.AreSame(headers, extractedHeaders);
    }

    [TestMethod]
    public void ErrorResponseError_GetHashCode_ConsistentWithEquals()
    {
        var headers = CreateHeaders();
        var error1 = new ResponseErrorString("test", HttpStatusCode.BadRequest, headers);
        var error2 = new ResponseErrorString("test", HttpStatusCode.BadRequest, headers);

        Assert.AreEqual(error1.GetHashCode(), error2.GetHashCode());
        Assert.AreEqual(error1, error2);
    }

    [TestMethod]
    public void ErrorResponseError_WithAllStatusCodeRanges_CreatesCorrectly()
    {
        var headers = CreateHeaders();
        var statusCodes = new[]
        {
            (HttpStatusCode)400, // Client errors
            (HttpStatusCode)404,
            (HttpStatusCode)409,
            (HttpStatusCode)422,
            (HttpStatusCode)429,
            (HttpStatusCode)500, // Server errors
            (HttpStatusCode)502,
            (HttpStatusCode)503,
            (HttpStatusCode)504,
        };

        foreach (var statusCode in statusCodes)
        {
            var error = new ResponseErrorString($"Error {(int)statusCode}", statusCode, headers);

            Assert.AreEqual(statusCode, error.StatusCode);
            Assert.AreEqual($"Error {(int)statusCode}", error.Body);
            Assert.IsTrue(
                error.ToString().Contains(statusCode.ToString(), StringComparison.Ordinal)
            );
        }
    }

    [TestMethod]
    public void IsErrorResponseError_MultipleCallsOnSameInstance_ReturnsSameValues()
    {
        var headers = CreateHeaders();
        var error = HttpError<string>.FromErrorResponse("test", HttpStatusCode.BadRequest, headers);

        var result1 = error.IsErrorResponseError(out var body1, out var status1, out var headers1);
        var result2 = error.IsErrorResponseError(out var body2, out var status2, out var headers2);

        Assert.IsTrue(result1);
        Assert.IsTrue(result2);
        Assert.AreEqual(body1, body2);
        Assert.AreEqual(status1, status2);
        Assert.AreSame(headers1, headers2);
    }
}
