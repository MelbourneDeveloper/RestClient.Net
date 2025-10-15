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
public sealed class HttpClientExtensionsTests
{
    [TestMethod]
    [DataRow(null, DisplayName = "Null headers")]
    [DataRow("", DisplayName = "Empty headers")]
    [DataRow("X-Custom-Header=TestValue", DisplayName = "Single header")]
    [DataRow(
        "X-Custom-Header=TestValue,Authorization=Bearer token123",
        DisplayName = "Multiple headers"
    )]
    public async Task PatchAsync_ReturnsSuccessResult(string? headerString)
    {
        // Arrange
        var expectedContent = "Patch Success";
        using var response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(expectedContent),
        };

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
        var httpClient = CreateMockHttpClientFactory(
                response: response,
                onRequestSent: req => capturedRequest = req
            )
            .CreateClient("test");

        // Act
        using var requestBody = new ProgressReportingHttpContent(
            Encoding.UTF8.GetBytes( /*lang=json,strict*/
                "{\"status\":\"updated\"}"
            )
        );
        var result = await httpClient
            .PatchAsync(
                url: "http://test.com/resource/1".ToAbsoluteUrl(),
                requestBody: requestBody,
                deserializeSuccess: TestDeserializer.Deserialize<string>,
                deserializeError: TestDeserializer.Deserialize<MyErrorModel>,
                headers: headers
            )
            .ConfigureAwait(false);

        // Assert
        Assert.AreEqual(result.GetValueOrThrow(), expectedContent);

        // Verify headers were sent correctly
        Assert.IsNotNull(capturedRequest, "Request should be captured");
        var expectedCount = headers?.Count ?? 0;

        if (expectedCount > 0)
        {
            // Verify each expected header is present with correct value
            foreach (var kvp in headers!)
            {
                Assert.IsTrue(
                    capturedRequest.Headers.Contains(kvp.Key),
                    $"Header {kvp.Key} should be present"
                );
                var actualValue = capturedRequest.Headers.GetValues(kvp.Key).First();
                Assert.AreEqual(kvp.Value, actualValue, $"Header {kvp.Key} value should match");
            }
        }
        else
        {
            // When null or empty, verify NO application headers were added
            var customHeaders = capturedRequest
                .Headers.Where(h =>
                    h.Key.StartsWith("X-", StringComparison.Ordinal)
                    || h.Key.Equals("Authorization", StringComparison.Ordinal)
                )
                .ToList();
            Assert.AreEqual(
                0,
                customHeaders.Count,
                "No custom headers should be added when input is null/empty"
            );
        }
    }

    [TestMethod]
    public async Task PatchAsync_ErrorResponse_ReturnsFailureResult()
    {
        // Arrange
        var expectedErrorContent = "Patch Failed";
        using var errorResponse = new HttpResponseMessage(HttpStatusCode.BadRequest)
        {
            Content = new StringContent( /*lang=json,strict*/
                "{\"message\":\"Patch Failed\"}"
            ),
        };
        var httpClient = CreateMockHttpClientFactory(response: errorResponse).CreateClient("test");

        // Act
        using var requestBody = new ProgressReportingHttpContent(
            Encoding.UTF8.GetBytes( /*lang=json,strict*/
                "{\"status\":\"updated\"}"
            )
        );
        var result = await httpClient
            .PatchAsync(
                url: "http://test.com/resource/1".ToAbsoluteUrl(),
                requestBody: requestBody,
                deserializeSuccess: TestDeserializer.Deserialize<string>,
                deserializeError: TestDeserializer.Deserialize<MyErrorModel>
            )
            .ConfigureAwait(false);

        // Assert
        var responseError = (ResponseError)!result;

        Assert.AreEqual(HttpStatusCode.BadRequest, responseError.StatusCode);
        Assert.AreEqual(expectedErrorContent, responseError.Body.Message);
    }

    [TestMethod]
    [DataRow(null, DisplayName = "Null headers")]
    [DataRow("", DisplayName = "Empty headers")]
    [DataRow("X-Api-Key=secret123", DisplayName = "Single header")]
    [DataRow("X-Api-Key=secret123,Authorization=Bearer token456", DisplayName = "Multiple headers")]
    public async Task DownloadFileAsync_SuccessfullyDownloadsFile(string? headerString)
    {
        // Arrange
        var content = "This is a test file content";
        using var response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(content),
        };
        response.Content.Headers.ContentLength = content.Length;

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
        var httpClient = CreateMockHttpClientFactory(
                response,
                chunkedData: content,
                onRequestSent: req => capturedRequest = req
            )
            .CreateClient("test");
        var destinationStream = new MemoryStream();

        // Act
        var result = await httpClient
            .DownloadFileAsync(
                url: "https://example.com/file".ToAbsoluteUrl(),
                destinationStream: destinationStream,
                deserializeError: static (_, _) => Task.FromResult("Error"),
                headers: headers,
                cancellationToken: CancellationToken.None
            )
            .ConfigureAwait(false);

        // Assert
        destinationStream.Position = 0;
        using var reader = new StreamReader(destinationStream);
        var downloadedContent = await reader.ReadToEndAsync().ConfigureAwait(false);
        Assert.AreEqual(content, downloadedContent);

        // Verify headers were sent correctly
        Assert.IsNotNull(capturedRequest, "Request should be captured");
        var expectedCount = headers?.Count ?? 0;

        if (expectedCount > 0)
        {
            // Verify each expected header is present with correct value
            foreach (var kvp in headers!)
            {
                Assert.IsTrue(
                    capturedRequest.Headers.Contains(kvp.Key),
                    $"Header {kvp.Key} should be present"
                );
                var actualValue = capturedRequest.Headers.GetValues(kvp.Key).First();
                Assert.AreEqual(kvp.Value, actualValue, $"Header {kvp.Key} value should match");
            }
        }
        else
        {
            // When null or empty, verify NO application headers were added
            var customHeaders = capturedRequest
                .Headers.Where(h =>
                    h.Key.StartsWith("X-", StringComparison.Ordinal)
                    || h.Key.Equals("Authorization", StringComparison.Ordinal)
                )
                .ToList();
            Assert.AreEqual(
                0,
                customHeaders.Count,
                "No custom headers should be added when input is null/empty"
            );
        }
    }

    [TestMethod]
    public async Task DownloadFileAsync_ErrorResponse_ReturnsFailureResult()
    {
        // Arrange
        using var errorResponse = new HttpResponseMessage(HttpStatusCode.NotFound)
        {
            Content = new StringContent("File not found"),
        };
        var httpClient = CreateMockHttpClientFactory(errorResponse).CreateClient("test");
        var destinationStream = new MemoryStream();

        // Act
        var result = await httpClient
            .DownloadFileAsync(
                url: "https://example.com/nonexistent".ToAbsoluteUrl(),
                destinationStream: destinationStream,
                deserializeError: static async (response, ct) =>
                    await response.Content.ReadAsStringAsync(ct).ConfigureAwait(false),
                cancellationToken: CancellationToken.None
            )
            .ConfigureAwait(false);

        // Assert
        var (body, statusCode) = result switch
        {
            OkUnit(var a) => throw new InvalidOperationException("Expected error result"),
            ErrorUnit(ResponseErrorString(var b, var sc, _)) => (b, sc),
            ErrorUnit(ExceptionErrorString(var ex)) => throw new InvalidOperationException(
                "Expected error response",
                ex
            ),
        };

        Assert.AreEqual(HttpStatusCode.NotFound, statusCode);
        Assert.AreEqual("File not found", body);
    }

    [TestMethod]
    [DataRow(null, DisplayName = "Null headers")]
    [DataRow("", DisplayName = "Empty headers")]
    [DataRow("X-Upload-Token=abc123", DisplayName = "Single header")]
    [DataRow("X-Upload-Token=abc123,X-Client-Id=client456", DisplayName = "Multiple headers")]
    public async Task UploadFileAsync_SuccessfullyUploadsFile(string? headerString)
    {
        // Arrange
        var uploadContent = "This is a test upload content";
        using var response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("Upload Successful"),
        };

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
        var httpClient = CreateMockHttpClientFactory(
                response: response,
                onRequestSent: req => capturedRequest = req
            )
            .CreateClient("test");

        // Act
        using var fileStream = new ProgressReportingHttpContent(
            uploadContent,
            progress: static (c, t) => { }
        );
        var result = await httpClient
            .UploadFileAsync(
                url: "http://test.com/upload".ToAbsoluteUrl(),
                requestBody: fileStream,
                deserializeSuccess: static async (response, ct) =>
                    await response.Content.ReadAsStringAsync(ct).ConfigureAwait(false),
                deserializeError: static async (response, ct) =>
                    await response.Content.ReadAsStringAsync(ct).ConfigureAwait(false),
                headers: headers
            )
            .ConfigureAwait(false);

        // Assert
        Assert.AreEqual("Upload Successful", result.GetValueOrThrow());

        // Verify headers were sent correctly
        Assert.IsNotNull(capturedRequest, "Request should be captured");
        var expectedCount = headers?.Count ?? 0;

        if (expectedCount > 0)
        {
            // Verify each expected header is present with correct value
            foreach (var kvp in headers!)
            {
                Assert.IsTrue(
                    capturedRequest.Headers.Contains(kvp.Key),
                    $"Header {kvp.Key} should be present"
                );
                var actualValue = capturedRequest.Headers.GetValues(kvp.Key).First();
                Assert.AreEqual(kvp.Value, actualValue, $"Header {kvp.Key} value should match");
            }
        }
        else
        {
            // When null or empty, verify NO application headers were added
            var customHeaders = capturedRequest
                .Headers.Where(h =>
                    h.Key.StartsWith("X-", StringComparison.Ordinal)
                    || h.Key.Equals("Authorization", StringComparison.Ordinal)
                )
                .ToList();
            Assert.AreEqual(
                0,
                customHeaders.Count,
                "No custom headers should be added when input is null/empty"
            );
        }
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
        var httpClient = CreateMockHttpClientFactory(response: response).CreateClient("test");

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
        Assert.AreEqual(expectedContent, result.GetValueOrThrow());
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
        var httpClient = CreateMockHttpClientFactory(response: errorResponse).CreateClient("test");

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
        var responseError = (ResponseError)!result;
        Assert.AreEqual(HttpStatusCode.BadRequest, responseError.StatusCode);
        Assert.AreEqual(expectedErrorContent, responseError.Body.Message);
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
        var httpClient = CreateMockHttpClientFactory(response: response).CreateClient("test");

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
        Assert.AreEqual(expectedContent, result.GetValueOrThrow());
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
        var httpClient = CreateMockHttpClientFactory(response: errorResponse).CreateClient("test");

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
        var responseError = (ResponseError)!result;
        Assert.AreEqual(HttpStatusCode.BadRequest, responseError.StatusCode);
        Assert.AreEqual(expectedErrorContent, responseError.Body.Message);
    }

    [TestMethod]
    [DataRow(null, DisplayName = "Null headers")]
    [DataRow("", DisplayName = "Empty headers")]
    [DataRow("X-Api-Key", DisplayName = "Single header")]
    [DataRow("X-Api-Key,X-Request-Id=put-123", DisplayName = "Multiple headers")]
    public async Task CreatePut_ReturnsSuccessResult(string? headerKeys)
    {
        // Arrange
        var expectedContent = "Put Success";
        using var response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(expectedContent),
        };

        HttpRequestMessage? capturedRequest = null;
        var httpClient = CreateMockHttpClientFactory(
                response: response,
                onRequestSent: req => capturedRequest = req
            )
            .CreateClient("test");

        var put = CreatePut<string, MyErrorModel, (int Id, string ApiKey)>(
            url: "http://test.com".ToAbsoluteUrl(),
            buildRequest: param =>
            {
                Dictionary<string, string>? headers = headerKeys switch
                {
                    null => null,
                    "" => [],
                    _ => headerKeys
                        .Split(',')
                        .Select(h =>
                        {
                            var parts = h.Split('=');
                            return parts.Length == 1
                                ? (Key: parts[0], Value: param.ApiKey)
                                : (Key: parts[0], Value: parts[1]);
                        })
                        .ToDictionary(p => p.Key, p => p.Value),
                };
                return new HttpRequestParts(
                    RelativeUrl: new RelativeUrl($"/items/{param.Id}"),
                    Body: new ProgressReportingHttpContent(
                        param.Id.ToString(System.Globalization.CultureInfo.InvariantCulture)
                    ),
                    Headers: headers
                );
            },
            deserializeSuccess: TestDeserializer.Deserialize<string>,
            deserializeError: TestDeserializer.Deserialize<MyErrorModel>
        );

        // Act
        var result = await put(httpClient, (123, "test-api-key")).ConfigureAwait(false);

        // Assert
        Assert.AreEqual(expectedContent, result.GetValueOrThrow());

        // Verify headers were sent correctly
        Assert.IsNotNull(capturedRequest, "Request should be captured");
        var expectedCount = headerKeys switch
        {
            null => 0,
            "" => 0,
            _ => headerKeys.Split(',').Length,
        };

        if (expectedCount > 0)
        {
            // Build expected headers for verification
            var expectedHeaders = headerKeys!
                .Split(',')
                .Select(h =>
                {
                    var parts = h.Split('=');
                    return parts.Length == 1
                        ? (Key: parts[0], Value: "test-api-key")
                        : (Key: parts[0], Value: parts[1]);
                })
                .ToDictionary(p => p.Key, p => p.Value);

            // Verify each expected header is present with correct value
            foreach (var kvp in expectedHeaders)
            {
                Assert.IsTrue(
                    capturedRequest.Headers.Contains(kvp.Key),
                    $"Header {kvp.Key} should be present"
                );
                var actualValue = capturedRequest.Headers.GetValues(kvp.Key).First();
                Assert.AreEqual(kvp.Value, actualValue, $"Header {kvp.Key} value should match");
            }
        }
        else
        {
            // When null or empty, verify NO application headers were added
            var customHeaders = capturedRequest
                .Headers.Where(h =>
                    h.Key.StartsWith("X-", StringComparison.Ordinal)
                    || h.Key.Equals("Authorization", StringComparison.Ordinal)
                )
                .ToList();
            Assert.AreEqual(
                0,
                customHeaders.Count,
                "No custom headers should be added when input is null/empty"
            );
        }
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
        var httpClient = CreateMockHttpClientFactory(response: errorResponse).CreateClient("test");

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
        var responseError = (ResponseError)!result;
        Assert.AreEqual(HttpStatusCode.BadRequest, responseError.StatusCode);
        Assert.AreEqual(expectedErrorContent, responseError.Body.Message);
    }

    [TestMethod]
    [DataRow(null, DisplayName = "Null headers")]
    [DataRow("", DisplayName = "Empty headers")]
    [DataRow("X-Api-Key", DisplayName = "Single header")]
    [DataRow("X-Api-Key,X-Delete-Token=del-789", DisplayName = "Multiple headers")]
    public async Task CreateDelete_ReturnsSuccessResult(string? headerKeys)
    {
        // Arrange
        var expectedContent = "Delete Success";
        using var response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(expectedContent),
        };

        HttpRequestMessage? capturedRequest = null;
        var httpClient = CreateMockHttpClientFactory(
                response: response,
                onRequestSent: req => capturedRequest = req
            )
            .CreateClient("test");

        var delete = CreateDelete<string, MyErrorModel, (int Id, string ApiKey)>(
            url: "http://test.com".ToAbsoluteUrl(),
            buildRequest: param =>
            {
                Dictionary<string, string>? headers = headerKeys switch
                {
                    null => null,
                    "" => [],
                    _ => headerKeys
                        .Split(',')
                        .Select(h =>
                        {
                            var parts = h.Split('=');
                            return parts.Length == 1
                                ? (Key: parts[0], Value: param.ApiKey)
                                : (Key: parts[0], Value: parts[1]);
                        })
                        .ToDictionary(p => p.Key, p => p.Value),
                };
                return new HttpRequestParts(
                    RelativeUrl: new RelativeUrl($"/items/{param.Id}"),
                    Body: null,
                    Headers: headers
                );
            },
            deserializeSuccess: TestDeserializer.Deserialize<string>,
            deserializeError: TestDeserializer.Deserialize<MyErrorModel>
        );

        // Act
        var result = await delete(httpClient, (123, "test-api-key")).ConfigureAwait(false);

        // Assert
        Assert.AreEqual(expectedContent, result.GetValueOrThrow());

        // Verify headers were sent correctly
        Assert.IsNotNull(capturedRequest, "Request should be captured");
        var expectedCount = headerKeys switch
        {
            null => 0,
            "" => 0,
            _ => headerKeys.Split(',').Length,
        };

        if (expectedCount > 0)
        {
            // Build expected headers for verification
            var expectedHeaders = headerKeys!
                .Split(',')
                .Select(h =>
                {
                    var parts = h.Split('=');
                    return parts.Length == 1
                        ? (Key: parts[0], Value: "test-api-key")
                        : (Key: parts[0], Value: parts[1]);
                })
                .ToDictionary(p => p.Key, p => p.Value);

            // Verify each expected header is present with correct value
            foreach (var kvp in expectedHeaders)
            {
                Assert.IsTrue(
                    capturedRequest.Headers.Contains(kvp.Key),
                    $"Header {kvp.Key} should be present"
                );
                var actualValue = capturedRequest.Headers.GetValues(kvp.Key).First();
                Assert.AreEqual(kvp.Value, actualValue, $"Header {kvp.Key} value should match");
            }
        }
        else
        {
            // When null or empty, verify NO application headers were added
            var customHeaders = capturedRequest
                .Headers.Where(h =>
                    h.Key.StartsWith("X-", StringComparison.Ordinal)
                    || h.Key.Equals("Authorization", StringComparison.Ordinal)
                )
                .ToList();
            Assert.AreEqual(
                0,
                customHeaders.Count,
                "No custom headers should be added when input is null/empty"
            );
        }
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
        var httpClient = CreateMockHttpClientFactory(response: errorResponse).CreateClient("test");

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
        var responseError = (ResponseError)!result;
        Assert.AreEqual(HttpStatusCode.BadRequest, responseError.StatusCode);
        Assert.AreEqual(expectedErrorContent, responseError.Body.Message);
    }

    [TestMethod]
    [DataRow(null, DisplayName = "Null headers")]
    [DataRow("", DisplayName = "Empty headers")]
    [DataRow("X-Api-Key", DisplayName = "Single header")]
    [DataRow("X-Api-Key,X-Patch-Version=v2", DisplayName = "Multiple headers")]
    public async Task CreatePatch_ReturnsSuccessResult(string? headerKeys)
    {
        // Arrange
        var expectedContent = "Patch Success";
        using var response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(expectedContent),
        };

        HttpRequestMessage? capturedRequest = null;
        var httpClient = CreateMockHttpClientFactory(
                response: response,
                onRequestSent: req => capturedRequest = req
            )
            .CreateClient("test");

        var patch = CreatePatch<string, MyErrorModel, (int Id, string ApiKey)>(
            url: "http://test.com".ToAbsoluteUrl(),
            buildRequest: param =>
            {
                Dictionary<string, string>? headers = headerKeys switch
                {
                    null => null,
                    "" => [],
                    _ => headerKeys
                        .Split(',')
                        .Select(h =>
                        {
                            var parts = h.Split('=');
                            return parts.Length == 1
                                ? (Key: parts[0], Value: param.ApiKey)
                                : (Key: parts[0], Value: parts[1]);
                        })
                        .ToDictionary(p => p.Key, p => p.Value),
                };
                return new HttpRequestParts(
                    RelativeUrl: new RelativeUrl($"/items/{param.Id}"),
                    Body: new ProgressReportingHttpContent(
                        param.Id.ToString(System.Globalization.CultureInfo.InvariantCulture)
                    ),
                    Headers: headers
                );
            },
            deserializeSuccess: TestDeserializer.Deserialize<string>,
            deserializeError: TestDeserializer.Deserialize<MyErrorModel>
        );

        // Act
        var result = await patch(httpClient, (123, "test-api-key")).ConfigureAwait(false);

        // Assert
        Assert.AreEqual(expectedContent, result.GetValueOrThrow());

        // Verify headers were sent correctly
        Assert.IsNotNull(capturedRequest, "Request should be captured");
        var expectedCount = headerKeys switch
        {
            null => 0,
            "" => 0,
            _ => headerKeys.Split(',').Length,
        };

        if (expectedCount > 0)
        {
            // Build expected headers for verification
            var expectedHeaders = headerKeys!
                .Split(',')
                .Select(h =>
                {
                    var parts = h.Split('=');
                    return parts.Length == 1
                        ? (Key: parts[0], Value: "test-api-key")
                        : (Key: parts[0], Value: parts[1]);
                })
                .ToDictionary(p => p.Key, p => p.Value);

            // Verify each expected header is present with correct value
            foreach (var kvp in expectedHeaders)
            {
                Assert.IsTrue(
                    capturedRequest.Headers.Contains(kvp.Key),
                    $"Header {kvp.Key} should be present"
                );
                var actualValue = capturedRequest.Headers.GetValues(kvp.Key).First();
                Assert.AreEqual(kvp.Value, actualValue, $"Header {kvp.Key} value should match");
            }
        }
        else
        {
            // When null or empty, verify NO application headers were added
            var customHeaders = capturedRequest
                .Headers.Where(h =>
                    h.Key.StartsWith("X-", StringComparison.Ordinal)
                    || h.Key.Equals("Authorization", StringComparison.Ordinal)
                )
                .ToList();
            Assert.AreEqual(
                0,
                customHeaders.Count,
                "No custom headers should be added when input is null/empty"
            );
        }
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
        var httpClient = CreateMockHttpClientFactory(response: errorResponse).CreateClient("test");

        var patch = CreatePatch<string, MyErrorModel, int>(
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
        var result = await patch(httpClient, 123).ConfigureAwait(false);

        // Assert
        var responseError = (ResponseError)!result;
        Assert.AreEqual(HttpStatusCode.BadRequest, responseError.StatusCode);
        Assert.AreEqual(expectedErrorContent, responseError.Body.Message);
    }

    [TestMethod]
    [DataRow(null, DisplayName = "Null headers")]
    [DataRow("", DisplayName = "Empty headers")]
    [DataRow("X-Api-Key", DisplayName = "Single header")]
    [DataRow("X-Api-Key,X-Custom-Header=CustomValue", DisplayName = "Multiple headers")]
    [DataRow("Authorization=Bearer xyz", DisplayName = "Auth header only")]
    [DataRow(
        "X-Trace-Id=trace-001,X-Request-Id=req-002,X-Api-Key",
        DisplayName = "Three headers mixed"
    )]
    public async Task CreateGet_WithHeaders_SendsHeadersCorrectly(string? headerKeys)
    {
        // Arrange
        var expectedContent = "Get Success with Headers";
        using var response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(expectedContent),
        };

        HttpRequestMessage? capturedRequest = null;
        var httpClient = CreateMockHttpClientFactory(
                response: response,
                onRequestSent: req => capturedRequest = req
            )
            .CreateClient("test");

        var get = CreateGet<string, MyErrorModel, (int Id, string ApiKey)>(
            url: "http://test.com".ToAbsoluteUrl(),
            buildRequest: param =>
            {
                Dictionary<string, string>? headers = headerKeys switch
                {
                    null => null,
                    "" => [],
                    _ => headerKeys
                        .Split(',')
                        .Select(h =>
                        {
                            var parts = h.Split('=');
                            return parts.Length == 1
                                ? (Key: parts[0], Value: param.ApiKey)
                                : (Key: parts[0], Value: parts[1]);
                        })
                        .ToDictionary(p => p.Key, p => p.Value),
                };
                return new HttpRequestParts(
                    RelativeUrl: new RelativeUrl($"/items/{param.Id}"),
                    Body: null,
                    Headers: headers
                );
            },
            deserializeSuccess: TestDeserializer.Deserialize<string>,
            deserializeError: TestDeserializer.Deserialize<MyErrorModel>
        );

        // Act
        var result = await get(httpClient, (123, "test-api-key")).ConfigureAwait(false);

        // Assert
        Assert.AreEqual(expectedContent, result.GetValueOrThrow());

        // Verify headers were sent correctly
        Assert.IsNotNull(capturedRequest, "Request should be captured");
        var expectedCount = headerKeys switch
        {
            null => 0,
            "" => 0,
            _ => headerKeys.Split(',').Length,
        };

        if (expectedCount > 0)
        {
            // Build expected headers for verification
            var expectedHeaders = headerKeys!
                .Split(',')
                .Select(h =>
                {
                    var parts = h.Split('=');
                    return parts.Length == 1
                        ? (Key: parts[0], Value: "test-api-key")
                        : (Key: parts[0], Value: parts[1]);
                })
                .ToDictionary(p => p.Key, p => p.Value);

            // Verify each expected header is present with correct value
            foreach (var kvp in expectedHeaders)
            {
                Assert.IsTrue(
                    capturedRequest.Headers.Contains(kvp.Key),
                    $"Header {kvp.Key} should be present"
                );
                var actualValue = capturedRequest.Headers.GetValues(kvp.Key).First();
                Assert.AreEqual(kvp.Value, actualValue, $"Header {kvp.Key} value should match");
            }
        }
        else
        {
            // When null or empty, verify NO application headers were added
            var customHeaders = capturedRequest
                .Headers.Where(h =>
                    h.Key.StartsWith("X-", StringComparison.Ordinal)
                    || h.Key.Equals("Authorization", StringComparison.Ordinal)
                )
                .ToList();
            Assert.AreEqual(
                0,
                customHeaders.Count,
                "No custom headers should be added when input is null/empty"
            );
        }
    }

    [TestMethod]
    [DataRow(null, DisplayName = "Null headers")]
    [DataRow("", DisplayName = "Empty headers")]
    [DataRow("X-Api-Key", DisplayName = "Single header from param")]
    [DataRow("X-Api-Key,X-Request-Id=12345", DisplayName = "Multiple headers")]
    [DataRow("X-Request-Id=12345,X-Trace-Id=trace-99,X-Api-Key", DisplayName = "Three headers")]
    [DataRow("Authorization=Bearer abc123", DisplayName = "Single static header")]
    [DataRow(
        "X-Custom=val1,X-Other=val2,X-Third=val3,X-Fourth=val4",
        DisplayName = "Four static headers"
    )]
    public async Task CreatePost_WithHeaders_SendsHeadersCorrectly(string? headerKeys)
    {
        // Arrange
        var expectedContent = "Post Success with Headers";
        using var response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(expectedContent),
        };

        HttpRequestMessage? capturedRequest = null;
        var httpClient = CreateMockHttpClientFactory(
                response: response,
                onRequestSent: req => capturedRequest = req
            )
            .CreateClient("test");

        var post = CreatePost<string, MyErrorModel, (int Id, string ApiKey, string Data)>(
            url: "http://test.com".ToAbsoluteUrl(),
            buildRequest: param =>
            {
                Dictionary<string, string>? headers = headerKeys switch
                {
                    null => null,
                    "" => [],
                    _ => headerKeys
                        .Split(',')
                        .Select(h =>
                        {
                            var parts = h.Split('=');
                            return parts.Length == 1
                                ? (Key: parts[0], Value: param.ApiKey)
                                : (Key: parts[0], Value: parts[1]);
                        })
                        .ToDictionary(p => p.Key, p => p.Value),
                };
                return new HttpRequestParts(
                    RelativeUrl: new RelativeUrl($"/items/{param.Id}"),
                    Body: new ProgressReportingHttpContent(param.Data),
                    Headers: headers
                );
            },
            deserializeSuccess: TestDeserializer.Deserialize<string>,
            deserializeError: TestDeserializer.Deserialize<MyErrorModel>
        );

        // Act
        var result = await post(httpClient, (123, "test-api-key", "test data"))
            .ConfigureAwait(false);

        // Assert
        Assert.AreEqual(expectedContent, result.GetValueOrThrow());

        // Verify headers were sent correctly
        Assert.IsNotNull(capturedRequest, "Request should be captured");
        var expectedCount = headerKeys switch
        {
            null => 0,
            "" => 0,
            _ => headerKeys.Split(',').Length,
        };

        if (expectedCount > 0)
        {
            // Build expected headers for verification
            var expectedHeaders = headerKeys!
                .Split(',')
                .Select(h =>
                {
                    var parts = h.Split('=');
                    return parts.Length == 1
                        ? (Key: parts[0], Value: "test-api-key")
                        : (Key: parts[0], Value: parts[1]);
                })
                .ToDictionary(p => p.Key, p => p.Value);

            // Verify each expected header is present with correct value
            foreach (var kvp in expectedHeaders)
            {
                Assert.IsTrue(
                    capturedRequest.Headers.Contains(kvp.Key),
                    $"Header {kvp.Key} should be present"
                );
                var actualValue = capturedRequest.Headers.GetValues(kvp.Key).First();
                Assert.AreEqual(kvp.Value, actualValue, $"Header {kvp.Key} value should match");
            }
        }
        else
        {
            // When null or empty, verify NO application headers were added
            var customHeaders = capturedRequest
                .Headers.Where(h =>
                    h.Key.StartsWith("X-", StringComparison.Ordinal)
                    || h.Key.Equals("Authorization", StringComparison.Ordinal)
                )
                .ToList();
            Assert.AreEqual(
                0,
                customHeaders.Count,
                "No custom headers should be added when input is null/empty"
            );
        }
    }

    [TestMethod]
    [DataRow(null, DisplayName = "Null headers")]
    [DataRow("", DisplayName = "Empty headers")]
    [DataRow("X-Custom-Header=TestValue", DisplayName = "Single header")]
    [DataRow(
        "X-Custom-Header=TestValue,Authorization=Bearer token123",
        DisplayName = "Multiple headers"
    )]
    public async Task GetAsync_WithHeaders_SendsHeadersCorrectly(string? headerString)
    {
        // Arrange
        var expectedContent = "Get Success";
        using var response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(expectedContent),
        };

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
        var httpClient = CreateMockHttpClientFactory(
                response: response,
                onRequestSent: req => capturedRequest = req
            )
            .CreateClient("test");

        // Act
        var result = await httpClient
            .GetAsync(
                url: "http://test.com/resource".ToAbsoluteUrl(),
                deserializeSuccess: TestDeserializer.Deserialize<string>,
                deserializeError: TestDeserializer.Deserialize<MyErrorModel>,
                headers: headers
            )
            .ConfigureAwait(false);

        // Assert
        Assert.AreEqual(expectedContent, result.GetValueOrThrow());

        // Verify headers were sent correctly
        Assert.IsNotNull(capturedRequest, "Request should be captured");
        var expectedCount = headers?.Count ?? 0;

        if (expectedCount > 0)
        {
            // Verify each expected header is present with correct value
            foreach (var kvp in headers!)
            {
                Assert.IsTrue(
                    capturedRequest.Headers.Contains(kvp.Key),
                    $"Header {kvp.Key} should be present"
                );
                var actualValue = capturedRequest.Headers.GetValues(kvp.Key).First();
                Assert.AreEqual(kvp.Value, actualValue, $"Header {kvp.Key} value should match");
            }
        }
        else
        {
            // When null or empty, verify NO application headers were added
            var customHeaders = capturedRequest
                .Headers.Where(h =>
                    h.Key.StartsWith("X-", StringComparison.Ordinal)
                    || h.Key.Equals("Authorization", StringComparison.Ordinal)
                )
                .ToList();
            Assert.AreEqual(
                0,
                customHeaders.Count,
                "No custom headers should be added when input is null/empty"
            );
        }
    }

    [TestMethod]
    [DataRow(null, DisplayName = "Null headers")]
    [DataRow("", DisplayName = "Empty headers")]
    [DataRow("X-Custom-Header=TestValue", DisplayName = "Single header")]
    [DataRow(
        "X-Custom-Header=TestValue,Authorization=Bearer token123",
        DisplayName = "Multiple headers"
    )]
    public async Task PostAsync_WithHeaders_SendsHeadersCorrectly(string? headerString)
    {
        // Arrange
        var expectedContent = "Post Success";
        using var response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(expectedContent),
        };

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
        var httpClient = CreateMockHttpClientFactory(
                response: response,
                onRequestSent: req => capturedRequest = req
            )
            .CreateClient("test");

        // Act
        using var requestBody = new ProgressReportingHttpContent("test data");
        var result = await httpClient
            .PostAsync(
                url: "http://test.com/resource".ToAbsoluteUrl(),
                requestBody: requestBody,
                deserializeSuccess: TestDeserializer.Deserialize<string>,
                deserializeError: TestDeserializer.Deserialize<MyErrorModel>,
                headers: headers
            )
            .ConfigureAwait(false);

        // Assert
        Assert.AreEqual(expectedContent, result.GetValueOrThrow());

        // Verify headers were sent correctly
        Assert.IsNotNull(capturedRequest, "Request should be captured");
        var expectedCount = headers?.Count ?? 0;

        if (expectedCount > 0)
        {
            // Verify each expected header is present with correct value
            foreach (var kvp in headers!)
            {
                Assert.IsTrue(
                    capturedRequest.Headers.Contains(kvp.Key),
                    $"Header {kvp.Key} should be present"
                );
                var actualValue = capturedRequest.Headers.GetValues(kvp.Key).First();
                Assert.AreEqual(kvp.Value, actualValue, $"Header {kvp.Key} value should match");
            }
        }
        else
        {
            // When null or empty, verify NO application headers were added
            var customHeaders = capturedRequest
                .Headers.Where(h =>
                    h.Key.StartsWith("X-", StringComparison.Ordinal)
                    || h.Key.Equals("Authorization", StringComparison.Ordinal)
                )
                .ToList();
            Assert.AreEqual(
                0,
                customHeaders.Count,
                "No custom headers should be added when input is null/empty"
            );
        }
    }

    [TestMethod]
    [TestCategory("Integration")]
    public async Task LiveApi_GetAsync_WithHeaders_VerifiesHeadersSent()
    {
        // Arrange
        using var httpClient = new HttpClient();
        var headers = new Dictionary<string, string>
        {
            ["X-Test-Header"] = "TestHeaderValue",
            ["X-Api-Key"] = "my-secret-key",
        };

        // Act - httpbin.org echoes back request headers
        var result = await httpClient
            .GetAsync(
                url: "https://httpbin.org/headers".ToAbsoluteUrl(),
                deserializeSuccess: async (response, ct) =>
                {
                    var json = await response.Content.ReadAsStringAsync(ct).ConfigureAwait(false);
                    return json;
                },
                deserializeError: async (response, ct) =>
                    await response.Content.ReadAsStringAsync(ct).ConfigureAwait(false),
                headers: headers
            )
            .ConfigureAwait(false);

        // Assert
        var responseJson = result.GetValueOrThrow();
        Assert.IsTrue(
            responseJson.Contains("X-Test-Header", StringComparison.Ordinal),
            "Response should contain X-Test-Header"
        );
        Assert.IsTrue(
            responseJson.Contains("TestHeaderValue", StringComparison.Ordinal),
            "Response should contain the header value"
        );
        Assert.IsTrue(
            responseJson.Contains("X-Api-Key", StringComparison.Ordinal),
            "Response should contain X-Api-Key"
        );
        Assert.IsTrue(
            responseJson.Contains("my-secret-key", StringComparison.Ordinal),
            "Response should contain the API key value"
        );
    }

    [TestMethod]
    public async Task GetAsync_ResponseHeaders_CanBeAccessed()
    {
        // Arrange
        var expectedContent = "Get Success";
        using var response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(expectedContent),
        };
        response.Headers.Add("X-Response-Header", "ResponseValue");
        response.Headers.Add("X-Request-Id", "req-12345");

        var httpClient = CreateMockHttpClientFactory(response: response).CreateClient("test");

        // Act
        var result = await httpClient
            .GetAsync(
                url: "http://test.com/resource".ToAbsoluteUrl(),
                deserializeSuccess: async (resp, ct) =>
                {
                    // Verify we can access response headers
                    var headerValue = resp.Headers.GetValues("X-Response-Header").FirstOrDefault();
                    var requestId = resp.Headers.GetValues("X-Request-Id").FirstOrDefault();
                    var content = await resp.Content.ReadAsStringAsync(ct).ConfigureAwait(false);
                    return $"{content}|{headerValue}|{requestId}";
                },
                deserializeError: TestDeserializer.Deserialize<MyErrorModel>
            )
            .ConfigureAwait(false);

        // Assert
        var resultValue = result.GetValueOrThrow();
        Assert.IsTrue(
            resultValue.Contains("ResponseValue", StringComparison.Ordinal),
            "Should contain response header value"
        );
        Assert.IsTrue(
            resultValue.Contains("req-12345", StringComparison.Ordinal),
            "Should contain request ID from headers"
        );
    }

    [TestMethod]
    public async Task HttpClientExtensions_CreateGet_ReturnsSuccessResult()
    {
        // Arrange
        var expectedContent = "Get Success from HttpClientExtensions";
        using var response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(expectedContent),
        };
        var httpClient = CreateMockHttpClientFactory(response: response).CreateClient("test");

        var get = HttpClientExtensions.CreateGet<string, MyErrorModel, int>(
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
        var result = await get(httpClient, 456).ConfigureAwait(false);

        // Assert
        Assert.AreEqual(expectedContent, result.GetValueOrThrow());
    }

    [TestMethod]
    public async Task HttpClientExtensions_CreatePut_ReturnsSuccessResult()
    {
        // Arrange
        var expectedContent = "Put Success from HttpClientExtensions";
        using var response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(expectedContent),
        };
        var httpClient = CreateMockHttpClientFactory(response: response).CreateClient("test");

        var put = HttpClientExtensions.CreatePut<string, MyErrorModel, int>(
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
        var result = await put(httpClient, 789).ConfigureAwait(false);

        // Assert
        Assert.AreEqual(expectedContent, result.GetValueOrThrow());
    }

    [TestMethod]
    public async Task HttpClientExtensions_CreateDelete_ReturnsSuccessResult()
    {
        // Arrange
        var expectedContent = "Delete Success from HttpClientExtensions";
        using var response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(expectedContent),
        };
        var httpClient = CreateMockHttpClientFactory(response: response).CreateClient("test");

        var delete = HttpClientExtensions.CreateDelete<string, MyErrorModel, int>(
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
        var result = await delete(httpClient, 321).ConfigureAwait(false);

        // Assert
        Assert.AreEqual(expectedContent, result.GetValueOrThrow());
    }

    [TestMethod]
    public async Task HttpClientExtensions_CreatePost_ReturnsSuccessResult()
    {
        // Arrange
        var expectedContent = "Post Success from HttpClientExtensions";
        using var response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(expectedContent),
        };
        var httpClient = CreateMockHttpClientFactory(response: response).CreateClient("test");

        var post = HttpClientExtensions.CreatePost<string, MyErrorModel, int>(
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
        var result = await post(httpClient, 654).ConfigureAwait(false);

        // Assert
        Assert.AreEqual(expectedContent, result.GetValueOrThrow());
    }

    [TestMethod]
    public async Task HttpClientExtensions_CreatePatch_ReturnsSuccessResult()
    {
        // Arrange
        var expectedContent = "Patch Success from HttpClientExtensions";
        using var response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(expectedContent),
        };
        var httpClient = CreateMockHttpClientFactory(response: response).CreateClient("test");

        var patch = HttpClientExtensions.CreatePatch<string, MyErrorModel, int>(
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
        var result = await patch(httpClient, 987).ConfigureAwait(false);

        // Assert
        Assert.AreEqual(expectedContent, result.GetValueOrThrow());
    }

    [TestMethod]
    public async Task SendAsync_WithContentHeader_ReturnsFailure()
    {
        // Arrange
        using var response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("Success"),
        };
        var httpClient = CreateMockHttpClientFactory(response: response).CreateClient("test");

        // Content-Type is a content header, not a request header - this should fail when added to request headers
        var headers = new Dictionary<string, string> { ["Content-Type"] = "application/json" };

        // Act
        var result = await httpClient
            .GetAsync(
                url: "http://test.com/resource".ToAbsoluteUrl(),
                deserializeSuccess: TestDeserializer.Deserialize<string>,
                deserializeError: TestDeserializer.Deserialize<MyErrorModel>,
                headers: headers
            )
            .ConfigureAwait(false);

        // Assert - verify we got an exception error
        var exceptionError = (ExceptionError)!result;
        Assert.IsInstanceOfType<InvalidOperationException>(exceptionError.Exception);
        Assert.IsTrue(
            exceptionError.Exception.Message.Contains("Content-Type", StringComparison.Ordinal),
            "Exception message should mention the Content-Type header that failed to add"
        );
    }
}
