namespace RestClient.Net;

/// <summary>
/// Represents an asynchronous HTTP operation that sends a request and returns a response.
/// This delegate encapsulates the core functionality of executing HTTP requests with support
/// for optional request content, headers, and cancellation.
/// </summary>
/// <param name="httpClient">The HTTP client to use for sending the request.</param>
/// <param name="requestMessage">The request message to send.</param>
/// <param name="cancellationToken">Cancellation token to cancel the operation.</param>
/// <returns>A task that represents the asynchronous operation, containing the HTTP response message.</returns>
public delegate Task<HttpResponseMessage> HttpAction(
    HttpClient httpClient,
    HttpRequestMessage requestMessage,
    CancellationToken cancellationToken
);
