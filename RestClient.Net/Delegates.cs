using Outcome;
using Urls;

namespace RestClient.Net;

/// <summary>
/// Represents the parts of an HTTP request including URL, body, and headers.
/// </summary>
/// <param name="RelativeUrl">The relative URL for the request.</param>
/// <param name="Body">The optional HTTP content for the request body.</param>
/// <param name="Headers">The optional headers to include in the request.</param>
public readonly record struct HttpRequestParts(
    RelativeUrl RelativeUrl,
    HttpContent? Body,
    IReadOnlyDictionary<string, string>? Headers
);

/// <summary>
/// Represents a function that builds all parts of an HTTP request from a typed parameter.
/// </summary>
/// <typeparam name="TParam">The type of parameter used to build the request.</typeparam>
/// <param name="argument">The parameter used to construct the request parts.</param>
/// <returns>The HTTP request parts including URL, body, and headers.</returns>
public delegate HttpRequestParts BuildRequest<TParam>(TParam argument);

/// <summary>
/// Represents an asynchronous function that deserializes an HTTP response message into a typed object.
/// </summary>
/// <typeparam name="T">The type to deserialize the response to.</typeparam>
/// <param name="httpResponseMessage">The HTTP response message containing the content to deserialize.</param>
/// <param name="cancellationToken">A token to cancel the deserialization operation.</param>
/// <returns>A task that represents the asynchronous deserialization operation, containing the deserialized object of type <typeparamref name="T"/>.</returns>
public delegate Task<T> Deserialize<T>(
    HttpResponseMessage httpResponseMessage,
    CancellationToken cancellationToken
);

/// <summary>
/// Represents an asynchronous function that serializes a typed object into HTTP content.
/// </summary>
/// <typeparam name="T">The type of the object to serialize.</typeparam>
/// <param name="body">The object to serialize.</param>
/// <param name="cancellationToken">A token to cancel the serialization operation.</param>
/// <returns>A task that represents the asynchronous serialization operation, containing the serialized HTTP content.</returns>
public delegate Task<HttpContent> Serialize<T>(T body, CancellationToken cancellationToken);

/// <summary>
/// Represents a function that constructs a relative URL from a typed parameter.
/// </summary>
/// <typeparam name="TParam">The type of the parameter used to construct the URL.</typeparam>
/// <param name="argument">The parameter used to construct the relative URL.</param>
/// <returns>A relative URL constructed from the parameter.</returns>
public delegate RelativeUrl GetRelativeUrl<TParam>(TParam argument);

/// <summary>
/// Delegate for executing GET requests that return a Result with typed success and error responses.
/// </summary>
/// <typeparam name="TSuccess">The type of the success response body.</typeparam>
/// <typeparam name="TError">The type of the error response body.</typeparam>
/// <typeparam name="TParam">The type of the parameter used to construct the request URL.</typeparam>
/// <param name="httpClient">The HttpClient to use for the request.</param>
/// <param name="parameters">The parameters used to construct the request URL.</param>
/// <param name="cancellationToken">Cancellation token to cancel the request.</param>
/// <returns>A Result containing either the success response or an HttpError with the error response.</returns>
#pragma warning disable CA1005 // Avoid excessive parameters on generic types
public delegate Task<Result<TSuccess, HttpError<TError>>> GetAsync<TSuccess, TError, TParam>(
#pragma warning restore CA1005 // Avoid excessive parameters on generic types
    HttpClient httpClient,
    TParam parameters,
    CancellationToken cancellationToken = default
);

#pragma warning restore CA1005 // Avoid excessive parameters on generic types

/// <summary>
/// Delegate for executing POST requests that return a Result with typed success and error responses.
/// </summary>
/// <typeparam name="TSuccess">The type of the success response body.</typeparam>
/// <typeparam name="TError">The type of the error response body.</typeparam>
/// <typeparam name="TRequest">The type of the request body.</typeparam>
/// <param name="httpClient">The HttpClient to use for the request.</param>
/// <param name="requestBody">The request body content.</param>
/// <param name="cancellationToken">Cancellation token to cancel the request.</param>
/// <returns>A Result containing either the success response or an HttpError with the error response.</returns>
#pragma warning disable CA1005 // Avoid excessive parameters on generic types
public delegate Task<Result<TSuccess, HttpError<TError>>> PostAsync<TSuccess, TError, TRequest>(
    HttpClient httpClient,
    TRequest requestBody,
    CancellationToken cancellationToken = default
);
#pragma warning restore CA1005 // Avoid excessive parameters on generic types

/// <summary>
/// Delegate for executing PUT requests that return a Result with typed success and error responses.
/// </summary>
/// <typeparam name="TSuccess">The type of the success response body.</typeparam>
/// <typeparam name="TError">The type of the error response body.</typeparam>
/// <typeparam name="TRequest">The type of the request body.</typeparam>
/// <param name="httpClient">The HttpClient to use for the request.</param>
/// <param name="requestBody">The request body content.</param>
/// <param name="cancellationToken">Cancellation token to cancel the request.</param>
/// <returns>A Result containing either the success response or an HttpError with the error response.</returns>
#pragma warning disable CA1005 // Avoid excessive parameters on generic types
public delegate Task<Result<TSuccess, HttpError<TError>>> PutAsync<TSuccess, TError, TRequest>(
    HttpClient httpClient,
    TRequest requestBody,
    CancellationToken cancellationToken = default
);
#pragma warning restore CA1005 // Avoid excessive parameters on generic types

/// <summary>
/// Delegate for executing DELETE requests that return a Result with typed success and error responses.
/// </summary>
/// <typeparam name="TSuccess">The type of the success response body.</typeparam>
/// <typeparam name="TError">The type of the error response body.</typeparam>
/// <typeparam name="TParam">The type of the parameter used to construct the request URL.</typeparam>
/// <param name="httpClient">The HttpClient to use for the request.</param>
/// <param name="parameters">The parameters used to construct the request URL.</param>
/// <param name="cancellationToken">Cancellation token to cancel the request.</param>
/// <returns>A Result containing either the success response or an HttpError with the error response.</returns>
#pragma warning disable CA1005 // Avoid excessive parameters on generic types
public delegate Task<Result<TSuccess, HttpError<TError>>> DeleteAsync<TSuccess, TError, TParam>(
    HttpClient httpClient,
    TParam parameters,
    CancellationToken cancellationToken = default
);
#pragma warning restore CA1005 // Avoid excessive parameters on generic types

/// <summary>
/// Delegate for executing PATCH requests that return a Result with typed success and error responses.
/// </summary>
/// <typeparam name="TSuccess">The type of the success response body.</typeparam>
/// <typeparam name="TError">The type of the error response body.</typeparam>
/// <typeparam name="TRequest">The type of the request body.</typeparam>
/// <param name="httpClient">The HttpClient to use for the request.</param>
/// <param name="requestBody">The request body content.</param>
/// <param name="cancellationToken">Cancellation token to cancel the request.</param>
/// <returns>A Result containing either the success response or an HttpError with the error response.</returns>
#pragma warning disable CA1005 // Avoid excessive parameters on generic types
public delegate Task<Result<TSuccess, HttpError<TError>>> PatchAsync<TSuccess, TError, TRequest>(
    HttpClient httpClient,
    TRequest requestBody,
    CancellationToken cancellationToken = default
);
#pragma warning restore CA1005 // Avoid excessive parameters on generic types

/// <summary>
/// Delegate for executing HEAD requests that return a Result with typed success and error responses.
/// </summary>
/// <typeparam name="TSuccess">The type of the success response body.</typeparam>
/// <typeparam name="TError">The type of the error response body.</typeparam>
/// <typeparam name="TParam">The type of the parameter used to construct the request URL.</typeparam>
/// <param name="httpClient">The HttpClient to use for the request.</param>
/// <param name="parameters">The parameters used to construct the request URL.</param>
/// <param name="cancellationToken">Cancellation token to cancel the request.</param>
/// <returns>A Result containing either the success response or an HttpError with the error response.</returns>
#pragma warning disable CA1005 // Avoid excessive parameters on generic types
public delegate Task<Result<TSuccess, HttpError<TError>>> HeadAsync<TSuccess, TError, TParam>(
    HttpClient httpClient,
    TParam parameters,
    CancellationToken cancellationToken = default
);
#pragma warning restore CA1005 // Avoid excessive parameters on generic types

/// <summary>
/// Delegate for executing OPTIONS requests that return a Result with typed success and error responses.
/// </summary>
/// <typeparam name="TSuccess">The type of the success response body.</typeparam>
/// <typeparam name="TError">The type of the error response body.</typeparam>
/// <typeparam name="TParam">The type of the parameter used to construct the request URL.</typeparam>
/// <param name="httpClient">The HttpClient to use for the request.</param>
/// <param name="parameters">The parameters used to construct the request URL.</param>
/// <param name="cancellationToken">Cancellation token to cancel the request.</param>
/// <returns>A Result containing either the success response or an HttpError with the error response.</returns>
#pragma warning disable CA1005 // Avoid excessive parameters on generic types
public delegate Task<Result<TSuccess, HttpError<TError>>> OptionsAsync<TSuccess, TError, TParam>(
    HttpClient httpClient,
    TParam parameters,
    CancellationToken cancellationToken = default
);
#pragma warning restore CA1005 // Avoid excessive parameters on generic types
