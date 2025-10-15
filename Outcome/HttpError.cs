using System.Net;
using System.Net.Http.Headers;

// Not relevant here
#pragma warning disable CA1000 // Do not declare static members on generic types

namespace Outcome;

/// <summary>
/// Represents HTTP-specific errors that can occur during web requests.
/// This sealed hierarchy ensures complete error handling coverage for HTTP operations.
///
/// Follows the functional programming principle of making illegal states unrepresentable
/// by using a discriminated union instead of inheritance hierarchies.
/// </summary>
/// <typeparam name="TError">The type representing parsed error response bodies.</typeparam>
public abstract partial record HttpError<TError>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="HttpError{TError}"/> class.
    /// Private constructor ensures this is a closed hierarchy.
    /// Only ExceptionError and ErrorResponseError can inherit from HttpError.
    /// </summary>
    private HttpError() { }

    /// <summary>
    /// Gets a value indicating whether this error was caused by an exception.
    /// </summary>
    public bool IsExceptionError => this is ExceptionError;

    /// <summary>
    /// Gets a value indicating whether this error was caused by an HTTP error response.
    /// </summary>
    public bool IsErrorResponse => this is ErrorResponseError;

    /// <summary>
    /// Transforms this HttpError into a new type using the provided functions.
    /// This is the catamorphism for HttpError, enabling complete pattern matching.
    /// </summary>
    /// <param name="onException">Function to apply if this is an ExceptionError.</param>
    /// <param name="onErrorResponse">Function to apply if this is an ErrorResponseError.</param>
    /// <returns>The result of applying the appropriate function.</returns>
    public TError Match(
        Func<Exception, TError> onException,
        Func<TError, HttpStatusCode, HttpResponseHeaders, TError> onErrorResponse
#pragma warning disable CS8509 // The switch expression does not handle all possible values of its input type (it is not exhaustive).
    ) =>
        this switch
        {
            ExceptionError(var exception) => onException(exception),
            ErrorResponseError(var body, var statusCode, var headers) => onErrorResponse(
                body,
                statusCode,
                headers
            ),
        };
#pragma warning restore CS8509 // The switch expression does not handle all possible values of its input type (it is not exhaustive).

    /// <summary>
    /// Creates an HttpError from an exception.
    /// This is the canonical way to represent infrastructure errors.
    /// </summary>
    /// <param name="exception">The exception that occurred.</param>
    /// <returns>An ExceptionError containing the exception.</returns>
    public static HttpError<TError> FromException(Exception exception) =>
        new ExceptionError(exception);

    /// <summary>
    /// Creates an HttpError from an error response.
    /// This is the canonical way to represent structured API errors.
    /// </summary>
    /// <param name="body">The parsed error response body.</param>
    /// <param name="statusCode">The HTTP status code.</param>
    /// <param name="headers">The HTTP response headers.</param>
    /// <returns>An ErrorResponseError containing the error details.</returns>
    public static HttpError<TError> FromErrorResponse(
        TError body,
        HttpStatusCode statusCode,
        HttpResponseHeaders headers
    ) => new ErrorResponseError(body, statusCode, headers);
}
