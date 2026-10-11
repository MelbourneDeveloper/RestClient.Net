using System.Net.Http.Json;
using Outcome;
using RestClient.Net;
using Urls;

namespace WebsiteExamples;

public static class RestClientExamples
{
    // This runnable entry point calls JSONPlaceholder. Tests use a local handler instead.
    public static async Task Main()
    {
        using var client = new HttpClient();
        using var cancellation = new CancellationTokenSource(TimeSpan.FromSeconds(15));
        var result = await GetPostAsync(client, 1, cancellation.Token);
        Console.WriteLine(Describe(result));
    }

    public static Task<Result<Post, HttpError<string>>> GetPostAsync(
        HttpClient client,
        int id,
        CancellationToken cancellationToken = default
    ) =>
        client.GetAsync(
            url: $"https://jsonplaceholder.typicode.com/posts/{id}".ToAbsoluteUrl(),
            deserializeSuccess: ReadPostAsync,
            deserializeError: ReadErrorAsync,
            headers: new Dictionary<string, string> { ["Accept"] = "application/json" },
            cancellationToken: cancellationToken
        );

    public static Task<Result<Post, HttpError<string>>> CreatePostAsync(
        HttpClient client,
        CreatePost request,
        CancellationToken cancellationToken = default
    ) =>
        client.PostAsync(
            url: "https://jsonplaceholder.typicode.com/posts".ToAbsoluteUrl(),
            requestBody: JsonContent.Create(request),
            deserializeSuccess: ReadPostAsync,
            deserializeError: ReadErrorAsync,
            cancellationToken: cancellationToken
        );

    // Register a named client with services.AddHttpClient("posts") in your application.
    public static Task<Result<Post, HttpError<string>>> GetUsingFactoryAsync(
        IHttpClientFactory factory,
        int id,
        CancellationToken cancellationToken = default
    ) =>
        factory.GetAsync(
            clientName: "posts",
            url: $"https://jsonplaceholder.typicode.com/posts/{id}".ToAbsoluteUrl(),
            deserializeSuccess: ReadPostAsync,
            deserializeError: ReadErrorAsync,
            cancellationToken: cancellationToken
        );

    // Both success and failure are explicit. HTTP failures retain the response body.
    public static string Describe(Result<Post, HttpError<string>> result) =>
        result.Match(
            onSuccess: post => $"Post {post.Id}: {post.Title}",
            onError: error =>
                error.Match(
                    onException: exception => $"Connection failed: {exception.Message}",
                    onErrorResponse: (body, status, headers) => $"HTTP {(int)status}: {body}"
                )
        );

    // Deserialize<T> receives HttpResponseMessage: read its Content property.
    private static async Task<Post> ReadPostAsync(
        HttpResponseMessage response,
        CancellationToken cancellationToken
    ) =>
        await response.Content.ReadFromJsonAsync<Post>(cancellationToken)
        ?? throw new InvalidDataException("The response contained no post.");

    private static Task<string> ReadErrorAsync(
        HttpResponseMessage response,
        CancellationToken cancellationToken
    ) => response.Content.ReadAsStringAsync(cancellationToken);
}

public sealed record Post(int UserId, int Id, string Title, string Body);

public sealed record CreatePost(int UserId, string Title, string Body);
