using System.Net;
using System.Net.Http.Headers;

// Not relevant here
#pragma warning disable CA1000 // Do not declare static members on generic types
// This is the workaround for the lack of Discriminated Unions in C#
#pragma warning disable CA1034 // Nested types should not be visible

namespace Outcome;

public abstract partial record HttpError<TError>
{
    /// <summary>
    /// Represents an HTTP error response from the server.
    /// This occurs when the server responds with an error status code (4xx, 5xx)
    /// but the response body can be parsed into a structured error format.
    ///
    /// This enables rich error handling where API errors can be strongly typed
    /// and contain detailed information about what went wrong.
    /// </summary>
    /// <param name="Body">The parsed error response body.</param>
    /// <param name="StatusCode">The HTTP status code of the error response.</param>
    /// <param name="Headers">The HTTP response headers.</param>

    public sealed record ErrorResponseError(
        TError Body,
        HttpStatusCode StatusCode,
        HttpResponseHeaders Headers
    ) : HttpError<TError>
    {
        /// <summary>
        /// Returns a string representation of the error response.
        /// </summary>
        /// <returns>A string representation of the error response.</returns>
        public override string ToString() => $"ErrorResponseError({StatusCode}: {Body})";
    }

    /// <summary>
    /// Attempts to extract the ErrorResponseError details from this HttpError.
    /// Returns true if this is an ErrorResponseError and outputs the details.
    /// Returns false if this is an ExceptionError.
    /// </summary>
    /// <param name="body">The error response body if ErrorResponseError, default otherwise.</param>
    /// <param name="statusCode">The HTTP status code if ErrorResponseError, default otherwise.</param>
    /// <param name="headers">The HTTP response headers if ErrorResponseError, null otherwise.</param>
    /// <returns>True if ErrorResponseError, false if ExceptionError.</returns>
#pragma warning disable CA1021 // Avoid out parameters
    public bool IsErrorResponseError(
        out TError body,
        out HttpStatusCode statusCode,
        out HttpResponseHeaders? headers
    )
#pragma warning restore CA1021 // Avoid out parameters
    {
        if (this is ErrorResponseError err)
        {
            body = err.Body;
            statusCode = err.StatusCode;
            headers = err.Headers;
            return true;
        }

        body = default!;
        statusCode = default;
        headers = null;
        return false;
    }
}
