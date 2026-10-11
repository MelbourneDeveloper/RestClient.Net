using System.Net;
using System.Text;
using System.Text.Json;
using Outcome;

namespace WebsiteExamples;

internal static class ExampleTests
{
    private static int assertions;

    public static async Task Main()
    {
        using var handler = new LocalHandler();
        using var client = new HttpClient(handler);
        var success = await RestClientExamples.GetPostAsync(client, 7);
        Check(success.IsOk && !success.IsError, "GET returns a success");
        Check(RestClientExamples.Describe(success) == "Post 7: Local post", "success display");
        Check(handler.LastMethod == HttpMethod.Get, "GET method");
        Check(
            handler.LastUri == "https://jsonplaceholder.typicode.com/posts/7",
            "absolute GET URL"
        );
        Check(handler.Accept == "application/json", "request headers");
        success.Tap(post =>
            Check(post.UserId == 3 && post.Body == "Local body", "typed response fields")
        );

        handler.Status = HttpStatusCode.NotFound;
        var missing = await RestClientExamples.GetPostAsync(client, 404);
        Check(missing.IsError && !missing.IsOk, "HTTP error is a failure result");
        Check(
            RestClientExamples.Describe(missing) == "HTTP 404: Missing post",
            "HTTP error retains status and body"
        );
        missing.Tap(onError: error =>
        {
            Check(error.IsErrorResponse && !error.IsExceptionError, "HTTP error classification");
            _ = error.Match(
                onException: _ => throw new InvalidOperationException("Unexpected network error"),
                onErrorResponse: (body, status, headers) =>
                {
                    Check(
                        body == "Missing post" && status == HttpStatusCode.NotFound,
                        "structured HTTP error"
                    );
                    Check(
                        headers.GetValues("X-Test").Single() == "local",
                        "response headers preserved"
                    );
                    return body;
                }
            );
        });

        handler.Failure = new HttpRequestException("Offline fixture");
        var offline = await RestClientExamples.GetPostAsync(client, 1);
        Check(offline.IsError, "network exceptions become failure results");
        Check(
            RestClientExamples.Describe(offline) == "Connection failed: Offline fixture",
            "network error display"
        );
        offline.Tap(onError: error =>
            Check(error.IsExceptionError && !error.IsErrorResponse, "network error classification")
        );

        handler.Failure = null;
        handler.Status = HttpStatusCode.Created;
        var created = await RestClientExamples.CreatePostAsync(
            client,
            new CreatePost(3, "A new post", "New body")
        );
        Check(created.IsOk, "POST returns success");
        Check(handler.LastMethod == HttpMethod.Post, "POST method");
        Check(handler.LastUri == "https://jsonplaceholder.typicode.com/posts", "absolute POST URL");
        Check(
            handler.ContentType == "application/json; charset=utf-8",
            "JSON request content type"
        );
        using var body = JsonDocument.Parse(handler.Body!);
        Check(
            body.RootElement.GetProperty("title").GetString() == "A new post",
            "POST title serialized"
        );
        Check(
            body.RootElement.GetProperty("body").GetString() == "New body",
            "POST body serialized"
        );
        Check(body.RootElement.GetProperty("userId").GetInt32() == 3, "POST user ID serialized");

        var factory = new LocalFactory(client);
        var factoryResult = await RestClientExamples.GetUsingFactoryAsync(factory, 19);
        Check(factoryResult.IsOk, "factory request succeeds");
        Check(factory.Name == "posts", "named factory client selected");
        Check(
            handler.LastUri!.EndsWith("/posts/19", StringComparison.Ordinal),
            "factory receives absolute request URL"
        );

        using var cancelled = new CancellationTokenSource();
        cancelled.Cancel();
        var cancellation = await RestClientExamples.GetPostAsync(client, 1, cancelled.Token);
        Check(cancellation.IsError, "cancellation is explicit");
        cancellation.Tap(onError: error =>
        {
            Check(error.IsExceptionError, "cancellation classification");
            _ = error.Match(
                onException: exception =>
                {
                    Check(
                        exception is OperationCanceledException,
                        "original cancellation exception retained"
                    );
                    return "cancelled";
                },
                onErrorResponse: (_, _, _) =>
                    throw new InvalidOperationException("Unexpected HTTP response")
            );
        });
        Check(handler.Calls == 6, "all live calls were intercepted by the local handler");
        Console.WriteLine($"PASS: {assertions} example assertions; no network requests.");
    }

    private static void Check(bool condition, string description)
    {
        if (!condition)
            throw new InvalidOperationException(description);
        assertions++;
    }

    private sealed class LocalFactory(HttpClient client) : IHttpClientFactory
    {
        public string? Name { get; private set; }

        public HttpClient CreateClient(string name)
        {
            Name = name;
            return client;
        }
    }

    private sealed class LocalHandler : HttpMessageHandler
    {
        public HttpStatusCode Status { get; set; } = HttpStatusCode.OK;
        public Exception? Failure { get; set; }
        public int Calls { get; private set; }
        public HttpMethod? LastMethod { get; private set; }
        public string? LastUri { get; private set; }
        public string? Accept { get; private set; }
        public string? Body { get; private set; }
        public string? ContentType { get; private set; }

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken
        )
        {
            Calls++;
            LastMethod = request.Method;
            LastUri = request.RequestUri!.AbsoluteUri;
            Accept = string.Join(",", request.Headers.Accept);
            cancellationToken.ThrowIfCancellationRequested();
            if (Failure is not null)
                throw Failure;
            Body = request.Content is null
                ? null
                : await request.Content.ReadAsStringAsync(cancellationToken);
            ContentType = request.Content?.Headers.ContentType?.ToString();
            var response = new HttpResponseMessage(Status)
            {
                Content = new StringContent(
                    Status == HttpStatusCode.NotFound
                        ? "Missing post"
                        : "{\"userId\":3,\"id\":7,\"title\":\"Local post\",\"body\":\"Local body\"}",
                    Encoding.UTF8,
                    "application/json"
                ),
            };
            response.Headers.Add("X-Test", "local");
            return response;
        }
    }
}
