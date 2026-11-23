using System.Text;
using System.Text.Json;
using Outcome;
using RestClient.Net.Utilities;
using Urls;
using ExceptionErrorString = Outcome.HttpError<string>.ExceptionError;
using ResponseErrorString = Outcome.HttpError<string>.ErrorResponseError;

#pragma warning disable CS8509 // The switch expression does not handle all possible values of its input type (it is not exhaustive).

namespace RestClient.Net;

/// <summary>
/// Implements IClient from RestClient.Net 6, providing HTTP client functionality.
/// </summary>
public class Client : IClient
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly bool _throwExceptionOnFailure;

    /// <summary>
    /// Initializes a new instance of the <see cref="Client"/> class.
    /// </summary>
    /// <param name="httpClientFactory">The HTTP client factory to use for creating HTTP clients.</param>
    /// <param name="baseUrl">The base URL for all requests made by this client.</param>
    /// <param name="defaultRequestHeaders">Default headers to be sent with each request.</param>
    /// <param name="throwExceptionOnFailure">Whether to throw an exception on HTTP failures.</param>
    public Client(
        IHttpClientFactory httpClientFactory,
        AbsoluteUrl baseUrl,
        IHeadersCollection defaultRequestHeaders,
        bool throwExceptionOnFailure = true
    ) =>
        (_httpClientFactory, BaseUrl, DefaultRequestHeaders, _throwExceptionOnFailure) = (
            httpClientFactory,
            baseUrl,
            defaultRequestHeaders,
            throwExceptionOnFailure
        );

    /// <summary>
    /// Gets the default request headers for this client.
    /// </summary>
    public IHeadersCollection DefaultRequestHeaders { get; }

    /// <summary>
    /// Gets the base URL for this client.
    /// </summary>
    public AbsoluteUrl BaseUrl { get; }

    /// <summary>
    /// Sends an HTTP request asynchronously.
    /// </summary>
    /// <typeparam name="TResponseBody">The type of the response body.</typeparam>
    /// <typeparam name="TRequestBody">The type of the request body.</typeparam>
    /// <param name="request">The request to send.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the response.</returns>
    public async Task<Response<TResponseBody>> SendAsync<TResponseBody, TRequestBody>(
        IRequest<TRequestBody> request
    )
    {
        using var requestBody =
            request.BodyData != null
                ? new ProgressReportingHttpContent(
                    Encoding.UTF8.GetBytes(JsonSerializer.Serialize(request.BodyData))
                )
                : null;

        var result = await _httpClientFactory
            .SendAsync(
                clientName: "RestClientNet",
                url: request.Uri,
                httpMethod: new HttpMethod(request.HttpRequestMethod.ToString()),
                deserializeSuccess: DeserializeSuccessResponse<TResponseBody>(request),
                deserializeError: DeserializeErrorResponse,
                requestBody: requestBody,
                headers: GetHeaders(request),
                cancellationToken: request.CancellationToken
            )
            .ConfigureAwait(false);

        return result switch
        {
            Result<Response<TResponseBody>, HttpError<string>>.Ok<
                Response<TResponseBody>,
                HttpError<string>
            > { Value: var response } => response,
            Result<Response<TResponseBody>, HttpError<string>>.Error<
                Response<TResponseBody>,
                HttpError<string>
            >(ExceptionErrorString) failure => HandleFailure(failure, request),
            Result<Response<TResponseBody>, HttpError<string>>.Error<
                Response<TResponseBody>,
                HttpError<string>
            >(ResponseErrorString) failure => HandleFailure(failure, request),
        };
    }

    private Dictionary<string, string> GetHeaders<TRequestBody>(IRequest<TRequestBody> request) =>
        DefaultRequestHeaders
            .Concat(request.Headers)
            .GroupBy(h => h.Key)
            .ToDictionary(g => g.Key, g => string.Join(", ", g.SelectMany(h => h.Value)));

    private static Deserialize<Response<TResponseBody>> DeserializeSuccessResponse<TResponseBody>(
        IRequest request
    ) =>
        async (response, cancellationToken) =>
        {
            // TODO: try/catch
            // We should return a reasonable error that explains that
            // reading or serializing failed
            using var reader = new StreamReader(
                await response.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false)
            );
            var content = await reader.ReadToEndAsync(cancellationToken).ConfigureAwait(false);
            var body =
                typeof(TResponseBody) == typeof(string)
                    ? (TResponseBody)(object)content
                    : JsonSerializer.Deserialize<TResponseBody>(content);
            return new Response<TResponseBody>(
                httpRequestMethod: request.HttpRequestMethod,
                headersCollection: new HeadersCollection(
                    response.Headers.ToDictionary(h => h.Key, h => h.Value)
                ),
                responseData: Encoding.UTF8.GetBytes(content),
                body: body,
                statusCode: (int)response.StatusCode,
                requestUri: request.Uri
            );
        };

    private static Deserialize<string> DeserializeErrorResponse =>
        async (response, cancellationToken) =>
        {
            using var reader = new StreamReader(
                await response.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false)
            );
            return await reader.ReadToEndAsync(cancellationToken).ConfigureAwait(false)!;
        };

    private Response<TResponseBody> HandleFailure<TResponseBody>(
        Result<Response<TResponseBody>, HttpError<string>>.Error<
            Response<TResponseBody>,
            HttpError<string>
        > failure,
        IRequest request
    ) =>
        _throwExceptionOnFailure
            // TODO: throw better exception
            ? HandleHttpException(failure, request)
            : HandleHttpError(failure, request);

    private static Response<TResponseBody> HandleHttpException<TResponseBody>(
        Result<Response<TResponseBody>, HttpError<string>>.Error<
            Response<TResponseBody>,
            HttpError<string>
        > failure,
        IRequest request
    ) =>
        failure.Value switch
        {
            ExceptionErrorString ee => throw ee.Exception,
            // Shouldn't be throwing exceptions at all, but this is a legacy thing...
            ResponseErrorString er => throw new HttpStatusException(
                er.StatusCode.ToString(),
                CreateErrorResponse<TResponseBody>(er, request)
            ),
        };

    private static Response<TResponseBody> HandleHttpError<TResponseBody>(
        Result<Response<TResponseBody>, HttpError<string>>.Error<
            Response<TResponseBody>,
            HttpError<string>
        > failure,
        IRequest request
    ) =>
        failure.Value switch
        {
            ExceptionErrorString => CreateExceptionErrorResponse<TResponseBody>(request),
            ResponseErrorString er => CreateErrorResponse<TResponseBody>(er, request),
        };

    private static Response<TResponseBody> CreateExceptionErrorResponse<TResponseBody>(
        IRequest request
    ) =>
        new(
            httpRequestMethod: request.HttpRequestMethod,
            headersCollection: HeadersCollection.Empty,
            responseData: Encoding.UTF8.GetBytes(string.Empty),
            body: default,
            statusCode: 500,
            requestUri: request.Uri
        );

    private static Response<TResponseBody> CreateErrorResponse<TResponseBody>(
        ResponseErrorString er,
        IRequest request
    ) =>
        new(
            httpRequestMethod: request.HttpRequestMethod,
            headersCollection: new HeadersCollection(
                er.Headers.ToDictionary(h => h.Key, h => h.Value)
            ),
            responseData: Encoding.UTF8.GetBytes(er.Body),
            body: default,
            statusCode: (int)er.StatusCode,
            requestUri: request.Uri
        );
}
