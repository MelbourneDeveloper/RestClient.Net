using System.Net;
using System.Text;
using Outcome;
using RestClient.Net.CsTest.Models;
using RestClient.Net.CsTest.Utilities;
using RestClient.Net.Utilities;
using Urls;
using static RestClient.Net.CsTest.Fakes.FakeHttpClientFactory;
using static RestClient.Net.HttpClientFactoryExtensions;

// We don't need this in these cases because we have Exhaustion
#pragma warning disable CS8509 // The switch expression does not handle all possible values of its input type (it is not exhaustive).
#pragma warning disable CA1812


namespace RestClient.Net.CsTest;

[TestClass]
public sealed class HttpClientFactoryExtensionsTests
{
    static HttpResponseMessage GetResponse() =>
        new(HttpStatusCode.OK)
        {
            Content = new StringContent( /*lang=json,strict*/
                "{\n  \"userId\": 1,\n  \"id\": 1,\n  \"title\": \"sunt aut facere repellat provident occaecati excepturi optio reprehenderit\",\n  \"body\": \"quia et suscipit\\nsuscipit recusandae consequuntur expedita et cum\\nreprehenderit molestiae ut ut quas totam\\nnostrum rerum est autem sunt rem eveniet architecto\"\n}",
                Encoding.UTF8,
                "application/json"
            ),
        };

    static HttpResponseMessage PutResponse() =>
        new(HttpStatusCode.OK)
        {
            Content = new StringContent( /*lang=json,strict*/
                "{\n  \"id\": 1\n}",
                Encoding.UTF8,
                "application/json"
            ),
        };

    static HttpResponseMessage PostResponse() =>
        new(HttpStatusCode.OK)
        {
            Content = new StringContent( /*lang=json,strict*/
                "{\n  \"id\": 101\n}",
                Encoding.UTF8,
                "application/json"
            ),
        };

    static HttpResponseMessage DeleteResponse() =>
        new(HttpStatusCode.OK)
        {
            Content = new StringContent( /*lang=json,strict*/
                "{}",
                Encoding.UTF8,
                "application/json"
            ),
        };

    static HttpResponseMessage PatchResponse() =>
        new(HttpStatusCode.OK)
        {
            Content = new StringContent( /*lang=json,strict*/
                "{\n  \"id\": 1\n}",
                Encoding.UTF8,
                "application/json"
            ),
        };

    [TestMethod]
    [DataRow(true)]
    [DataRow(false)]
    public async Task GetAsync_ReturnsSuccessResult(bool useRealApi)
    {
        // Arrange
        using var response = GetResponse();
        var httpClientFactory = CreateHttpClientFactory(useRealApi, response: response);

        // Act
        var result = await httpClientFactory
            .GetAsync(
                clientName: "JsonPlaceholderClient",
                url: "https://jsonplaceholder.typicode.com/posts/1".ToAbsoluteUrl(),
                deserializeSuccess: TestDeserializer.Deserialize<string>,
                deserializeError: TestDeserializer.Deserialize<MyErrorModel>
            )
            .ConfigureAwait(false);

        // Assert
        Assert.AreEqual(
            +result,
            /*lang=json,strict*/
            "{\n  \"userId\": 1,\n  \"id\": 1,\n  \"title\": \"sunt aut facere repellat provident occaecati excepturi optio reprehenderit\",\n  \"body\": \"quia et suscipit\\nsuscipit recusandae consequuntur expedita et cum\\nreprehenderit molestiae ut ut quas totam\\nnostrum rerum est autem sunt rem eveniet architecto\"\n}"
        );
    }

    [TestMethod]
    [DataRow(true)]
    [DataRow(false)]
    public async Task PutAsync_ReturnsSuccessResult(bool useRealApi)
    {
        // Arrange
        using var response = PutResponse();
        var httpClientFactory = CreateHttpClientFactory(useRealApi, response: response);
        using var requestBody = new ProgressReportingHttpContent(
            new MemoryStream(
                Encoding.UTF8.GetBytes( /*lang=json,strict*/
                    "{\"id\":1,\"title\":\"Updated Title\",\"body\":\"Updated body\",\"userId\":1}"
                )
            )
        );
        var expectedResponse = /*lang=json,strict*/
            "{\n  \"id\": 1\n}";

        // Act
        var result = await httpClientFactory
            .PutAsync(
                clientName: "JsonPlaceholderClient",
                url: "https://jsonplaceholder.typicode.com/posts/1".ToAbsoluteUrl(),
                requestBody: requestBody,
                deserializeSuccess: TestDeserializer.Deserialize<string>,
                deserializeError: TestDeserializer.Deserialize<MyErrorModel>
            )
            .ConfigureAwait(false);

        // Assert
        var successResult = +result;
        Assert.AreEqual(expectedResponse, successResult);
    }

    [TestMethod]
    [DataRow(true)]
    [DataRow(false)]
    public async Task PostAsync_ReturnsSuccessResult(bool useRealApi)
    {
        // Arrange
        using var response = PostResponse();
        var httpClientFactory = CreateHttpClientFactory(useRealApi, response: response);
        using var requestBody = new ProgressReportingHttpContent(
            Encoding.UTF8.GetBytes( /*lang=json,strict*/
                "{\"title\":\"New Post\",\"body\":\"This is the body of the new post\",\"userId\":1}"
            )
        );
        var expectedResponse = /*lang=json,strict*/
            "{\n  \"id\": 101\n}";

        // Act
        var result = await httpClientFactory
            .PostAsync(
                clientName: "JsonPlaceholderClient",
                url: "https://jsonplaceholder.typicode.com/posts".ToAbsoluteUrl(),
                requestBody: requestBody,
                deserializeSuccess: TestDeserializer.Deserialize<string>,
                deserializeError: TestDeserializer.Deserialize<MyErrorModel>
            )
            .ConfigureAwait(false);

        // Assert
        var successResult = +result;
        Assert.AreEqual(expectedResponse, successResult);
    }

    [TestMethod]
    [DataRow(null, DisplayName = "Null headers")]
    [DataRow("", DisplayName = "Empty headers")]
    [DataRow("X-Request-Header=RequestValue", DisplayName = "Single header")]
    [DataRow("X-Request-Header=RequestValue,X-Api-Key=secret123", DisplayName = "Multiple headers")]
    public async Task GetAsync_SuccessResponse_ReturnsSuccessResult(string? headerString)
    {
        // Arrange
        var expectedContent = "Success content";
        using var response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(expectedContent),
        };
        response.Headers.Add("X-Test-Header", "TestValue");

        Dictionary<string, string>? headers = headerString switch
        {
            null => null,
            "" => [],
            _ => headerString
                .Split(',')
                .Select(h => h.Split('='))
                .ToDictionary(p => p[0], p => p[1]),
        };

        HttpRequestMessage? capturedRequest = null;
        var httpClientFactory = CreateMockHttpClientFactory(
            response: response,
            onRequestSent: req => capturedRequest = req
        );

        // Act
        var result = await httpClientFactory
            .SendAsync(
                clientName: "TestClient",
                url: "http://test.com".ToAbsoluteUrl(),
                httpMethod: HttpMethod.Get,
                deserializeSuccess: static async (r, c) =>
                    new BodyAndHeader(
                        await r.Content.ReadAsStringAsync(c).ConfigureAwait(false),
                        r.Headers.GetValues("X-Test-Header").First()
                    ),
                deserializeError: TestDeserializer.Deserialize<MyErrorModel>,
                headers: headers
            )
            .ConfigureAwait(false);

        // Assert
        var successValue = +result;

        Assert.AreEqual(expectedContent, successValue.Body);
        Assert.AreEqual("TestValue", successValue.HeaderValue);

        // Verify headers were actually sent in the request
        Assert.IsNotNull(capturedRequest, "Request should have been captured");
        if (headers != null && headers.Count > 0)
        {
            foreach (var header in headers)
            {
                Assert.IsTrue(
                    capturedRequest.Headers.Contains(header.Key),
                    $"Request should contain header {header.Key}"
                );
                var values = capturedRequest.Headers.GetValues(header.Key);
                Assert.AreEqual(
                    header.Value,
                    values.First(),
                    $"Header {header.Key} should have correct value"
                );
            }
        }
        else
        {
            // When headers is null or empty, verify no custom headers were added
            var hasCustomHeaders = capturedRequest.Headers.Any(h =>
                h.Key.StartsWith("X-", StringComparison.Ordinal)
            );
            Assert.IsFalse(
                hasCustomHeaders,
                "No custom headers should be present when input is null/empty"
            );
        }
    }

    [TestMethod]
    public async Task GetAsync_ErrorResponse_ReturnsFailureResult()
    {
        // Arrange
        var expectedErrorContent = "Error occurred";
        using var errorResponse = new HttpResponseMessage(HttpStatusCode.BadRequest)
        {
            Content = new StringContent( /*lang=json,strict*/
                "{\"message\":\"Error occurred\"}"
            ),
        };
        errorResponse.Headers.Add("X-Custom-Header", "CustomValue");

        var httpClientFactory = CreateMockHttpClientFactory(response: errorResponse);

        // Act
        var result = await httpClientFactory
            .SendAsync(
                clientName: "TestClient",
                url: "http://test.com".ToAbsoluteUrl(),
                httpMethod: HttpMethod.Get,
                deserializeSuccess: TestDeserializer.Deserialize<string>,
                deserializeError: TestDeserializer.Deserialize<MyErrorModel>
            )
            .ConfigureAwait(false);

        // Assert
        var (body, statusCode, headers) = !result switch
        {
            ResponseError(var b, var sc, var h) => (b, sc, h),
            ExceptionError(var ex) => throw new InvalidOperationException(
                "Expected error response",
                ex
            ),
        };

        Assert.AreEqual(HttpStatusCode.BadRequest, statusCode);
        Assert.AreEqual(expectedErrorContent, body.Message);
        Assert.IsTrue(headers.Contains("X-Custom-Header"));
        Assert.AreEqual("CustomValue", headers.GetValues("X-Custom-Header").First());
    }

    [TestMethod]
    public async Task GetAsync_ExceptionThrown_ReturnsFailureResult()
    {
        // Arrange
        var exceptionMessage = "Network failure";
        var httpClientFactory = CreateMockHttpClientFactory(
            exceptionToThrow: new Exception(exceptionMessage)
        );

        // Act
        var result = await httpClientFactory
            .SendAsync(
                clientName: "TestClient",
                url: "http://test.com".ToAbsoluteUrl(),
                httpMethod: HttpMethod.Get,
                deserializeSuccess: TestDeserializer.Deserialize<string>,
                deserializeError: TestDeserializer.Deserialize<MyErrorModel>
            )
            .ConfigureAwait(false);

        // Assert
        var exception = !result switch
        {
            ExceptionError(var ex) => ex,
            ResponseError(var b, var sc, var h) => throw new InvalidOperationException(
                "Expected exception error"
            ),
        };

        Assert.AreEqual(exceptionMessage, exception.Message);
    }

    [TestMethod]
    public async Task CallAsync_PutAsync_SuccessResponse_ReturnsSuccessResult()
    {
        // Arrange
        var expectedContent = "Put Success";
        using var response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(expectedContent),
        };
        var httpClientFactory = CreateMockHttpClientFactory(response: response);

        // Act
        using var requestBody = new ProgressReportingHttpContent(Encoding.UTF8.GetBytes("{}"));
        var result = await httpClientFactory
            .SendAsync(
                clientName: "TestClient",
                url: "http://test.com".ToAbsoluteUrl(),
                httpMethod: HttpMethod.Put,
                deserializeSuccess: TestDeserializer.Deserialize<string>,
                deserializeError: TestDeserializer.Deserialize<MyErrorModel>,
                requestBody: requestBody
            )
            .ConfigureAwait(false);

        // Assert
        var successResult = +result;
        Assert.AreEqual(expectedContent, successResult);
    }

    [TestMethod]
    public async Task CallAsync_PostAsync_ErrorResponse_ReturnsFailureResult()
    {
        // Arrange
        var expectedErrorContent = "Post Error";
        using var response = new HttpResponseMessage(HttpStatusCode.BadRequest)
        {
            Content = new StringContent( /*lang=json,strict*/
                "{\"message\":\"Post Error\"}"
            ),
        };
        var httpClientFactory = CreateMockHttpClientFactory(response: response);

        // Act
        using var requestBody = new ProgressReportingHttpContent(Encoding.UTF8.GetBytes("{}"));
        var result = await httpClientFactory
            .SendAsync(
                clientName: "TestClient",
                url: "http://test.com".ToAbsoluteUrl(),
                httpMethod: HttpMethod.Post,
                deserializeSuccess: TestDeserializer.Deserialize<string>,
                deserializeError: TestDeserializer.Deserialize<MyErrorModel>,
                requestBody: requestBody
            )
            .ConfigureAwait(false);

        // Assert
        var error = !result;
        var (body, statusCode) = error switch
        {
            ResponseError(var b, var sc, _) => (b, sc),
            ExceptionError(var ex) => throw new InvalidOperationException(
                "Expected error response",
                ex
            ),
        };
        Assert.AreEqual(HttpStatusCode.BadRequest, statusCode);
        Assert.AreEqual(expectedErrorContent, body.Message);
    }

    [TestMethod]
    [DataRow(true)]
    [DataRow(false)]
    public async Task DeleteAsync_ReturnsSuccessResult(bool useRealApi)
    {
        // Arrange
        using var response = DeleteResponse();
        var httpClientFactory = CreateHttpClientFactory(useRealApi, response: response);
        var expectedResponse = "{}";

        // Act
        var result = await httpClientFactory
            .DeleteAsync(
                clientName: "JsonPlaceholderClient",
                url: "https://jsonplaceholder.typicode.com/posts/1".ToAbsoluteUrl(),
                deserializeSuccess: TestDeserializer.Deserialize<string>,
                deserializeError: TestDeserializer.Deserialize<MyErrorModel>
            )
            .ConfigureAwait(false);

        // Assert
        var successResult = +result;
        Assert.AreEqual(expectedResponse, successResult);
    }

    [TestMethod]
    public async Task PostAsync_ErrorResponse_ReturnsFailureResult()
    {
        // Arrange
        var expectedErrorContent = "Post Error";
        using var postErrorResponse = new HttpResponseMessage(HttpStatusCode.BadRequest)
        {
            Content = new StringContent( /*lang=json,strict*/
                "{\"message\":\"Post Error\"}"
            ),
        };
        var httpClientFactory = CreateMockHttpClientFactory(response: postErrorResponse);

        // Act
        using var requestBody = new ProgressReportingHttpContent(Encoding.UTF8.GetBytes("{}"));
        var result = await httpClientFactory
            .SendAsync(
                clientName: "TestClient",
                url: "http://test.com".ToAbsoluteUrl(),
                httpMethod: HttpMethod.Post,
                deserializeSuccess: TestDeserializer.Deserialize<string>,
                deserializeError: TestDeserializer.Deserialize<MyErrorModel>, // Ensure MyErrorModel can handle the error response
                requestBody: requestBody
            )
            .ConfigureAwait(false);

        // Assert
        var (body, statusCode) = !result switch
        {
            ResponseError(var b, var sc, _) => (b, sc),
            ExceptionError(var ex) => throw new InvalidOperationException(
                "Expected error response",
                ex
            ),
        };
        Assert.AreEqual(HttpStatusCode.BadRequest, statusCode);

        // Verify deserialization of the error model
        Assert.IsNotNull(body);
        Assert.AreEqual(expectedErrorContent, body.Message); // Ensure the error message matches
    }

    [TestMethod]
    public async Task SendAsync_DownloadsFileInChunks_ReportsProgress()
    {
        var progressReports = new List<(long, long)>();

        // Arrange
        var content = "This is a test file content";
        using var response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new ProgressReportingHttpContent(
                Encoding.UTF8.GetBytes(content),
                progress: (current, total) => progressReports.Add((current, total))
            ),
        };
        response.Content.Headers.ContentLength = content.Length;

        var factory = CreateMockHttpClientFactory(response, chunkedData: content);

        // Act
        var result = await factory
            .SendAsync(
                "testClient",
                "https://example.com/file".ToAbsoluteUrl(),
                HttpMethod.Get,
                async (response, ct) =>
                    await response.Content.ReadAsStringAsync(ct).ConfigureAwait(false),
                (_, _) => Task.FromResult("Error"),
                cancellationToken: CancellationToken.None
            )
            .ConfigureAwait(false);

        // Assert
        var result1 = +result;
        Assert.AreEqual(content, result1);
        Assert.IsTrue(progressReports.Count > 0);
        Assert.AreEqual((content.Length, content.Length), progressReports.Last());
    }

    [TestMethod]
    public async Task DownloadFileAsync_FailureResponse_ReturnsFailureResult()
    {
        // Arrange
        using var response = new HttpResponseMessage(HttpStatusCode.NotFound);
        var httpClientFactory = CreateMockHttpClientFactory(response: response);

        // Act
        var result = await httpClientFactory
            .DownloadFileAsync(
                clientName: "TestClient",
                url: "http://test.com/nonexistentfile".ToAbsoluteUrl(),
                destinationStream: new MemoryStream(),
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
                deserializeError: static async (m, c) => ""
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
            )
            .ConfigureAwait(false);

        // Assert
        var (body, statusCode) = !result switch
        {
            ResponseErrorString(var b, var sc, _) => (b, sc),
            ExceptionErrorString(var ex) => throw new InvalidOperationException(
                "Expected error response",
                ex
            ),
        };
        Assert.AreEqual(HttpStatusCode.NotFound, statusCode);
        Assert.AreEqual("", body);
    }

    [TestMethod]
    public async Task DownloadFileAsync_ExceptionThrown_ReturnsFailureResult()
    {
        // Arrange
        var httpClientFactory = CreateMockHttpClientFactory(
            exceptionToThrow: new HttpRequestException("Network error")
        );

        // Act
        var result = await httpClientFactory
            .DownloadFileAsync(
                clientName: "TestClient",
                url: "http://test.com/file".ToAbsoluteUrl(),
                destinationStream: new MemoryStream(),
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
                deserializeError: static async (m, c) => ""
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
            )
            .ConfigureAwait(false);

        // Assert
        var exception = !result switch
        {
            ExceptionErrorString(var ex) => ex,
            ResponseErrorString(var b, var sc, var h) => throw new InvalidOperationException(
                "Expected exception error"
            ),
        };

        Assert.AreEqual("Network error", exception.Message);
    }

    [TestMethod]
    public async Task DownloadFileAsync_SuccessfullyDownloadsFile_ReportsProgress()
    {
        var progressReports = new List<(long, long)>();

        // Arrange
        var content = "This is a test file content";
        using var response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new ProgressReportingHttpContent(
                Encoding.UTF8.GetBytes(content),
                progress: (c, t) => progressReports.Add((c, t))
            ),
        };
        response.Content.Headers.ContentLength = content.Length;

        var factory = CreateMockHttpClientFactory(response, chunkedData: content);

        var destinationStream = new MemoryStream();

        // Act
        var result = await factory
            .DownloadFileAsync(
                clientName: "testClient",
                url: "https://example.com/file".ToAbsoluteUrl(),
                destinationStream: destinationStream,
                deserializeError: (_, _) => Task.FromResult("Error"),
                cancellationToken: CancellationToken.None
            )
            .ConfigureAwait(false);

        // Assert
        _ = +result; // Verify it's a success result

        Assert.IsTrue(progressReports.Count > 0);
        Assert.AreEqual((content.Length, content.Length), progressReports.Last());

        destinationStream.Position = 0;
        using var reader = new StreamReader(destinationStream);
        var downloadedContent = await reader.ReadToEndAsync().ConfigureAwait(false);
        Assert.AreEqual(content, downloadedContent);
    }

    [TestMethod]
    public async Task DownloadFileAsync_HandlesError_ReturnsFailureResult()
    {
        // Arrange
        using var errorResponse = new HttpResponseMessage(HttpStatusCode.NotFound)
        {
            Content = new StringContent("File not found"),
        };
        var factory = CreateMockHttpClientFactory(errorResponse);
        var destinationStream = new MemoryStream();

        // Act
        var result = await factory
            .DownloadFileAsync(
                clientName: "testClient",
                url: "https://example.com/nonexistent".ToAbsoluteUrl(),
                destinationStream: destinationStream,
                deserializeError: static async (response, ct) =>
                    await response.Content.ReadAsStringAsync(ct).ConfigureAwait(false),
                cancellationToken: CancellationToken.None
            )
            .ConfigureAwait(false);

        // Assert
        var (body, statusCode) = !result switch
        {
            ResponseErrorString(var b, var sc, _) => (b, sc),
            ExceptionErrorString(var ex) => throw new InvalidOperationException(
                "Expected error response",
                ex
            ),
        };

        Assert.AreEqual(HttpStatusCode.NotFound, statusCode);
        Assert.AreEqual("File not found", body);
    }

    [TestMethod]
    public async Task UploadAsync_ReturnsSuccessResult_WithProgress()
    {
        var progressReports = new List<(long Current, long Total)>();
        var requestString = "";

        // Arrange
        var uploadContent = "This is a test upload content";
        using var response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("Upload Successful"),
        };

        var httpClientFactory = CreateMockHttpClientFactory(
            response: response,
            onUploadStreamAvailable: async (request, stream, ct) =>
            {
                var buffer = new byte[5];
                int bytesRead;
                while ((bytesRead = await stream.ReadAsync(buffer, ct).ConfigureAwait(false)) > 0)
                {
                    var chunk = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                    requestString += chunk;
                    progressReports.Add((requestString.Length, uploadContent.Length));
                }
            }
        );

        // Act
        using var fileStream = new ProgressReportingHttpContent(
            uploadContent,
            progress: (c, t) => { }
        );
        var result = await httpClientFactory
            .UploadFileAsync(
                clientName: "TestClient",
                url: "http://test.com/upload".ToAbsoluteUrl(),
                fileStream: fileStream,
                deserializeSuccess: async (response, ct) =>
                    await response.Content.ReadAsStringAsync(ct).ConfigureAwait(false),
                deserializeError: async (response, ct) =>
                    await response.Content.ReadAsStringAsync(ct).ConfigureAwait(false)
            )
            .ConfigureAwait(false);

        // Assert
        var successResult = +result;
        Assert.AreEqual("Upload Successful", successResult);

        Assert.IsTrue(progressReports.Count > 0, "Progress reports should have been recorded.");
        Assert.AreEqual(
            uploadContent.Length,
            progressReports.Last().Current,
            "Final uploaded bytes should match the content length."
        );
        Assert.AreEqual(
            uploadContent.Length,
            progressReports.Last().Total,
            "Total bytes should match the content length."
        );
        Assert.AreEqual(uploadContent, requestString, "Uploaded content should match");
    }

    [TestMethod]
    public async Task UploadAsync_ReturnsFailureResult_OnError()
    {
        // Arrange
        var uploadContent = "This is a test upload content";
        using var errorResponse = new HttpResponseMessage(HttpStatusCode.BadRequest)
        {
            Content = new StringContent( /*lang=json,strict*/
                "{\"message\":\"Upload Failed\"}"
            ),
        };

        var progressReports = new List<(long Current, long Total)>();

        var httpClientFactory = CreateMockHttpClientFactory(
            response: errorResponse,
            onUploadStreamAvailable: async (request, stream, ct) =>
            {
                var buffer = new byte[5];
                int bytesRead;
                long totalBytesRead = 0;
                while ((bytesRead = await stream.ReadAsync(buffer, ct).ConfigureAwait(false)) > 0)
                {
                    totalBytesRead += bytesRead;
                    progressReports.Add((totalBytesRead, uploadContent.Length));
                }
            }
        );

        // Act
        using var fileStream = new ProgressReportingHttpContent(
            Encoding.UTF8.GetBytes(uploadContent),
            progress: (c, t) => { },
            bufferSize: 5
        );
        var result = await httpClientFactory
            .UploadFileAsync(
                clientName: "TestClient",
                url: "http://test.com/upload".ToAbsoluteUrl(),
                fileStream: fileStream,
                deserializeSuccess: async (response, ct) =>
                    await response.Content.ReadAsStringAsync(ct).ConfigureAwait(false),
                deserializeError: async (response, ct) =>
                    await response.Content.ReadAsStringAsync(ct).ConfigureAwait(false),
                cancellationToken: CancellationToken.None
            )
            .ConfigureAwait(false);

        // Assert
        var (body, statusCode) = !result switch
        {
            ResponseErrorString(var b, var sc, _) => (b, sc),
            ExceptionErrorString(var ex) => throw new InvalidOperationException(
                "Expected error response",
                ex
            ),
        };
        Assert.AreEqual(
            HttpStatusCode.BadRequest,
            statusCode,
            "Status code should be 400 Bad Request."
        );
        Assert.AreEqual(
            /*lang=json,strict*/
            "{\"message\":\"Upload Failed\"}",
            body,
            "Error message should match."
        );

        Assert.IsTrue(progressReports.Count > 0, "Progress reports should have been recorded.");
        Assert.AreEqual(
            uploadContent.Length,
            progressReports.Last().Current,
            "Final uploaded bytes should match the content length."
        );
        Assert.AreEqual(
            uploadContent.Length,
            progressReports.Last().Total,
            "Total bytes should match the content length."
        );
    }

    [TestMethod]
#pragma warning disable CA1506 // Avoid excessive class coupling - acceptable in complex integration tests
    public async Task MultiFileUploadAsync_ReturnsSuccessResult_WithProgress()
#pragma warning restore CA1506
    {
        // Arrange
        var files = new List<(string Name, Stream Stream)>
        {
            ("file1.txt", new MemoryStream(Encoding.UTF8.GetBytes("File 1 content"))),
            ("file2.txt", new MemoryStream(Encoding.UTF8.GetBytes("File 2 content"))),
        };

        using var response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("Multi-file upload successful"),
        };

        var progressReports = new List<(long Current, long Total)>();
        var uploadedFiles = new List<(string Name, string Content)>();

        var httpClientFactory = CreateMockHttpClientFactory(
            response: response,
            onMultipartUploadAvailable: async (request, multipartContent, ct) =>
            {
                foreach (var content in multipartContent)
                {
                    var fileName = content.Headers.ContentDisposition?.FileName?.Trim('"');
                    using var stream = await content.ReadAsStreamAsync(ct).ConfigureAwait(false);
                    using var reader = new StreamReader(stream);
                    var fileContent = await reader.ReadToEndAsync(ct).ConfigureAwait(false);
                    uploadedFiles.Add((fileName!, fileContent));
                    progressReports.Add(
                        (uploadedFiles.Sum(f => f.Content.Length), files.Sum(f => f.Stream.Length))
                    );
                }
            }
        );

        using var multiContent = new MultipartFormDataContent();
#pragma warning disable CA2000 // Dispose objects before losing scope - MultipartFormDataContent disposes child content
        foreach (var file in files)
        {
            var fileContent = new ProgressReportingHttpContent(
                content: file.Stream,
                progress: (current, total) => { }
            );
            multiContent.Add(fileContent, "files", file.Name);
        }
#pragma warning restore CA2000 // Dispose objects before losing scope

        // Act
        var result = await httpClientFactory
            .SendAsync(
                clientName: "TestClient",
                url: "http://test.com/multi-upload".ToAbsoluteUrl(),
                httpOperation: (client, request, ct) => client.SendAsync(request, ct),
                httpMethod: HttpMethod.Post,
                deserializeSuccess: async (response, ct) =>
                    await response.Content.ReadAsStringAsync(ct).ConfigureAwait(false),
                deserializeError: async (response, ct) =>
                    await response.Content.ReadAsStringAsync(ct).ConfigureAwait(false),
                requestBody: multiContent,
                cancellationToken: CancellationToken.None
            )
            .ConfigureAwait(false);

        // Assert
        var successResult = +result;
        Assert.AreEqual("Multi-file upload successful", successResult);

        // Verify upload progress was reported correctly
        Assert.IsTrue(progressReports.Count > 0, "Progress reports should have been recorded.");
        Assert.AreEqual(
            files.Sum(f => f.Stream.Length),
            progressReports.Last().Current,
            "Final uploaded bytes should match the total content length."
        );
        Assert.AreEqual(
            files.Sum(f => f.Stream.Length),
            progressReports.Last().Total,
            "Total bytes should match the total content length."
        );

        // Verify uploaded files match
        Assert.AreEqual(files.Count, uploadedFiles.Count, "Number of uploaded files should match");
        foreach (var file in files)
        {
            var (name, content1) = uploadedFiles.Single(f => f.Name == file.Name);
            file.Stream.Position = 0;
            using var reader = new StreamReader(file.Stream);
            var expectedContent = await reader.ReadToEndAsync().ConfigureAwait(false);
            Assert.AreEqual(expectedContent, content1, $"Content of file {file.Name} should match");
        }
    }

    [TestMethod]
    public async Task MultiFileUploadAsync_ReturnsFailureResult_OnError()
    {
        // Arrange
        var files = new List<(string Name, Stream Stream)>
        {
            ("file1.txt", new MemoryStream(Encoding.UTF8.GetBytes("File 1 content"))),
            ("file2.txt", new MemoryStream(Encoding.UTF8.GetBytes("File 2 content"))),
        };

        using var errorResponse = new HttpResponseMessage(HttpStatusCode.BadRequest)
        {
            Content = new StringContent( /*lang=json,strict*/
                "{\"message\":\"Multi-file upload failed\"}"
            ),
        };

        var progressReports = new List<(long Current, long Total)>();
        var uploadedFiles = new List<(string Name, string Content)>();

        var httpClientFactory = CreateMockHttpClientFactory(
            response: errorResponse,
            onMultipartUploadAvailable: async (request, multipartContent, ct) =>
            {
                foreach (var content in multipartContent)
                {
                    var fileName = content.Headers.ContentDisposition?.FileName?.Trim('"');
                    using var stream = await content.ReadAsStreamAsync(ct).ConfigureAwait(false);
                    using var reader = new StreamReader(stream);
                    var fileContent = await reader.ReadToEndAsync(ct).ConfigureAwait(false);
                    uploadedFiles.Add((fileName!, fileContent));
                    progressReports.Add(
                        (uploadedFiles.Sum(f => f.Content.Length), files.Sum(f => f.Stream.Length))
                    );
                }
            }
        );

        using var multiContent = new MultipartFormDataContent();
#pragma warning disable CA2000 // Dispose objects before losing scope - MultipartFormDataContent disposes child content
        foreach (var file in files)
        {
            var fileContent = new ProgressReportingHttpContent(
                content: file.Stream,
                progress: (current, total) => { }
            );
            multiContent.Add(fileContent, "files", file.Name);
        }
#pragma warning restore CA2000 // Dispose objects before losing scope

        // Act
        var result = await httpClientFactory
            .SendAsync(
                httpMethod: HttpMethod.Post,
                clientName: "TestClient",
                url: "http://test.com/multi-upload".ToAbsoluteUrl(),
                httpOperation: (client, request, ct) => client.SendAsync(request, ct),
                deserializeSuccess: async (response, ct) =>
                    await response.Content.ReadAsStringAsync(ct).ConfigureAwait(false),
                deserializeError: async (response, ct) =>
                    await response.Content.ReadAsStringAsync(ct).ConfigureAwait(false),
                requestBody: multiContent,
                cancellationToken: CancellationToken.None
            )
            .ConfigureAwait(false);

        // Assert
        var (body1, statusCode, headers) = (ResponseErrorString)!result;

        Assert.AreEqual(
            HttpStatusCode.BadRequest,
            statusCode,
            "Status code should be 400 Bad Request."
        );
        Assert.AreEqual(
            /*lang=json,strict*/
            "{\"message\":\"Multi-file upload failed\"}",
            body1,
            "Error message should match."
        );

        // Verify upload progress was reported correctly
        Assert.IsTrue(progressReports.Count > 0, "Progress reports should have been recorded.");
        Assert.AreEqual(
            files.Sum(f => f.Stream.Length),
            progressReports.Last().Current,
            "Final uploaded bytes should match the total content length."
        );
        Assert.AreEqual(
            files.Sum(f => f.Stream.Length),
            progressReports.Last().Total,
            "Total bytes should match the total content length."
        );

        // Verify uploaded files match
        Assert.AreEqual(files.Count, uploadedFiles.Count, "Number of uploaded files should match");
        foreach (var file in files)
        {
            var (name, content1) = uploadedFiles.Single(f => f.Name == file.Name);
            file.Stream.Position = 0;
            using var reader = new StreamReader(file.Stream);
            var expectedContent = await reader.ReadToEndAsync().ConfigureAwait(false);
            Assert.AreEqual(expectedContent, content1, $"Content of file {file.Name} should match");
        }
    }

    [TestMethod]
    public async Task PatchAsync_ReturnsSuccessResult()
    {
        // Arrange
        using var response = PatchResponse();
        var httpClientFactory = CreateMockHttpClientFactory(response: response);
        using var requestBody = new ProgressReportingHttpContent(
            new MemoryStream(
                Encoding.UTF8.GetBytes( /*lang=json,strict*/
                    "{\"id\":1,\"title\":\"Patched Title\",\"body\":\"Patched body\",\"userId\":1}"
                )
            )
        );
        var expectedResponse = /*lang=json,strict*/
            "{\n  \"id\": 1\n}";

        // Act
        var result = await httpClientFactory
            .PatchAsync(
                clientName: "TestClient",
                url: "http://test.com/posts/1".ToAbsoluteUrl(),
                requestBody: requestBody,
                deserializeSuccess: TestDeserializer.Deserialize<string>,
                deserializeError: TestDeserializer.Deserialize<MyErrorModel>
            )
            .ConfigureAwait(false);

        // Assert
        var successResult = +result;
        Assert.AreEqual(expectedResponse, successResult);
    }

    [TestMethod]
    public async Task CreatePatch_ReturnsSuccessResult()
    {
        // Arrange
        var expectedContent = "Patch Success";
        using var response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(expectedContent),
        };
        using var httpClient = CreateMockHttpClientFactory(response: response).CreateClient();

        var patch = CreatePatch<string, MyErrorModel, int>(
            url: "http://test.com".ToAbsoluteUrl(),
            buildRequest: id => new HttpRequestParts(
                RelativeUrl: new RelativeUrl($"/items/{id}"),
                Body: new ProgressReportingHttpContent(
                    id.ToString(System.Globalization.CultureInfo.InvariantCulture)
                ),
                Headers: null
            ),
            deserializeSuccess: TestDeserializer.Deserialize<string>,
            deserializeError: TestDeserializer.Deserialize<MyErrorModel>
        );

        // Act
        var result = await patch(httpClient, 123).ConfigureAwait(false);

        // Assert
        var successValue = +result;

        Assert.AreEqual(expectedContent, successValue);
    }

    [TestMethod]
    public async Task CreatePatch_ErrorResponse_ReturnsFailureResult()
    {
        // Arrange
        var expectedErrorContent = "Patch Failed";
        using var errorResponse = new HttpResponseMessage(HttpStatusCode.BadRequest)
        {
            Content = new StringContent( /*lang=json,strict*/
                "{\"message\":\"Patch Failed\"}"
            ),
        };
        using var httpClient = CreateMockHttpClientFactory(response: errorResponse).CreateClient();

        var patch = CreatePatch<string, MyErrorModel, int>(
            url: "http://test.com".ToAbsoluteUrl(),
            buildRequest: id => new HttpRequestParts(
                RelativeUrl: new RelativeUrl($"/items/{id}"),
                Body: new ProgressReportingHttpContent(
                    id.ToString(System.Globalization.CultureInfo.InvariantCulture)
                ),
                Headers: null
            ),
            deserializeSuccess: TestDeserializer.Deserialize<string>,
            deserializeError: TestDeserializer.Deserialize<MyErrorModel>
        );

        // Act
        using var requestBody = new ProgressReportingHttpContent(
            Encoding.UTF8.GetBytes( /*lang=json,strict*/
                "{\"status\":\"updated\"}"
            )
        );
        var result = await patch(httpClient, 123).ConfigureAwait(false);

        // Assert
        var httpError = !result;

        if (httpError is not ResponseError(var body, var statusCode, var headers))
        {
            throw new InvalidOperationException("Expected error response");
        }

        Assert.AreEqual(HttpStatusCode.BadRequest, statusCode);
        Assert.AreEqual(expectedErrorContent, body.Message);
    }

    [TestMethod]
    public async Task CreateGet_ReturnsSuccessResult()
    {
        // Arrange
        var expectedContent = "Get Success";
        using var response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(expectedContent),
        };
        using var httpClient = CreateMockHttpClientFactory(response: response).CreateClient();

        var get = CreateGet<string, MyErrorModel, int>(
            url: "http://test.com".ToAbsoluteUrl(),
            buildRequest: id => new HttpRequestParts(
                RelativeUrl: new RelativeUrl($"/items/{id}"),
                Body: null,
                Headers: null
            ),
            deserializeSuccess: TestDeserializer.Deserialize<string>,
            deserializeError: TestDeserializer.Deserialize<MyErrorModel>
        );

        // Act
        var result = await get(httpClient, 123).ConfigureAwait(false);

        // Assert
        var successValue = +result;
        Assert.AreEqual(expectedContent, successValue);
    }

    [TestMethod]
    public async Task CreateGet_ErrorResponse_ReturnsFailureResult()
    {
        // Arrange
        var expectedErrorContent = "Get Failed";
        using var errorResponse = new HttpResponseMessage(HttpStatusCode.BadRequest)
        {
            Content = new StringContent( /*lang=json,strict*/
                "{\"message\":\"Get Failed\"}"
            ),
        };
        using var httpClient = CreateMockHttpClientFactory(response: errorResponse).CreateClient();

        var get = CreateGet<string, MyErrorModel, int>(
            url: "http://test.com".ToAbsoluteUrl(),
            buildRequest: id => new HttpRequestParts(
                RelativeUrl: new RelativeUrl($"/items/{id}"),
                Body: null,
                Headers: null
            ),
            deserializeSuccess: TestDeserializer.Deserialize<string>,
            deserializeError: TestDeserializer.Deserialize<MyErrorModel>
        );

        // Act
        var result = await get(httpClient, 123).ConfigureAwait(false);

        // Assert
        var httpError = !result;
        if (httpError is not ResponseError(var body, var statusCode, var headers))
        {
            throw new InvalidOperationException("Expected error response");
        }

        Assert.AreEqual(HttpStatusCode.BadRequest, statusCode);
        Assert.AreEqual(expectedErrorContent, body.Message);
    }

    [TestMethod]
    public async Task CreatePost_ReturnsSuccessResult()
    {
        // Arrange
        var expectedContent = "Post Success";
        using var response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(expectedContent),
        };
        using var httpClient = CreateMockHttpClientFactory(response: response).CreateClient();

        var post = CreatePost<string, MyErrorModel, int>(
            url: "http://test.com".ToAbsoluteUrl(),
            buildRequest: id => new HttpRequestParts(
                RelativeUrl: new RelativeUrl($"/items/{id}"),
                Body: new ProgressReportingHttpContent(
                    id.ToString(System.Globalization.CultureInfo.InvariantCulture) ?? string.Empty
                ),
                Headers: null
            ),
            deserializeSuccess: TestDeserializer.Deserialize<string>,
            deserializeError: TestDeserializer.Deserialize<MyErrorModel>
        );

        // Act
        var result = await post(httpClient, 123).ConfigureAwait(false);

        // Assert
        var successValue = +result;
        Assert.AreEqual(expectedContent, successValue);
    }

    [TestMethod]
    public async Task CreatePost_ErrorResponse_ReturnsFailureResult()
    {
        // Arrange
        var expectedErrorContent = "Post Failed";
        using var errorResponse = new HttpResponseMessage(HttpStatusCode.BadRequest)
        {
            Content = new StringContent( /*lang=json,strict*/
                "{\"message\":\"Post Failed\"}"
            ),
        };
        using var httpClient = CreateMockHttpClientFactory(response: errorResponse).CreateClient();

        var post = CreatePost<string, MyErrorModel, int>(
            url: "http://test.com".ToAbsoluteUrl(),
            buildRequest: id => new HttpRequestParts(
                RelativeUrl: new RelativeUrl($"/items/{id}"),
                Body: new ProgressReportingHttpContent(
                    id.ToString(System.Globalization.CultureInfo.InvariantCulture)
                ),
                Headers: null
            ),
            deserializeSuccess: TestDeserializer.Deserialize<string>,
            deserializeError: TestDeserializer.Deserialize<MyErrorModel>
        );

        // Act
        var result = await post(httpClient, 123).ConfigureAwait(false);

        // Assert
        var httpError = !result;
        if (httpError is not ResponseError(var body, var statusCode, var headers))
        {
            throw new InvalidOperationException("Expected error response");
        }

        Assert.AreEqual(HttpStatusCode.BadRequest, statusCode);
        Assert.AreEqual(expectedErrorContent, body.Message);
    }

    [TestMethod]
    public async Task CreatePut_ReturnsSuccessResult()
    {
        // Arrange
        var expectedContent = "Put Success";
        using var response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(expectedContent),
        };
        using var httpClient = CreateMockHttpClientFactory(response: response).CreateClient();

        var put = CreatePut<string, MyErrorModel, int>(
            url: "http://test.com".ToAbsoluteUrl(),
            buildRequest: id => new HttpRequestParts(
                RelativeUrl: new RelativeUrl($"/items/{id}"),
                Body: new ProgressReportingHttpContent(
                    id.ToString(System.Globalization.CultureInfo.InvariantCulture) ?? string.Empty
                ),
                Headers: null
            ),
            deserializeSuccess: TestDeserializer.Deserialize<string>,
            deserializeError: TestDeserializer.Deserialize<MyErrorModel>
        );

        // Act
        var result = await put(httpClient, 123).ConfigureAwait(false);

        // Assert
        var successValue = +result;
        Assert.AreEqual(expectedContent, successValue);
    }

    [TestMethod]
    public async Task CreatePut_ErrorResponse_ReturnsFailureResult()
    {
        // Arrange
        var expectedErrorContent = "Put Failed";
        using var errorResponse = new HttpResponseMessage(HttpStatusCode.BadRequest)
        {
            Content = new StringContent( /*lang=json,strict*/
                "{\"message\":\"Put Failed\"}"
            ),
        };
        using var httpClient = CreateMockHttpClientFactory(response: errorResponse).CreateClient();

        var put = CreatePut<string, MyErrorModel, int>(
            url: "http://test.com".ToAbsoluteUrl(),
            buildRequest: id => new HttpRequestParts(
                RelativeUrl: new RelativeUrl($"/items/{id}"),
                Body: new ProgressReportingHttpContent(
                    id.ToString(System.Globalization.CultureInfo.InvariantCulture)
                ),
                Headers: null
            ),
            deserializeSuccess: TestDeserializer.Deserialize<string>,
            deserializeError: TestDeserializer.Deserialize<MyErrorModel>
        );

        // Act
        var result = await put(httpClient, 123).ConfigureAwait(false);

        // Assert
        var httpError = !result;
        if (httpError is not ResponseError(var body, var statusCode, var headers))
        {
            throw new InvalidOperationException("Expected error response");
        }

        Assert.AreEqual(HttpStatusCode.BadRequest, statusCode);
        Assert.AreEqual(expectedErrorContent, body.Message);
    }

    [TestMethod]
    public async Task CreateDelete_ReturnsSuccessResult()
    {
        // Arrange
        var expectedContent = "Delete Success";
        using var response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(expectedContent),
        };
        using var httpClient = CreateMockHttpClientFactory(response: response).CreateClient();

        var delete = CreateDelete<string, MyErrorModel, int>(
            url: "http://test.com".ToAbsoluteUrl(),
            buildRequest: id => new HttpRequestParts(
                RelativeUrl: new RelativeUrl($"/items/{id}"),
                Body: null,
                Headers: null
            ),
            deserializeSuccess: TestDeserializer.Deserialize<string>,
            deserializeError: TestDeserializer.Deserialize<MyErrorModel>
        );

        // Act
        var result = await delete(httpClient, 123).ConfigureAwait(false);

        // Assert
        var successValue = +result;
        Assert.AreEqual(expectedContent, successValue);
    }

    [TestMethod]
    public async Task CreateDelete_ErrorResponse_ReturnsFailureResult()
    {
        // Arrange
        var expectedErrorContent = "Delete Failed";
        using var errorResponse = new HttpResponseMessage(HttpStatusCode.BadRequest)
        {
            Content = new StringContent( /*lang=json,strict*/
                "{\"message\":\"Delete Failed\"}"
            ),
        };
        using var httpClient = CreateMockHttpClientFactory(response: errorResponse).CreateClient();

        var delete = CreateDelete<string, MyErrorModel, int>(
            url: "http://test.com".ToAbsoluteUrl(),
            buildRequest: id => new HttpRequestParts(
                RelativeUrl: new RelativeUrl($"/items/{id}"),
                Body: null,
                Headers: null
            ),
            deserializeSuccess: TestDeserializer.Deserialize<string>,
            deserializeError: TestDeserializer.Deserialize<MyErrorModel>
        );

        // Act
        var result = await delete(httpClient, 123).ConfigureAwait(false);

        // Assert
        var httpError = !result;
        if (httpError is not ResponseError(var body, var statusCode, var headers))
        {
            throw new InvalidOperationException("Expected error response");
        }

        Assert.AreEqual(HttpStatusCode.BadRequest, statusCode);
        Assert.AreEqual(expectedErrorContent, body.Message);
    }

    [TestMethod]
    public async Task GetAsync_DeserializationThrowsException_ReturnsFailureResult()
    {
        // Arrange
        using var response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("invalid json"),
        };
        using var httpClient = CreateMockHttpClientFactory(response: response).CreateClient();

        // Act
        var get = CreateGet<string, MyErrorModel, Unit>(
            buildRequest: _ => new HttpRequestParts(
                RelativeUrl: new RelativeUrl("/items"),
                Body: null,
                Headers: null
            ),
            url: "http://test.com".ToAbsoluteUrl(),
            deserializeSuccess: static async (_, _) =>
            {
                await Task.CompletedTask.ConfigureAwait(false);
                throw new InvalidOperationException("Deserialization failed");
            },
            deserializeError: TestDeserializer.Deserialize<MyErrorModel>
        );

        var result = await get(httpClient, Unit.Value).ConfigureAwait(false);

        // Assert
        var exception = !result switch
        {
            ExceptionError(var ex) => ex,
            ResponseError(var b, var sc, var h) => throw new InvalidOperationException(
                "Expected exception error"
            ),
        };

        Assert.IsInstanceOfType<InvalidOperationException>(exception);
        Assert.AreEqual("Deserialization failed", exception.Message);
    }

    [TestMethod]
    public async Task PostAsync_DeserializationThrowsException_ReturnsFailureResult()
    {
        // Arrange
        using var response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("invalid json"),
        };
        var httpClientFactory = CreateMockHttpClientFactory(response: response);

        // Act
        using var requestBody = new ProgressReportingHttpContent(Encoding.UTF8.GetBytes("{}"));
        var result = await httpClientFactory
            .PostAsync<string, MyErrorModel>(
                clientName: "TestClient",
                url: "http://test.com".ToAbsoluteUrl(),
                requestBody: requestBody,
                deserializeSuccess: static async (_, _) =>
                {
                    await Task.CompletedTask.ConfigureAwait(false);
                    throw new InvalidOperationException("Deserialization failed");
                },
                deserializeError: TestDeserializer.Deserialize<MyErrorModel>
            )
            .ConfigureAwait(false);

        // Assert
        var exception = !result switch
        {
            ExceptionError(var ex) => ex,
            ResponseError(var b, var sc, var h) => throw new InvalidOperationException(
                "Expected exception error"
            ),
        };

        Assert.IsInstanceOfType<InvalidOperationException>(exception);
        Assert.AreEqual("Deserialization failed", exception.Message);
    }

    [TestMethod]
    public async Task GetAsync_ErrorDeserializationThrowsException_ReturnsFailureResult()
    {
        // Arrange
        using var response = new HttpResponseMessage(HttpStatusCode.BadRequest)
        {
            Content = new StringContent("invalid error json"),
        };
        var httpClientFactory = CreateMockHttpClientFactory(response: response);

        // Act
        var result = await httpClientFactory
            .GetAsync<string, MyErrorModel>(
                clientName: "TestClient",
                url: "http://test.com".ToAbsoluteUrl(),
                deserializeSuccess: TestDeserializer.Deserialize<string>,
                deserializeError: static async (_, _) =>
                {
                    await Task.CompletedTask.ConfigureAwait(false);
                    throw new InvalidOperationException("Error deserialization failed");
                }
            )
            .ConfigureAwait(false);

        // Assert
        var exception = !result switch
        {
            ExceptionError(var ex) => ex,
            ResponseError(var b, var sc, var h) => throw new InvalidOperationException(
                "Expected exception error"
            ),
        };

        Assert.IsInstanceOfType<InvalidOperationException>(exception);
        Assert.AreEqual("Error deserialization failed", exception.Message);
    }

    [TestMethod]
    public async Task CreateGet_CancellationToken_CancelsRequest()
    {
        // Arrange
        using var cts = new CancellationTokenSource();
        using var httpClient = CreateMockHttpClientFactory(
                exceptionToThrow: new TaskCanceledException()
            )
            .CreateClient();

        var get = CreateGet<string, MyErrorModel, int>(
            url: "http://test.com".ToAbsoluteUrl(),
            buildRequest: id => new HttpRequestParts(
                RelativeUrl: new RelativeUrl($"/items/{id}"),
                Body: null,
                Headers: null
            ),
            deserializeSuccess: TestDeserializer.Deserialize<string>,
            deserializeError: TestDeserializer.Deserialize<MyErrorModel>
        );

        await cts.CancelAsync().ConfigureAwait(false);

        // Act
        var result = await get(httpClient, 123, cts.Token).ConfigureAwait(false);

        // Assert
        var exception = !result switch
        {
            ExceptionError(var ex) => ex,
            ResponseError(var b, var sc, var h) => throw new InvalidOperationException(
                "Expected exception error"
            ),
        };

        Assert.IsInstanceOfType<TaskCanceledException>(exception);
    }

    [TestMethod]
    public async Task CreatePost_CancellationToken_CancelsRequest()
    {
        // Arrange
        using var cts = new CancellationTokenSource();
        using var httpClient = CreateMockHttpClientFactory(
                exceptionToThrow: new TaskCanceledException()
            )
            .CreateClient();

        var post = CreatePost<string, MyErrorModel, int>(
            url: "http://test.com".ToAbsoluteUrl(),
            buildRequest: id => new HttpRequestParts(
                RelativeUrl: new RelativeUrl($"/items/{id}"),
                Body: new ProgressReportingHttpContent(
                    id.ToString(System.Globalization.CultureInfo.InvariantCulture) ?? string.Empty
                ),
                Headers: null
            ),
            deserializeSuccess: TestDeserializer.Deserialize<string>,
            deserializeError: TestDeserializer.Deserialize<MyErrorModel>
        );

        await cts.CancelAsync().ConfigureAwait(false);

        // Act
        var result = await post(httpClient, 123, cts.Token).ConfigureAwait(false);

        // Assert
        var exception = !result switch
        {
            ExceptionError(var ex) => ex,
            ResponseError(var b, var sc, var h) => throw new InvalidOperationException(
                "Expected exception error"
            ),
        };

        Assert.IsInstanceOfType<TaskCanceledException>(exception);
    }

    [TestMethod]
    public async Task CreatePut_CancellationToken_CancelsRequest()
    {
        // Arrange
        using var cts = new CancellationTokenSource();
        using var httpClient = CreateMockHttpClientFactory(
                exceptionToThrow: new TaskCanceledException()
            )
            .CreateClient();

        var put = CreatePut<string, MyErrorModel, int>(
            url: "http://test.com".ToAbsoluteUrl(),
            buildRequest: id => new HttpRequestParts(
                RelativeUrl: new RelativeUrl($"/items/{id}"),
                Body: new ProgressReportingHttpContent(
                    id.ToString(System.Globalization.CultureInfo.InvariantCulture) ?? string.Empty
                ),
                Headers: null
            ),
            deserializeSuccess: TestDeserializer.Deserialize<string>,
            deserializeError: TestDeserializer.Deserialize<MyErrorModel>
        );

        await cts.CancelAsync().ConfigureAwait(false);

        // Act
        var result = await put(httpClient, 123, cts.Token).ConfigureAwait(false);

        // Assert
        var exception = !result switch
        {
            ExceptionError(var ex) => ex,
            ResponseError(var b, var sc, var h) => throw new InvalidOperationException(
                "Expected exception error"
            ),
        };

        Assert.IsInstanceOfType<TaskCanceledException>(exception);
    }

    [TestMethod]
    public async Task CreateDelete_CancellationToken_CancelsRequest()
    {
        // Arrange
        using var cts = new CancellationTokenSource();
        using var httpClient = CreateMockHttpClientFactory(
                exceptionToThrow: new TaskCanceledException()
            )
            .CreateClient();

        var delete = CreateDelete<string, MyErrorModel, int>(
            url: "http://test.com".ToAbsoluteUrl(),
            buildRequest: id => new HttpRequestParts(
                RelativeUrl: new RelativeUrl($"/items/{id}"),
                Body: null,
                Headers: null
            ),
            deserializeSuccess: TestDeserializer.Deserialize<string>,
            deserializeError: TestDeserializer.Deserialize<MyErrorModel>
        );

        await cts.CancelAsync().ConfigureAwait(false);

        // Act
        var result = await delete(httpClient, 123, cts.Token).ConfigureAwait(false);

        // Assert
        var exception = !result switch
        {
            ExceptionError(var ex) => ex,
            ResponseError(var b, var sc, var h) => throw new InvalidOperationException(
                "Expected exception error"
            ),
        };

        Assert.IsInstanceOfType<TaskCanceledException>(exception);
    }

    [TestMethod]
    public async Task CreatePatch_CancellationToken_CancelsRequest()
    {
        // Arrange
        using var cts = new CancellationTokenSource();
        using var httpClient = CreateMockHttpClientFactory(
                exceptionToThrow: new TaskCanceledException()
            )
            .CreateClient();

        var patch = CreatePatch<string, MyErrorModel, int>(
            url: "http://test.com".ToAbsoluteUrl(),
            buildRequest: id => new HttpRequestParts(
                RelativeUrl: new RelativeUrl($"/items/{id}"),
                Body: new ProgressReportingHttpContent(
                    id.ToString(System.Globalization.CultureInfo.InvariantCulture)
                ),
                Headers: null
            ),
            deserializeSuccess: TestDeserializer.Deserialize<string>,
            deserializeError: TestDeserializer.Deserialize<MyErrorModel>
        );

        await cts.CancelAsync().ConfigureAwait(false);

        // Act
        var result = await patch(httpClient, 123, cts.Token).ConfigureAwait(false);

        // Assert
        var exception = !result switch
        {
            ExceptionError(var ex) => ex,
            ResponseError(var b, var sc, var h) => throw new InvalidOperationException(
                "Expected exception error"
            ),
        };

        Assert.IsInstanceOfType<TaskCanceledException>(exception);
    }

    [TestMethod]
    public async Task CreateHead_ReturnsSuccessResult()
    {
        // Arrange
        var expectedContent = "Head Success";
        using var response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(expectedContent),
        };
        using var httpClient = CreateMockHttpClientFactory(response: response).CreateClient();

        var head = CreateHead<string, MyErrorModel, int>(
            url: "http://test.com".ToAbsoluteUrl(),
            buildRequest: id => new HttpRequestParts(
                RelativeUrl: new RelativeUrl($"/items/{id}"),
                Body: null,
                Headers: null
            ),
            deserializeSuccess: TestDeserializer.Deserialize<string>,
            deserializeError: TestDeserializer.Deserialize<MyErrorModel>
        );

        // Act
        var result = await head(httpClient, 123).ConfigureAwait(false);

        // Assert
        var successValue = +result;
        Assert.AreEqual(expectedContent, successValue);
    }

    [TestMethod]
    public async Task CreateHead_ErrorResponse_ReturnsFailureResult()
    {
        // Arrange
        var expectedErrorContent = "Head Failed";
        using var errorResponse = new HttpResponseMessage(HttpStatusCode.BadRequest)
        {
            Content = new StringContent( /*lang=json,strict*/
                "{\"message\":\"Head Failed\"}"
            ),
        };
        using var httpClient = CreateMockHttpClientFactory(response: errorResponse).CreateClient();

        var head = CreateHead<string, MyErrorModel, int>(
            url: "http://test.com".ToAbsoluteUrl(),
            buildRequest: id => new HttpRequestParts(
                RelativeUrl: new RelativeUrl($"/items/{id}"),
                Body: null,
                Headers: null
            ),
            deserializeSuccess: TestDeserializer.Deserialize<string>,
            deserializeError: TestDeserializer.Deserialize<MyErrorModel>
        );

        // Act
        var result = await head(httpClient, 123).ConfigureAwait(false);

        // Assert
        var httpError = !result;
        if (httpError is not ResponseError(var body, var statusCode, var headers))
        {
            throw new InvalidOperationException("Expected error response");
        }

        Assert.AreEqual(HttpStatusCode.BadRequest, statusCode);
        Assert.AreEqual(expectedErrorContent, body.Message);
    }

    [TestMethod]
    public async Task CreateHead_CancellationToken_CancelsRequest()
    {
        // Arrange
        using var cts = new CancellationTokenSource();
        using var httpClient = CreateMockHttpClientFactory(
                exceptionToThrow: new TaskCanceledException()
            )
            .CreateClient();

        var head = CreateHead<string, MyErrorModel, int>(
            url: "http://test.com".ToAbsoluteUrl(),
            buildRequest: id => new HttpRequestParts(
                RelativeUrl: new RelativeUrl($"/items/{id}"),
                Body: null,
                Headers: null
            ),
            deserializeSuccess: TestDeserializer.Deserialize<string>,
            deserializeError: TestDeserializer.Deserialize<MyErrorModel>
        );

        await cts.CancelAsync().ConfigureAwait(false);

        // Act
        var result = await head(httpClient, 123, cts.Token).ConfigureAwait(false);

        // Assert
        var exception = !result switch
        {
            ExceptionError(var ex) => ex,
            ResponseError(var b, var sc, var h) => throw new InvalidOperationException(
                "Expected exception error"
            ),
        };

        Assert.IsInstanceOfType<TaskCanceledException>(exception);
    }

    [TestMethod]
    public async Task CreateOptions_ReturnsSuccessResult()
    {
        // Arrange
        var expectedContent = "Options Success";
        using var response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(expectedContent),
        };
        using var httpClient = CreateMockHttpClientFactory(response: response).CreateClient();

        var options = CreateOptions<string, MyErrorModel, int>(
            url: "http://test.com".ToAbsoluteUrl(),
            buildRequest: id => new HttpRequestParts(
                RelativeUrl: new RelativeUrl($"/items/{id}"),
                Body: null,
                Headers: null
            ),
            deserializeSuccess: TestDeserializer.Deserialize<string>,
            deserializeError: TestDeserializer.Deserialize<MyErrorModel>
        );

        // Act
        var result = await options(httpClient, 123).ConfigureAwait(false);

        // Assert
        var successValue = +result;
        Assert.AreEqual(expectedContent, successValue);
    }

    [TestMethod]
    public async Task CreateOptions_ErrorResponse_ReturnsFailureResult()
    {
        // Arrange
        var expectedErrorContent = "Options Failed";
        using var errorResponse = new HttpResponseMessage(HttpStatusCode.BadRequest)
        {
            Content = new StringContent( /*lang=json,strict*/
                "{\"message\":\"Options Failed\"}"
            ),
        };
        using var httpClient = CreateMockHttpClientFactory(response: errorResponse).CreateClient();

        var options = CreateOptions<string, MyErrorModel, int>(
            url: "http://test.com".ToAbsoluteUrl(),
            buildRequest: id => new HttpRequestParts(
                RelativeUrl: new RelativeUrl($"/items/{id}"),
                Body: null,
                Headers: null
            ),
            deserializeSuccess: TestDeserializer.Deserialize<string>,
            deserializeError: TestDeserializer.Deserialize<MyErrorModel>
        );

        // Act
        var result = await options(httpClient, 123).ConfigureAwait(false);

        // Assert
        var httpError = !result;
        if (httpError is not ResponseError(var body, var statusCode, var headers))
        {
            throw new InvalidOperationException("Expected error response");
        }

        Assert.AreEqual(HttpStatusCode.BadRequest, statusCode);
        Assert.AreEqual(expectedErrorContent, body.Message);
    }

    [TestMethod]
    public async Task CreateOptions_CancellationToken_CancelsRequest()
    {
        // Arrange
        using var cts = new CancellationTokenSource();
        using var httpClient = CreateMockHttpClientFactory(
                exceptionToThrow: new TaskCanceledException()
            )
            .CreateClient();

        var options = CreateOptions<string, MyErrorModel, int>(
            url: "http://test.com".ToAbsoluteUrl(),
            buildRequest: id => new HttpRequestParts(
                RelativeUrl: new RelativeUrl($"/items/{id}"),
                Body: null,
                Headers: null
            ),
            deserializeSuccess: TestDeserializer.Deserialize<string>,
            deserializeError: TestDeserializer.Deserialize<MyErrorModel>
        );

        await cts.CancelAsync().ConfigureAwait(false);

        // Act
        var result = await options(httpClient, 123, cts.Token).ConfigureAwait(false);

        // Assert
        var exception = !result switch
        {
            ExceptionError(var ex) => ex,
            ResponseError(var b, var sc, var h) => throw new InvalidOperationException(
                "Expected exception error"
            ),
        };

        Assert.IsInstanceOfType<TaskCanceledException>(exception);
    }
}
