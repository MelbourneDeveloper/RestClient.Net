using System.Net;
using System.Text;
using System.Text.Json;
using RestClient.Net.CsTest.Models;
using Urls;
using static RestClient.Net.CsTest.Fakes.FakeHttpClientFactory;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
#pragma warning disable CA1812 // Missing XML comment for publicly visible type or member

namespace RestClient.Net.CsTest;

/// <summary>
/// These tests test that the original IClient interface methods continue to work as expected.
/// </summary>
[TestClass]
public sealed class ClientTests
{
    [TestMethod]
    [DataRow(true)]
    [DataRow(false)]
    public async Task PostAsync_ReturnsSuccessResult(bool useRealApi)
    {
        var client = new Client(
            CreateHttpClientFactory(useRealApi),
            new AbsoluteUrl("https://jsonplaceholder.typicode.com"),
            HeadersCollection.Empty
        );

        var requestBody = new UserPost("New Post", "This is the body of the new post", 1);
        var result = await client
            .PostAsync<PostResponse, UserPost>(requestBody, "posts")
            .ConfigureAwait(false);

        Assert.IsNotNull(result, "Result should not be null");
        Assert.IsTrue(result.IsSuccess, "Result should be success");
        Assert.AreEqual(101, result!.Body!.id, "Response should contain expected id");

        AssertResponse(result, 201, "POST", "posts", new PostResponse(101));
        Assert.AreEqual(
            requestBody,
            new UserPost("New Post", "This is the body of the new post", 1)
        );
        AssertClientDefaults(client);
    }

    [TestMethod]
    [DataRow(true)]
    [DataRow(false)]
    public async Task PutAsync_ReturnsSuccessResult(bool useRealApi)
    {
        var client = new Client(
            CreateHttpClientFactory(useRealApi),
            new AbsoluteUrl("https://jsonplaceholder.typicode.com"),
            HeadersCollection.Empty
        );

        var requestBody = new UserPost("Updated Title", "Updated body", 1);
        var result = await client
            .PutAsync<PostResponse, UserPost>(requestBody, "posts/1")
            .ConfigureAwait(false);

        Assert.IsNotNull(result, "Result should not be null");
        Assert.IsTrue(result.IsSuccess, "Result should be success");
        Assert.AreEqual(1, result.Body!.id, "Response should contain expected id");

        AssertResponse(result, 200, "PUT", "posts/1", new PostResponse(1));
        Assert.AreEqual(requestBody, new UserPost("Updated Title", "Updated body", 1));
        AssertClientDefaults(client);
    }

    [TestMethod]
    [DataRow(true)]
    [DataRow(false)]
    public async Task DeleteAsync_ReturnsSuccessResult(bool useRealApi)
    {
        var client = new Client(
            CreateHttpClientFactory(useRealApi),
            new AbsoluteUrl("https://jsonplaceholder.typicode.com"),
            HeadersCollection.Empty,
            throwExceptionOnFailure: false
        );

        var result = await client.DeleteAsync("posts/1").ConfigureAwait(false);

        Assert.IsNotNull(result, "Result should not be null");
        Assert.IsTrue(result.IsSuccess, "Result should be success");

        Assert.AreEqual(200, result.StatusCode);
        Assert.AreEqual("DELETE", result.HttpRequestMethod.ToString().ToUpperInvariant());
        Assert.AreEqual(
            new AbsoluteUrl("https://jsonplaceholder.typicode.com/posts/1"),
            result.RequestUri
        );
        AssertClientDefaults(client);
    }

    [TestMethod]
    public async Task TestGet()
    {
        var client = new Client(
            new SimpleFakeHttpClientFactory(),
            new AbsoluteUrl("https://jsonplaceholder.typicode.com"),
            HeadersCollection.Empty
        );
        var result = await client.GetAsync<string>("posts/1").ConfigureAwait(false);

        Assert.IsNotNull(result, "Result should not be null");
        Assert.IsTrue(result.IsSuccess, "Result should be success");
        Assert.IsNotNull(result.Body, "Success value should not be null");
        Assert.IsTrue(
            result.Body.Contains("\"userId\": 1", StringComparison.Ordinal),
            "Response should contain expected content"
        );

        AssertResponse(result, 200, "GET", "posts/1", result.Body);
        using var parsed = JsonDocument.Parse(result.Body);
        Assert.AreEqual(1, parsed.RootElement.GetProperty("id").GetInt32());
        Assert.AreEqual(1, parsed.RootElement.GetProperty("userId").GetInt32());
        Assert.AreEqual(
            "sunt aut facere repellat provident occaecati excepturi optio reprehenderit",
            parsed.RootElement.GetProperty("title").GetString()
        );
        Assert.IsTrue(parsed.RootElement.GetProperty("body").GetString()!.Length > 0);
        AssertClientDefaults(client);
    }

    [TestMethod]
    public async Task PostAsync_ReturnsErrorResult()
    {
        // Arrange
        using var response = new HttpResponseMessage(statusCode: HttpStatusCode.BadRequest);
        response.Content = new StringContent( /*lang=json,strict*/
            "{\"message\":\"Post Error\"}",
            Encoding.UTF8,
            "application/json"
        );
        var client = new Client(
            CreateHttpClientFactory(false, response),
            new AbsoluteUrl("https://jsonplaceholder.typicode.com"),
            HeadersCollection.Empty
        );

        // TODO: Assert that this throws the same exception as the original RestClient.Net
        // API interface

        // Act
        var exception = await Assert
            .ThrowsExceptionAsync<HttpStatusException>(
                async () =>
                    await client
                        .PostAsync<PostResponse, UserPost>(
                            new UserPost("New Post", "This is the body of the new post", 1),
                            "posts"
                        )
                        .ConfigureAwait(false)
            )
            .ConfigureAwait(false);

        Assert.AreEqual("BadRequest", exception.Message);
        Assert.IsNotNull(exception.Response);
        Assert.AreEqual(400, exception.Response.StatusCode);
        Assert.IsFalse(exception.Response.IsSuccess);
        Assert.AreEqual(
            new AbsoluteUrl("https://jsonplaceholder.typicode.com/posts"),
            exception.Response.RequestUri
        );
        AssertClientDefaults(client);
    }

    [TestMethod]
    public async Task PostAsync_ExceptionThrown_ReturnsFailureResult()
    {
        // Arrange
        var exceptionMessage = "Network failure";
        var client = new Client(
            CreateHttpClientFactory(false, exception: new Exception(exceptionMessage)),
            new AbsoluteUrl("https://jsonplaceholder.typicode.com"),
            HeadersCollection.Empty,
            throwExceptionOnFailure: false
        );

        var requestBody = new UserPost("New Post", "This is the body of the new post", 1);

        // Act
        var result = await client
            .PostAsync<PostResponse, UserPost>(requestBody, "posts")
            .ConfigureAwait(false);

        // Assert
        Assert.IsFalse(result.IsSuccess);
        Assert.AreEqual(500, result.StatusCode);
        Assert.IsNull(result.Body);

        AssertResponse(result, 500, "POST", "posts", null);
        Assert.AreEqual(0, result.Headers.Count());
        AssertClientDefaults(client);
    }

    [TestMethod]
    public async Task PostAsync_ErrorResponse_ReturnsFailureResult()
    {
        // Arrange
        var errorMessage = "Bad Request";
        using var response = new HttpResponseMessage(statusCode: HttpStatusCode.BadRequest)
        {
            Content = new StringContent(
                $"{{\"message\":\"{errorMessage}\"}}",
                Encoding.UTF8,
                "application/json"
            ),
        };
        var client = new Client(
            CreateHttpClientFactory(false, response),
            new AbsoluteUrl("https://jsonplaceholder.typicode.com"),
            HeadersCollection.Empty,
            throwExceptionOnFailure: false
        );

        var requestBody = new UserPost("New Post", "This is the body of the new post", 1);

        // Act
        var result = await client
            .PostAsync<PostResponse, UserPost>(requestBody, "posts")
            .ConfigureAwait(false);

        // Assert
        Assert.IsFalse(result.IsSuccess);
        Assert.AreEqual(400, result.StatusCode);
        Assert.IsNull(result.Body);

        AssertResponse(result, 400, "POST", "posts", null);
        Assert.AreEqual(0, result.Headers.Count());
        AssertClientDefaults(client);
    }

    [TestMethod]
    public async Task PostAsync_ErrorResponse_ThrowsException()
    {
        // Arrange
        var errorMessage = "Bad Request";
        using var response = new HttpResponseMessage(statusCode: HttpStatusCode.BadRequest)
        {
            Content = new StringContent(
                $"{{\"message\":\"{errorMessage}\"}}",
                Encoding.UTF8,
                "application/json"
            ),
        };
        var client = new Client(
            CreateHttpClientFactory(false, response),
            new AbsoluteUrl("https://jsonplaceholder.typicode.com"),
            HeadersCollection.Empty,
            throwExceptionOnFailure: true
        );

        var requestBody = new UserPost("New Post", "This is the body of the new post", 1);

        var exception = await Assert
            .ThrowsExceptionAsync<HttpStatusException>(
                () => client.PostAsync<PostResponse, UserPost>(requestBody, "posts")
            )
            .ConfigureAwait(false);

        Assert.AreEqual("BadRequest", exception.Message);
        Assert.IsNotNull(exception.Response);
        Assert.AreEqual(400, exception.Response.StatusCode);
        Assert.IsFalse(exception.Response.IsSuccess);
        Assert.AreEqual(
            new AbsoluteUrl("https://jsonplaceholder.typicode.com/posts"),
            exception.Response.RequestUri
        );
        AssertClientDefaults(client);
    }

    [TestMethod]
    public async Task PostAsync_CancellationTokenCancels_ThrowsTaskCanceledException()
    {
        // Arrange
        using var cts = new CancellationTokenSource();
        cts.CancelAfter(TimeSpan.FromMilliseconds(100));

        using var response = new HttpResponseMessage(statusCode: HttpStatusCode.OK)
        {
            Content = new StringContent(
                /*lang=json,strict*/
                "{\"id\": 101}",
                Encoding.UTF8,
                "application/json"
            ),
        };
        var client = new Client(
            CreateHttpClientFactory(false, response, simulatedDelay: TimeSpan.FromSeconds(1)),
            new AbsoluteUrl("https://jsonplaceholder.typicode.com"),
            HeadersCollection.Empty
        );

        var requestBody = new UserPost("New Post", "This is the body of the new post", 1);

        // Act & Assert
        var exception = await Assert
            .ThrowsExceptionAsync<TaskCanceledException>(
                async () =>
                    await client
                        .PostAsync<PostResponse, UserPost>(
                            requestBody: requestBody,
                            resource: new RelativeUrl("posts"),
                            cancellationToken: cts.Token
                        )
                        .ConfigureAwait(false)
            )
            .ConfigureAwait(false);

        Assert.IsTrue(cts.IsCancellationRequested);
        Assert.IsTrue(exception.CancellationToken.IsCancellationRequested);
        Assert.AreEqual(cts.Token, exception.CancellationToken);
        AssertClientDefaults(client);
    }

    [TestMethod]
    public async Task GetAsync_CancellationTokenCancels_ThrowsTaskCanceledException()
    {
        // Arrange
        using var cts = new CancellationTokenSource();
        cts.CancelAfter(TimeSpan.FromMilliseconds(100));

        using var response = new HttpResponseMessage(statusCode: HttpStatusCode.OK);
        response.Content = new StringContent("\"userId\": 1", Encoding.UTF8, "application/json");
        var client = new Client(
            CreateHttpClientFactory(false, response, simulatedDelay: TimeSpan.FromSeconds(1)),
            new AbsoluteUrl("https://jsonplaceholder.typicode.com"),
            HeadersCollection.Empty
        );

        // Act & Assert
        var exception = await Assert
            .ThrowsExceptionAsync<TaskCanceledException>(
                async () =>
                    await client
                        .GetAsync<string>(
                            resource: new RelativeUrl("posts/1"),
                            cancellationToken: cts.Token
                        )
                        .ConfigureAwait(false)
            )
            .ConfigureAwait(false);

        Assert.IsTrue(cts.IsCancellationRequested);
        Assert.IsTrue(exception.CancellationToken.IsCancellationRequested);
        Assert.AreEqual(cts.Token, exception.CancellationToken);
        AssertClientDefaults(client);
    }

    [TestMethod]
    [DataRow(false)]
    [DataRow(true)]
    public async Task PostAsync_WithDefaultAndRequestHeaders_MergesHeaders(bool useDirectRequest)
    {
        var defaultHeaders = new HeadersCollection(
            new Dictionary<string, IEnumerable<string>>
            {
                { "X-Default-Header", ["default-value"] },
                { "X-Default-Token", ["Bearer default-token"] },
                { "X-Shared", ["default-shared"] },
                { "X-Cased", ["default-cased"] },
                { "X-Values", ["default-list"] },
            }
        );
        var requestHeaders = new HeadersCollection(
            new Dictionary<string, IEnumerable<string>>
            {
                { "X-Request-Header", ["request-value"] },
                { "X-Custom-Header", ["custom-value"] },
                { "X-Shared", ["request-shared"] },
                { "x-cased", ["request-cased"] },
                { "X-Values", ["first", "first", "second"] },
            }
        );
        using var handler = new HeaderRecordingHandler();
        using var httpClient = new HttpClient(handler);
        var client = new Client(
            new Fakes.FakeHttpClientFactory(httpClient),
            new AbsoluteUrl("https://jsonplaceholder.typicode.com"),
            defaultHeaders
        );

        for (var interaction = 0; interaction < 3; interaction++)
        {
            // A request without overrides between two overridden requests must not leak state.
            var hasOverrides = interaction != 1;
            var activeHeaders = hasOverrides ? requestHeaders : HeadersCollection.Empty;
            var requestBody = new UserPost($"New Post {interaction}", "This is the body", 1);
            var result = useDirectRequest
                ? await client
                    .SendAsync<PostResponse, UserPost>(
                        new Request<UserPost>(
                            new AbsoluteUrl("https://jsonplaceholder.typicode.com/posts"),
                            requestBody,
                            activeHeaders,
                            HttpRequestMethod.Post
                        )
                    )
                    .ConfigureAwait(false)
                : await client
                    .PostAsync<PostResponse, UserPost>(
                        requestBody: requestBody,
                        resource: new RelativeUrl("posts"),
                        requestHeaders: activeHeaders
                    )
                    .ConfigureAwait(false);

            Assert.IsNotNull(result);
            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(101, result.Body!.id);
            AssertResponse(result, 200, "POST", "posts", new PostResponse(101));
            Assert.AreEqual(interaction + 1, handler.Requests.Count);
            var captured = handler.Requests[interaction];
            Assert.AreEqual(HttpMethod.Post, captured.Method);
            Assert.AreEqual(new Uri("https://jsonplaceholder.typicode.com/posts"), captured.Uri);
            Assert.AreEqual(requestBody, captured.Body);
            Assert.IsNotNull(captured.Body);
            Assert.AreEqual($"New Post {interaction}", captured.Body.Title);
            Assert.AreEqual("This is the body", captured.Body.Body);
            Assert.AreEqual(1, captured.Body.UserId);
            Assert.AreEqual(hasOverrides ? 7 : 5, captured.Headers.Count);
            Assert.AreEqual("default-value", captured.Headers["X-Default-Header"]);
            Assert.AreEqual("Bearer default-token", captured.Headers["X-Default-Token"]);
            Assert.AreEqual(
                hasOverrides ? "request-shared" : "default-shared",
                captured.Headers["X-Shared"]
            );
            Assert.AreEqual(
                hasOverrides ? "request-cased" : "default-cased",
                captured.Headers["X-Cased"]
            );
            Assert.AreEqual(
                hasOverrides ? "first, first, second" : "default-list",
                captured.Headers["X-Values"]
            );
            if (hasOverrides)
            {
                Assert.AreEqual("request-value", captured.Headers["X-Request-Header"]);
                Assert.AreEqual("custom-value", captured.Headers["X-Custom-Header"]);
            }
            else
            {
                Assert.IsFalse(captured.Headers.ContainsKey("X-Request-Header"));
                Assert.IsFalse(captured.Headers.ContainsKey("X-Custom-Header"));
            }

            Assert.AreSame(defaultHeaders, client.DefaultRequestHeaders);
            Assert.AreEqual(5, client.DefaultRequestHeaders.Count());
            Assert.AreEqual(5, requestHeaders.Count());
            Assert.AreEqual(
                "default-shared",
                ((IHeadersCollection)defaultHeaders)["X-Shared"].Single()
            );
            Assert.AreEqual(
                "default-cased",
                ((IHeadersCollection)defaultHeaders)["X-Cased"].Single()
            );
            Assert.AreEqual(
                "default-list",
                ((IHeadersCollection)defaultHeaders)["X-Values"].Single()
            );
            Assert.AreEqual(
                "request-shared",
                ((IHeadersCollection)requestHeaders)["X-Shared"].Single()
            );
            Assert.AreEqual(
                "first, first, second",
                string.Join(", ", ((IHeadersCollection)requestHeaders)["X-Values"])
            );
            Assert.AreEqual(
                new AbsoluteUrl("https://jsonplaceholder.typicode.com"),
                client.BaseUrl
            );
        }
    }

    [TestMethod]
    public async Task GetAsync_WithNoRequestBody_ExecutesSuccessfully()
    {
        // Arrange
        using var response = new HttpResponseMessage(statusCode: HttpStatusCode.OK);
        response.Content = new StringContent(
            /*lang=json,strict*/
            "{\"id\": 1, \"title\": \"Test\"}",
            Encoding.UTF8,
            "application/json"
        );

        var requestCount = 0;
        var hadContent = true;
        var client = new Client(
            CreateMockHttpClientFactory(
                response: response,
                onRequestSent: request =>
                {
                    requestCount++;
                    hadContent = request.Content != null;
                    Assert.AreEqual(HttpMethod.Get, request.Method);
                    Assert.AreEqual(
                        new Uri("https://jsonplaceholder.typicode.com/posts/1"),
                        request.RequestUri
                    );
                    Assert.AreEqual(0, request.Headers.Count());
                }
            ),
            new AbsoluteUrl("https://jsonplaceholder.typicode.com"),
            HeadersCollection.Empty
        );

        // Act
        var result = await client.GetAsync<PostResponse>("posts/1").ConfigureAwait(false);

        // Assert
        Assert.IsTrue(result.IsSuccess);
        Assert.IsNotNull(result.Body);
        Assert.AreEqual(1, result.Body.id);

        AssertResponse(result, 200, "GET", "posts/1", new PostResponse(1));
        Assert.AreEqual(1, requestCount);
        Assert.IsFalse(hadContent);
        AssertClientDefaults(client);
    }

    [TestMethod]
    public async Task PostAsync_ExceptionThrown_RethrowsException()
    {
        // Arrange
        var exceptionMessage = "Network failure";
        var expectedException = new Exception(exceptionMessage);
        var client = new Client(
            CreateHttpClientFactory(false, exception: expectedException),
            new AbsoluteUrl("https://jsonplaceholder.typicode.com"),
            HeadersCollection.Empty,
            throwExceptionOnFailure: true
        );

        var requestBody = new UserPost("New Post", "This is the body of the new post", 1);

        // Act
        var exception = await Assert
            .ThrowsExceptionAsync<Exception>(
                () => client.PostAsync<PostResponse, UserPost>(requestBody, "posts")
            )
            .ConfigureAwait(false);

        Assert.AreSame(expectedException, exception);
        Assert.AreEqual(exceptionMessage, exception.Message);
        Assert.IsNull(exception.InnerException);
        AssertClientDefaults(client);
    }

    [TestMethod]
    public async Task PostAsync_WithNullBody_SendsRequestWithoutContent()
    {
        // Arrange
        using var response = new HttpResponseMessage(statusCode: HttpStatusCode.OK)
        {
            Content = new StringContent(
                /*lang=json,strict*/
                "{\"id\": 101}",
                Encoding.UTF8,
                "application/json"
            ),
        };

        var requestCount = 0;
        var hadContent = true;
        var client = new Client(
            CreateMockHttpClientFactory(
                response: response,
                onRequestSent: request =>
                {
                    requestCount++;
                    hadContent = request.Content != null;
                    Assert.AreEqual(HttpMethod.Post, request.Method);
                    Assert.AreEqual(
                        new Uri("https://jsonplaceholder.typicode.com/posts"),
                        request.RequestUri
                    );
                    Assert.AreEqual(0, request.Headers.Count());
                }
            ),
            new AbsoluteUrl("https://jsonplaceholder.typicode.com"),
            HeadersCollection.Empty
        );

        // Act - Using null as the body
        var result = await client
            .PostAsync<PostResponse, UserPost>(null!, "posts")
            .ConfigureAwait(false);

        // Assert
        Assert.IsNotNull(result);
        Assert.IsTrue(result.IsSuccess);

        AssertResponse(result, 200, "POST", "posts", new PostResponse(101));
        Assert.AreEqual(1, requestCount);
        Assert.IsFalse(hadContent);
        AssertClientDefaults(client);
    }

    [TestMethod]
    public async Task PostAsync_WithNonNullBody_SendsRequestWithContent()
    {
        // Arrange
        using var response = new HttpResponseMessage(statusCode: HttpStatusCode.OK)
        {
            Content = new StringContent(
                /*lang=json,strict*/
                "{\"id\": 101}",
                Encoding.UTF8,
                "application/json"
            ),
        };

        var requestCount = 0;
        UserPost? deserializedRequest = null;
        var client = new Client(
            CreateMockHttpClientFactory(
                response: response,
                onUploadStreamAvailable: async (request, stream, cancellationToken) =>
                {
                    requestCount++;
                    Assert.AreEqual(HttpMethod.Post, request.Method);
                    Assert.AreEqual(
                        new Uri("https://jsonplaceholder.typicode.com/posts"),
                        request.RequestUri
                    );
                    Assert.AreEqual(0, request.Headers.Count());
                    Assert.IsTrue(stream.CanRead);
                    deserializedRequest = await JsonSerializer
                        .DeserializeAsync<UserPost>(stream, cancellationToken: cancellationToken)
                        .ConfigureAwait(false);
                }
            ),
            new AbsoluteUrl("https://jsonplaceholder.typicode.com"),
            HeadersCollection.Empty
        );

        var requestBody = new UserPost("New Post", "This is the body of the new post", 1);

        // Act - Using non-null body
        var result = await client
            .PostAsync<PostResponse, UserPost>(requestBody, "posts")
            .ConfigureAwait(false);

        // Assert
        Assert.IsNotNull(result);
        Assert.IsTrue(result.IsSuccess);
        Assert.AreEqual(101, result.Body!.id);

        AssertResponse(result, 200, "POST", "posts", new PostResponse(101));
        Assert.AreEqual(1, requestCount);
        Assert.AreEqual(requestBody, deserializedRequest);
        Assert.AreEqual("New Post", deserializedRequest!.Title);
        Assert.AreEqual("This is the body of the new post", deserializedRequest.Body);
        Assert.AreEqual(1, deserializedRequest.UserId);
        AssertClientDefaults(client);
    }

    private sealed record CapturedLegacyRequest(
        HttpMethod Method,
        Uri? Uri,
        Dictionary<string, string> Headers,
        UserPost? Body
    );

    private sealed class HeaderRecordingHandler : HttpMessageHandler
    {
        public List<CapturedLegacyRequest> Requests { get; } = [];

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken
        )
        {
            Assert.IsNotNull(request.Content);
            var body = await request
                .Content.ReadAsStringAsync(cancellationToken)
                .ConfigureAwait(false);
            Requests.Add(
                new CapturedLegacyRequest(
                    request.Method,
                    request.RequestUri,
                    request.Headers.ToDictionary(
                        static header => header.Key,
                        static header => string.Join(", ", header.Value),
                        StringComparer.OrdinalIgnoreCase
                    ),
                    JsonSerializer.Deserialize<UserPost>(body)
                )
            );
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("{\"id\": 101}", Encoding.UTF8, "application/json"),
            };
        }
    }

    private static void AssertClientDefaults(Client client)
    {
        Assert.AreEqual(new AbsoluteUrl("https://jsonplaceholder.typicode.com"), client.BaseUrl);
        Assert.IsInstanceOfType<HeadersCollection>(client.DefaultRequestHeaders);
        Assert.AreEqual(0, client.DefaultRequestHeaders.Count());
    }

    private static void AssertResponse<T>(
        Response<T> response,
        int status,
        string method,
        string resource,
        T? expectedBody
    )
    {
        Assert.IsNotNull(response);
        Assert.AreEqual(status, response.StatusCode);
        Assert.AreEqual(status is >= 200 and <= 299, response.IsSuccess);
        Assert.AreEqual(method, response.HttpRequestMethod.ToString().ToUpperInvariant());
        Assert.AreEqual(
            new AbsoluteUrl($"https://jsonplaceholder.typicode.com/{resource}"),
            response.RequestUri
        );
        Assert.AreEqual(expectedBody, response.Body);
        Assert.IsNotNull(response.Headers);
    }
}
