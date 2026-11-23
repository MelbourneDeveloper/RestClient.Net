---
layout: layouts/docs.njk
title: Basic Usage
description: Learn the fundamentals of making type-safe REST calls with RestClient.Net. GET, POST, PUT, DELETE examples with proper error handling.
keywords: RestClient.Net tutorial, C# REST client, HttpClient extensions, type-safe HTTP calls
eleventyNavigation:
  key: Basic Usage
  order: 3
faq:
  - question: How do I make a GET request with RestClient.Net?
    answer: Use the GetAsync extension method on HttpClient with a URL, success deserializer, and error deserializer. The result is a discriminated union you must pattern match.
  - question: What is the ToAbsoluteUrl() extension?
    answer: ToAbsoluteUrl() converts a string to an AbsoluteUrl type, which validates the URL format at compile time and provides type safety.
  - question: Why do I need deserializer functions?
    answer: Deserializer functions allow you to control exactly how responses are parsed, supporting any serialization format (JSON, XML, etc.) and custom validation.
---

# Basic Usage

This guide covers the fundamentals of making REST calls with RestClient.Net.

## Your First Request

Let's make a simple GET request to fetch a post:

```csharp
using System.Net.Http.Json;
using RestClient.Net;
using Urls;

// Define your models
record Post(int UserId, int Id, string Title, string Body);
record ErrorResponse(string Message);

// Create HttpClient (use IHttpClientFactory in production)
using var httpClient = new HttpClient();

// Make the request
var result = await httpClient.GetAsync(
    url: "https://jsonplaceholder.typicode.com/posts/1".ToAbsoluteUrl(),
    deserializeSuccess: async (content, ct) =>
        await content.ReadFromJsonAsync<Post>(ct)
        ?? throw new InvalidOperationException("Null response"),
    deserializeError: async (content, ct) =>
        await content.ReadFromJsonAsync<ErrorResponse>(ct)
        ?? new ErrorResponse("Unknown error")
);

// Handle the result - ALL cases must be handled
var message = result switch
{
    Outcome.Result<Post, Outcome.HttpError<ErrorResponse>>.Ok(var post) =>
        $"Success: {post.Title}",

    Outcome.Result<Post, Outcome.HttpError<ErrorResponse>>.Error(
        Outcome.HttpError<ErrorResponse>.ResponseError(var err, var status, _)) =>
        $"API Error {status}: {err.Message}",

    Outcome.Result<Post, Outcome.HttpError<ErrorResponse>>.Error(
        Outcome.HttpError<ErrorResponse>.ExceptionError(var ex)) =>
        $"Exception: {ex.Message}",
};

Console.WriteLine(message);
```

## Using Type Aliases

The full type names are verbose. Add type aliases to `GlobalUsings.cs`:

```csharp
// GlobalUsings.cs
global using OkPost = Outcome.Result<Post, Outcome.HttpError<ErrorResponse>>
    .Ok<Post, Outcome.HttpError<ErrorResponse>>;

global using ErrorPost = Outcome.Result<Post, Outcome.HttpError<ErrorResponse>>
    .Error<Post, Outcome.HttpError<ErrorResponse>>;

global using ResponseErrorPost = Outcome.HttpError<ErrorResponse>.ErrorResponseError;

global using ExceptionErrorPost = Outcome.HttpError<ErrorResponse>.ExceptionError;
```

Now pattern matching is cleaner:

```csharp
var message = result switch
{
    OkPost(var post) => $"Success: {post.Title}",
    ErrorPost(ResponseErrorPost(var err, var status, _)) => $"API Error {status}: {err.Message}",
    ErrorPost(ExceptionErrorPost(var ex)) => $"Exception: {ex.Message}",
};
```

## POST Request

Create a new resource with POST:

```csharp
record CreatePostRequest(string Title, string Body, int UserId);

var newPost = new CreatePostRequest("My Title", "My content", 1);

var result = await httpClient.PostAsync(
    url: "https://jsonplaceholder.typicode.com/posts".ToAbsoluteUrl(),
    body: newPost,
    serializeRequest: body => JsonContent.Create(body),
    deserializeSuccess: async (content, ct) =>
        await content.ReadFromJsonAsync<Post>(ct)
        ?? throw new InvalidOperationException("Null response"),
    deserializeError: async (content, ct) =>
        await content.ReadFromJsonAsync<ErrorResponse>(ct)
        ?? new ErrorResponse("Unknown error")
);
```

## PUT Request

Replace an existing resource:

```csharp
record UpdatePostRequest(int Id, string Title, string Body, int UserId);

var updatedPost = new UpdatePostRequest(1, "Updated Title", "Updated content", 1);

var result = await httpClient.PutAsync(
    url: "https://jsonplaceholder.typicode.com/posts/1".ToAbsoluteUrl(),
    body: updatedPost,
    serializeRequest: body => JsonContent.Create(body),
    deserializeSuccess: async (content, ct) =>
        await content.ReadFromJsonAsync<Post>(ct)
        ?? throw new InvalidOperationException("Null response"),
    deserializeError: async (content, ct) =>
        await content.ReadFromJsonAsync<ErrorResponse>(ct)
        ?? new ErrorResponse("Unknown error")
);
```

## DELETE Request

Remove a resource:

```csharp
record DeleteResponse(bool Success);

var result = await httpClient.DeleteAsync(
    url: "https://jsonplaceholder.typicode.com/posts/1".ToAbsoluteUrl(),
    deserializeSuccess: async (content, ct) =>
        new DeleteResponse(true),
    deserializeError: async (content, ct) =>
        await content.ReadFromJsonAsync<ErrorResponse>(ct)
        ?? new ErrorResponse("Unknown error")
);
```

## PATCH Request

Partially update a resource:

```csharp
record PatchPostRequest(string? Title = null, string? Body = null);

var patch = new PatchPostRequest(Title: "Only updating title");

var result = await httpClient.PatchAsync(
    url: "https://jsonplaceholder.typicode.com/posts/1".ToAbsoluteUrl(),
    body: patch,
    serializeRequest: body => JsonContent.Create(body),
    deserializeSuccess: async (content, ct) =>
        await content.ReadFromJsonAsync<Post>(ct)
        ?? throw new InvalidOperationException("Null response"),
    deserializeError: async (content, ct) =>
        await content.ReadFromJsonAsync<ErrorResponse>(ct)
        ?? new ErrorResponse("Unknown error")
);
```

## Reusable Deserializers

Create reusable deserializer methods:

```csharp
public static class Deserializers
{
    public static async Task<T> Json<T>(HttpContent content, CancellationToken ct)
        where T : class =>
        await content.ReadFromJsonAsync<T>(ct)
        ?? throw new InvalidOperationException($"Failed to deserialize {typeof(T).Name}");

    public static async Task<ErrorResponse> Error(HttpContent content, CancellationToken ct) =>
        await content.ReadFromJsonAsync<ErrorResponse>(ct)
        ?? new ErrorResponse("Unknown error");
}

// Usage
var result = await httpClient.GetAsync(
    url: "https://api.example.com/posts/1".ToAbsoluteUrl(),
    deserializeSuccess: Deserializers.Json<Post>,
    deserializeError: Deserializers.Error
);
```

## Adding Headers

Set headers on the HttpClient:

```csharp
httpClient.DefaultRequestHeaders.Authorization =
    new AuthenticationHeaderValue("Bearer", "your-token");

httpClient.DefaultRequestHeaders.Add("X-Custom-Header", "value");

var result = await httpClient.GetAsync(...);
```

## Query Parameters

Build URLs with query parameters:

```csharp
// Using string interpolation
var url = $"https://api.example.com/posts?userId={userId}&page={page}".ToAbsoluteUrl();

// Or use a URL builder library
var result = await httpClient.GetAsync(
    url: url,
    deserializeSuccess: Deserializers.Json<List<Post>>,
    deserializeError: Deserializers.Error
);
```

## Cancellation

Pass a `CancellationToken` for cancellable requests:

```csharp
var cts = new CancellationTokenSource(TimeSpan.FromSeconds(10));

var result = await httpClient.GetAsync(
    url: "https://api.example.com/posts/1".ToAbsoluteUrl(),
    deserializeSuccess: Deserializers.Json<Post>,
    deserializeError: Deserializers.Error,
    cancellationToken: cts.Token
);
```

## Best Practices

1. **Use IHttpClientFactory** in production for proper connection pooling
2. **Define type aliases** in GlobalUsings.cs for cleaner code
3. **Create reusable deserializers** to avoid duplication
4. **Always handle all cases** in switch expressions
5. **Use cancellation tokens** for user-facing requests

## Next Steps

- [Error Handling](/docs/error-handling/) - Deep dive into Result types
- [Advanced Usage](/docs/advanced-usage/) - Retry policies, middleware, and more
- [API Reference](/api/) - Complete method documentation
