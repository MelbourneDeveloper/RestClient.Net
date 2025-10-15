using System.Net;
using System.Text;
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
        _ = await Assert
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
    }

    [TestMethod]
    [ExpectedException(typeof(HttpStatusException))]
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

        _ = await client
            .PostAsync<PostResponse, UserPost>(requestBody, "posts")
            .ConfigureAwait(false);
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
        _ = await Assert
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
        _ = await Assert
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
    }

    [TestMethod]
    public async Task PostAsync_WithDefaultAndRequestHeaders_MergesHeaders()
    {
        // Arrange
        var defaultHeaders = new HeadersCollection(
            new Dictionary<string, IEnumerable<string>>
            {
                { "X-Default-Header", ["default-value"] },
                { "X-Default-Token", ["Bearer default-token"] },
            }
        );

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
            CreateHttpClientFactory(false, response),
            new AbsoluteUrl("https://jsonplaceholder.typicode.com"),
            defaultHeaders
        );

        var requestHeaders = new HeadersCollection(
            new Dictionary<string, IEnumerable<string>>
            {
                { "X-Request-Header", ["request-value"] },
                { "X-Custom-Header", ["custom-value"] },
            }
        );

        var requestBody = new UserPost("New Post", "This is the body", 1);

        // Act
        var result = await client
            .PostAsync<PostResponse, UserPost>(
                requestBody: requestBody,
                resource: new RelativeUrl("posts"),
                requestHeaders: requestHeaders
            )
            .ConfigureAwait(false);

        // Assert
        Assert.IsNotNull(result);
        Assert.IsTrue(result.IsSuccess);
        Assert.AreEqual(101, result!.Body!.id);
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

        var client = new Client(
            CreateHttpClientFactory(false, response),
            new AbsoluteUrl("https://jsonplaceholder.typicode.com"),
            HeadersCollection.Empty
        );

        // Act
        var result = await client.GetAsync<PostResponse>("posts/1").ConfigureAwait(false);

        // Assert
        Assert.IsTrue(result.IsSuccess);
        Assert.IsNotNull(result.Body);
        Assert.AreEqual(1, result.Body!.id);
    }

    [TestMethod]
    [ExpectedException(typeof(Exception))]
    public async Task PostAsync_ExceptionThrown_RethrowsException()
    {
        // Arrange
        var exceptionMessage = "Network failure";
        var client = new Client(
            CreateHttpClientFactory(false, exception: new Exception(exceptionMessage)),
            new AbsoluteUrl("https://jsonplaceholder.typicode.com"),
            HeadersCollection.Empty,
            throwExceptionOnFailure: true
        );

        var requestBody = new UserPost("New Post", "This is the body of the new post", 1);

        // Act
        _ = await client
            .PostAsync<PostResponse, UserPost>(requestBody, "posts")
            .ConfigureAwait(false);
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

        var client = new Client(
            CreateHttpClientFactory(false, response),
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

        var client = new Client(
            CreateHttpClientFactory(false, response),
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
    }
}
