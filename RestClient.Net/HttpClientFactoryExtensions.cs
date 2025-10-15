using Outcome;
using RestClient.Net.Utilities;
using Urls;

namespace RestClient.Net;

/// <summary>
/// Provides extension methods for IHttpClientFactory to simplify HTTP operations.
/// </summary>
public static class HttpClientFactoryExtensions
{
    /// <summary>
    /// Sends an HTTP request asynchronously using the specified HTTP operation.
    /// </summary>
    /// <typeparam name="TSuccess">The type of the successful response.</typeparam>
    /// <typeparam name="TError">The type of the error response.</typeparam>
    /// <param name="httpClientFactory">The HTTP client factory.</param>
    /// <param name="clientName">The name of the client to create.</param>
    /// <param name="url">The URL to send the request to.</param>
    /// <param name="httpMethod">The HTTP method to use for the request.</param>
    /// <param name="deserializeSuccess">Function to deserialize a successful response.</param>
    /// <param name="deserializeError">Function to deserialize an error response.</param>
    /// <param name="requestBody">The request body stream (optional).</param>
    /// <param name="headers">The headers to include in the request (optional).</param>
    /// <param name="httpOperation">An optional action to configure the HttpClient before sending the request.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A Result containing either the successful response or an HTTP error.</returns>
    public static async Task<Result<TSuccess, HttpError<TError>>> SendAsync<TSuccess, TError>(
        this IHttpClientFactory httpClientFactory,
        string clientName,
        AbsoluteUrl url,
        HttpMethod httpMethod,
        Deserialize<TSuccess> deserializeSuccess,
        Deserialize<TError> deserializeError,
        HttpContent? requestBody = null,
        IReadOnlyDictionary<string, string>? headers = null,
        HttpAction? httpOperation = null,
        CancellationToken cancellationToken = default
    ) =>
        await httpClientFactory
            .CreateClient(clientName)
            .SendAsync(
                url: url,
                httpMethod: httpMethod,
                deserializeSuccess: deserializeSuccess,
                deserializeError: deserializeError,
                requestBody: requestBody,
                headers: headers,
                httpOperation: httpOperation,
                cancellationToken: cancellationToken
            )
            .ConfigureAwait(false);

    /// <summary>
    /// Performs a GET request.
    /// </summary>
    /// <typeparam name="TSuccess">The type representing a successful response.</typeparam>
    /// <typeparam name="TError">The type representing an error response.</typeparam>
    /// <param name="httpClientFactory">The HTTP client factory instance.</param>
    /// <param name="clientName">The name of the client to create.</param>
    /// <param name="url">The URL to send the request to.</param>
    /// <param name="deserializeSuccess">Function to deserialize a successful response.</param>
    /// <param name="deserializeError">Function to deserialize an error response.</param>
    /// <param name="headers">The headers to include in the request (optional).</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A Result containing either the successful response or an HTTP error.</returns>
    public static Task<Result<TSuccess, HttpError<TError>>> GetAsync<TSuccess, TError>(
        this IHttpClientFactory httpClientFactory,
        string clientName,
        AbsoluteUrl url,
        Deserialize<TSuccess> deserializeSuccess,
        Deserialize<TError> deserializeError,
        IReadOnlyDictionary<string, string>? headers = null,
        CancellationToken cancellationToken = default
    ) =>
        httpClientFactory
            .CreateClient(clientName)
            .GetAsync(url, deserializeSuccess, deserializeError, headers, cancellationToken);

    /// <summary>
    /// Performs a POST request.
    /// </summary>
    /// <typeparam name="TSuccess">The type representing a successful response.</typeparam>
    /// <typeparam name="TError">The type representing an error response.</typeparam>
    /// <param name="httpClientFactory">The HTTP client factory instance.</param>
    /// <param name="clientName">The name of the client to create.</param>
    /// <param name="url">The URL to send the request to.</param>
    /// <param name="requestBody">The content to send in the request body.</param>
    /// <param name="deserializeSuccess">Function to deserialize a successful response.</param>
    /// <param name="deserializeError">Function to deserialize an error response.</param>
    /// <param name="headers">The headers to include in the request (optional).</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A Result containing either the successful response or an HTTP error.</returns>
    public static Task<Result<TSuccess, HttpError<TError>>> PostAsync<TSuccess, TError>(
        this IHttpClientFactory httpClientFactory,
        string clientName,
        AbsoluteUrl url,
        HttpContent? requestBody,
        Deserialize<TSuccess> deserializeSuccess,
        Deserialize<TError> deserializeError,
        IReadOnlyDictionary<string, string>? headers = null,
        CancellationToken cancellationToken = default
    ) =>
        httpClientFactory
            .CreateClient(clientName)
            .PostAsync(
                url,
                requestBody,
                deserializeSuccess,
                deserializeError,
                headers,
                cancellationToken
            );

    /// <summary>
    /// Performs a PUT request.
    /// </summary>
    /// <typeparam name="TSuccess">The type representing a successful response.</typeparam>
    /// <typeparam name="TError">The type representing an error response.</typeparam>
    /// <param name="httpClientFactory">The HTTP client factory instance.</param>
    /// <param name="clientName">The name of the client to create.</param>
    /// <param name="url">The URL to send the request to.</param>
    /// <param name="requestBody">The content to send in the request body.</param>
    /// <param name="deserializeSuccess">Function to deserialize a successful response.</param>
    /// <param name="deserializeError">Function to deserialize an error response.</param>
    /// <param name="headers">The headers to include in the request (optional).</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A Result containing either the successful response or an HTTP error.</returns>
    public static Task<Result<TSuccess, HttpError<TError>>> PutAsync<TSuccess, TError>(
        this IHttpClientFactory httpClientFactory,
        string clientName,
        AbsoluteUrl url,
        HttpContent? requestBody,
        Deserialize<TSuccess> deserializeSuccess,
        Deserialize<TError> deserializeError,
        IReadOnlyDictionary<string, string>? headers = null,
        CancellationToken cancellationToken = default
    ) =>
        httpClientFactory
            .CreateClient(clientName)
            .PutAsync(
                url,
                requestBody,
                deserializeSuccess,
                deserializeError,
                headers,
                cancellationToken
            );

    /// <summary>
    /// Performs a DELETE request.
    /// </summary>
    /// <typeparam name="TSuccess">The type representing a successful response.</typeparam>
    /// <typeparam name="TError">The type representing an error response.</typeparam>
    /// <param name="httpClientFactory">The HTTP client factory instance.</param>
    /// <param name="clientName">The name of the client to create.</param>
    /// <param name="url">The URL to send the request to.</param>
    /// <param name="deserializeSuccess">Function to deserialize a successful response.</param>
    /// <param name="deserializeError">Function to deserialize an error response.</param>
    /// <param name="headers">The headers to include in the request (optional).</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A Result containing either the successful response or an HTTP error.</returns>
    public static Task<Result<TSuccess, HttpError<TError>>> DeleteAsync<TSuccess, TError>(
        this IHttpClientFactory httpClientFactory,
        string clientName,
        AbsoluteUrl url,
        Deserialize<TSuccess> deserializeSuccess,
        Deserialize<TError> deserializeError,
        IReadOnlyDictionary<string, string>? headers = null,
        CancellationToken cancellationToken = default
    ) =>
        httpClientFactory
            .CreateClient(clientName)
            .DeleteAsync(url, deserializeSuccess, deserializeError, headers, cancellationToken);

    /// <summary>
    /// Downloads a file from the specified URL to a stream.
    /// </summary>
    /// <typeparam name="TError">The type representing an error response.</typeparam>
    /// <param name="httpClientFactory">The HTTP client factory instance.</param>
    /// <param name="clientName">The name of the client to create.</param>
    /// <param name="url">The URL to download the file from.</param>
    /// <param name="destinationStream">The stream to write the downloaded file to.</param>
    /// <param name="deserializeError">Function to deserialize an error response.</param>
    /// <param name="headers">The headers to include in the request (optional).</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A Result containing either a Unit value on success or an HTTP error.</returns>
    public static Task<Result<Unit, HttpError<TError>>> DownloadFileAsync<TError>(
        this IHttpClientFactory httpClientFactory,
        string clientName,
        AbsoluteUrl url,
        Stream destinationStream,
        Deserialize<TError> deserializeError,
        IReadOnlyDictionary<string, string>? headers = null,
        CancellationToken cancellationToken = default
    ) =>
        httpClientFactory
            .CreateClient(clientName)
            .DownloadFileAsync(
                url,
                destinationStream,
                deserializeError,
                headers,
                cancellationToken
            );

    /// <summary>
    /// Uploads a file to the specified URL.
    /// </summary>
    /// <typeparam name="TSuccess">The type representing a successful response.</typeparam>
    /// <typeparam name="TError">The type representing an error response.</typeparam>
    /// <param name="httpClientFactory">The HTTP client factory instance.</param>
    /// <param name="clientName">The name of the client to create.</param>
    /// <param name="url">The URL to upload the file to.</param>
    /// <param name="fileStream">The file content to upload.</param>
    /// <param name="deserializeSuccess">Function to deserialize a successful response.</param>
    /// <param name="deserializeError">Function to deserialize an error response.</param>
    /// <param name="headers">The headers to include in the request (optional).</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A Result containing either the successful response or an HTTP error.</returns>
    public static Task<Result<TSuccess, HttpError<TError>>> UploadFileAsync<TSuccess, TError>(
        this IHttpClientFactory httpClientFactory,
        string clientName,
        AbsoluteUrl url,
        ProgressReportingHttpContent fileStream,
        Deserialize<TSuccess> deserializeSuccess,
        Deserialize<TError> deserializeError,
        IReadOnlyDictionary<string, string>? headers = null,
        CancellationToken cancellationToken = default
    ) =>
        httpClientFactory
            .CreateClient(clientName)
            .UploadFileAsync(
                url,
                fileStream,
                deserializeSuccess,
                deserializeError,
                headers,
                cancellationToken
            );

    /// <summary>
    /// Performs a PATCH request.
    /// </summary>
    /// <typeparam name="TSuccess">The type representing a successful response.</typeparam>
    /// <typeparam name="TError">The type representing an error response.</typeparam>
    /// <param name="httpClientFactory">The HTTP client factory instance.</param>
    /// <param name="clientName">The name of the client to create.</param>
    /// <param name="url">The URL to send the request to.</param>
    /// <param name="requestBody">The content to send in the request body.</param>
    /// <param name="deserializeSuccess">Function to deserialize a successful response.</param>
    /// <param name="deserializeError">Function to deserialize an error response.</param>
    /// <param name="headers">The headers to include in the request (optional).</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A Result containing either the successful response or an HTTP error.</returns>
    public static Task<Result<TSuccess, HttpError<TError>>> PatchAsync<TSuccess, TError>(
        this IHttpClientFactory httpClientFactory,
        string clientName,
        AbsoluteUrl url,
        HttpContent requestBody,
        Deserialize<TSuccess> deserializeSuccess,
        Deserialize<TError> deserializeError,
        IReadOnlyDictionary<string, string>? headers = null,
        CancellationToken cancellationToken = default
    ) =>
        httpClientFactory
            .CreateClient(clientName)
            .PatchAsync(
                url,
                requestBody,
                deserializeSuccess,
                deserializeError,
                headers,
                cancellationToken
            );

    /// <summary>
    /// Creates a GET request delegate for the specified URL.
    /// </summary>
    /// <typeparam name="TSuccess">The type to deserialize successful responses to.</typeparam>
    /// <typeparam name="TError">The type to deserialize error responses to.</typeparam>
    /// <typeparam name="TParam">The type of parameter used to construct the request.</typeparam>
    /// <param name="url">The absolute URL for the GET request.</param>
    /// <param name="buildRequest">Function to build the request parts from parameters.</param>
    /// <param name="deserializeSuccess">Function to deserialize successful HTTP responses.</param>
    /// <param name="deserializeError">Function to deserialize error HTTP responses.</param>
    /// <returns>A delegate that can execute the GET request with the specified parameters.</returns>
    public static GetAsync<TSuccess, TError, TParam> CreateGet<TSuccess, TError, TParam>(
        AbsoluteUrl url,
        BuildRequest<TParam> buildRequest,
        Deserialize<TSuccess> deserializeSuccess,
        Deserialize<TError> deserializeError
    ) =>
        async (httpClient, parameters, ct) =>
        {
            var requestParts = buildRequest(parameters);
            return await httpClient
                .GetAsync(
                    url: url.WithRelativeUrl(requestParts.RelativeUrl),
                    deserializeSuccess: deserializeSuccess,
                    deserializeError: deserializeError,
                    headers: requestParts.Headers,
                    cancellationToken: ct
                )
                .ConfigureAwait(false);
        };

    /// <summary>
    /// Creates a POST request delegate for the specified URL.
    /// </summary>
    /// <typeparam name="TSuccess">The type to deserialize successful responses to.</typeparam>
    /// <typeparam name="TError">The type to deserialize error responses to.</typeparam>
    /// <typeparam name="TParam">The type of parameter used to construct the request.</typeparam>
    /// <param name="url">The absolute URL for the POST request.</param>
    /// <param name="buildRequest">Function to build the request parts from parameters.</param>
    /// <param name="deserializeSuccess">Function to deserialize successful HTTP responses.</param>
    /// <param name="deserializeError">Function to deserialize error HTTP responses.</param>
    /// <returns>A delegate that can execute the POST request with the specified parameters.</returns>
    public static PostAsync<TSuccess, TError, TParam> CreatePost<TSuccess, TError, TParam>(
        AbsoluteUrl url,
        BuildRequest<TParam> buildRequest,
        Deserialize<TSuccess> deserializeSuccess,
        Deserialize<TError> deserializeError
    ) =>
        async (httpClient, request, ct) =>
        {
            var requestParts = buildRequest(request);
            return await httpClient
                .PostAsync(
                    url: url.WithRelativeUrl(requestParts.RelativeUrl),
                    requestBody: requestParts.Body,
                    deserializeSuccess: deserializeSuccess,
                    deserializeError: deserializeError,
                    headers: requestParts.Headers,
                    cancellationToken: ct
                )
                .ConfigureAwait(false);
        };

    /// <summary>
    /// Creates a PUT request delegate for the specified URL.
    /// </summary>
    /// <typeparam name="TSuccess">The type to deserialize successful responses to.</typeparam>
    /// <typeparam name="TError">The type to deserialize error responses to.</typeparam>
    /// <typeparam name="TParam">The type of parameter used to construct the request.</typeparam>
    /// <param name="url">The absolute URL for the PUT request.</param>
    /// <param name="buildRequest">Function to build the request parts from parameters.</param>
    /// <param name="deserializeSuccess">Function to deserialize successful HTTP responses.</param>
    /// <param name="deserializeError">Function to deserialize error HTTP responses.</param>
    /// <returns>A delegate that can execute the PUT request with the specified parameters.</returns>
    public static PutAsync<TSuccess, TError, TParam> CreatePut<TSuccess, TError, TParam>(
        AbsoluteUrl url,
        BuildRequest<TParam> buildRequest,
        Deserialize<TSuccess> deserializeSuccess,
        Deserialize<TError> deserializeError
    ) =>
        async (httpClient, request, ct) =>
        {
            var requestParts = buildRequest(request);
            return await httpClient
                .PutAsync(
                    url: url.WithRelativeUrl(requestParts.RelativeUrl),
                    requestBody: requestParts.Body,
                    deserializeSuccess: deserializeSuccess,
                    deserializeError: deserializeError,
                    headers: requestParts.Headers,
                    cancellationToken: ct
                )
                .ConfigureAwait(false);
        };

    /// <summary>
    /// Creates a DELETE request delegate for the specified URL.
    /// </summary>
    /// <typeparam name="TSuccess">The type to deserialize successful responses to.</typeparam>
    /// <typeparam name="TError">The type to deserialize error responses to.</typeparam>
    /// <typeparam name="TParam">The type of parameter used to construct the request.</typeparam>
    /// <param name="url">The absolute URL for the DELETE request.</param>
    /// <param name="buildRequest">Function to build the request parts from parameters.</param>
    /// <param name="deserializeSuccess">Function to deserialize successful HTTP responses.</param>
    /// <param name="deserializeError">Function to deserialize error HTTP responses.</param>
    /// <returns>A delegate that can execute the DELETE request with the specified parameters.</returns>
    public static DeleteAsync<TSuccess, TError, TParam> CreateDelete<TSuccess, TError, TParam>(
        AbsoluteUrl url,
        BuildRequest<TParam> buildRequest,
        Deserialize<TSuccess> deserializeSuccess,
        Deserialize<TError> deserializeError
    ) =>
        async (httpClient, parameters, ct) =>
        {
            var requestParts = buildRequest(parameters);
            return await httpClient
                .DeleteAsync(
                    url: url.WithRelativeUrl(requestParts.RelativeUrl),
                    deserializeSuccess: deserializeSuccess,
                    deserializeError: deserializeError,
                    headers: requestParts.Headers,
                    cancellationToken: ct
                )
                .ConfigureAwait(false);
        };

    /// <summary>
    /// Creates a PATCH request delegate for the specified URL.
    /// </summary>
    /// <typeparam name="TSuccess">The type to deserialize successful responses to.</typeparam>
    /// <typeparam name="TError">The type to deserialize error responses to.</typeparam>
    /// <typeparam name="TParam">The type of parameter used to construct the request.</typeparam>
    /// <param name="url">The absolute URL for the PATCH request.</param>
    /// <param name="buildRequest">Function to build the request parts from parameters.</param>
    /// <param name="deserializeSuccess">Function to deserialize successful HTTP responses.</param>
    /// <param name="deserializeError">Function to deserialize error HTTP responses.</param>
    /// <returns>A delegate that can execute the PATCH request with the specified parameters.</returns>
    public static PatchAsync<TSuccess, TError, TParam> CreatePatch<TSuccess, TError, TParam>(
        AbsoluteUrl url,
        BuildRequest<TParam> buildRequest,
        Deserialize<TSuccess> deserializeSuccess,
        Deserialize<TError> deserializeError
    ) =>
        async (httpClient, request, ct) =>
        {
            var requestParts = buildRequest(request);
            return await httpClient
                .PatchAsync(
                    url: url.WithRelativeUrl(requestParts.RelativeUrl),
                    requestBody: requestParts.Body,
                    deserializeSuccess: deserializeSuccess,
                    deserializeError: deserializeError,
                    headers: requestParts.Headers,
                    cancellationToken: ct
                )
                .ConfigureAwait(false);
        };
}
