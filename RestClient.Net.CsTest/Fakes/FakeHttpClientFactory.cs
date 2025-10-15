using System.Globalization;
using System.Net;
using System.Text;

namespace RestClient.Net.CsTest.Fakes;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
#pragma warning disable CA1515 // Consider making public types internal

public sealed class FakeHttpClientFactory(HttpClient httpClient) : IHttpClientFactory
{
    private readonly HttpClient _httpClient = httpClient;

    public HttpClient CreateClient(string name) => _httpClient;

    public static FakeHttpClientFactory CreateMockHttpClientFactory(
        HttpResponseMessage? response = null,
        Exception? exceptionToThrow = null,
        string? chunkedData = null,
        Func<
            HttpRequestMessage,
            MultipartFormDataContent,
            CancellationToken,
            Task
        >? onMultipartUploadAvailable = null,
        Func<HttpRequestMessage, Stream, CancellationToken, Task>? onUploadStreamAvailable = null,
        TimeSpan? simulatedDelay = null,
        Action<HttpRequestMessage>? onRequestSent = null
    )
    {
#pragma warning disable CA2000 // Dispose objects before losing scope - HttpClient lifecycle managed by factory
        HttpMessageHandler messageHandler =
            chunkedData == null
                ? CreateFakeHttpMessageHandler(
                    response: response,
                    exceptionToThrow: exceptionToThrow,
                    onMultipartUploadAvailable: onMultipartUploadAvailable,
                    onUploadStreamAvailable: onUploadStreamAvailable,
                    simulatedDelay: simulatedDelay,
                    onRequestSent: onRequestSent
                )
                : new FakeHttpMessageHandlerStreaming(
                    chunkedData,
                    (a) => Task.FromResult(response!),
                    onRequestSent: onRequestSent
                );

        var httpClient = new HttpClient(messageHandler);
        return new FakeHttpClientFactory(httpClient);
#pragma warning restore CA2000 // Dispose objects before losing scope
    }

    private static FakeHttpMessageHandler CreateFakeHttpMessageHandler(
        HttpResponseMessage? response = null,
        Exception? exceptionToThrow = null,
        Func<
            HttpRequestMessage,
            MultipartFormDataContent,
            CancellationToken,
            Task
        >? onMultipartUploadAvailable = null,
        Func<HttpRequestMessage, Stream, CancellationToken, Task>? onUploadStreamAvailable = null,
        TimeSpan? simulatedDelay = null,
        Action<HttpRequestMessage>? onRequestSent = null
    ) =>
        new(
            async request =>
            {
                if (simulatedDelay.HasValue)
                {
                    await Task.Delay(simulatedDelay.Value).ConfigureAwait(false);
                }

                onRequestSent?.Invoke(request);

                return response
                    ?? (
                        request.Method.ToString().ToUpper(CultureInfo.InvariantCulture),
                        request.RequestUri?.AbsoluteUri
                    ) switch
                    {
                        ("GET", var uri)
                            when uri?.Contains("posts", StringComparison.Ordinal) == true =>
                            new HttpResponseMessage(HttpStatusCode.OK)
                            {
                                Content = new StringContent( /*lang=json,strict*/
                                    "{\n  \"userId\": 1,\n  \"id\": 1,\n  \"title\": \"sunt aut facere repellat provident occaecati excepturi optio reprehenderit\",\n  \"body\": \"quia et suscipit\\nsuscipit recusandae consequuntur expedita et cum\\nreprehenderit molestiae ut ut quas totam\\nnostrum rerum est autem sunt rem eveniet architecto\"\n}",
                                    Encoding.UTF8,
                                    "application/json"
                                ),
                            },

                        ("POST", var uri)
                            when uri?.Contains("posts", StringComparison.Ordinal) == true =>
                            new HttpResponseMessage(HttpStatusCode.Created)
                            {
                                Content = new StringContent(
                                    /*lang=json,strict*/
                                    "{\"id\": 101}",
                                    Encoding.UTF8,
                                    "application/json"
                                ),
                            },

                        ("PUT", var uri)
                            when uri?.Contains("posts/1", StringComparison.Ordinal) == true =>
                            new HttpResponseMessage(HttpStatusCode.OK)
                            {
                                Content = new StringContent(
                                    /*lang=json,strict*/
                                    "{\"id\": 1}",
                                    Encoding.UTF8,
                                    "application/json"
                                ),
                            },

                        ("DELETE", var uri)
                            when uri?.Contains("posts/1", StringComparison.Ordinal) == true =>
                            new HttpResponseMessage(HttpStatusCode.OK)
                            {
                                Content = new StringContent("{}"),
                            },

                        _ => new HttpResponseMessage(HttpStatusCode.NotFound),
                    };
            },
            exception: exceptionToThrow,
            onMultipartUploadAvailable: onMultipartUploadAvailable,
            onUploadStreamAvailable: onUploadStreamAvailable
        );

    public static IHttpClientFactory CreateHttpClientFactory(
        bool useRealApi,
        HttpResponseMessage? response = null,
        Exception? exception = null,
        string? chunkedData = null,
        TimeSpan? simulatedDelay = null
    ) =>
        useRealApi
            ? new SimpleFakeHttpClientFactory()
            : CreateMockHttpClientFactory(
                response: response,
                exceptionToThrow: exception,
                chunkedData: chunkedData,
                simulatedDelay: simulatedDelay
            );
}
