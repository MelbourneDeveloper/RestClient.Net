---
layout: layouts/docs.njk
title: Getting Started
description: Get started with RestClient.Net, a type-safe REST client for C# with functional error handling, discriminated unions, and compile-time exhaustiveness checking.
keywords: RestClient.Net, C# REST client, type-safe HTTP, functional error handling, discriminated unions
eleventyNavigation:
  key: Getting Started
  order: 1
faq:
  - question: What is RestClient.Net?
    answer: RestClient.Net is a type-safe REST client for C# that uses discriminated unions and pattern matching for error handling, eliminating the need for try-catch blocks around HTTP calls.
  - question: Why use RestClient.Net instead of HttpClient directly?
    answer: RestClient.Net wraps HttpClient with Result types that force you to handle all error cases at compile time. This eliminates runtime crashes from unhandled exceptions.
  - question: What is the Exhaustion analyzer?
    answer: The Exhaustion analyzer is a Roslyn analyzer that verifies your switch expressions handle all possible cases of a Result type. Missing a case causes a compile error instead of a runtime crash.
---

<script type="application/ld+json">
{
  "@context": "https://schema.org",
  "@type": "HowTo",
  "name": "How to Use RestClient.Net",
  "description": "Make type-safe REST calls in C# with functional error handling",
  "totalTime": "PT5M",
  "step": [
    {
      "@type": "HowToStep",
      "name": "Install the package",
      "text": "Run 'dotnet add package RestClient.Net' to install from NuGet"
    },
    {
      "@type": "HowToStep",
      "name": "Define your models",
      "text": "Create record types for your API responses and error types"
    },
    {
      "@type": "HowToStep",
      "name": "Make a request",
      "text": "Use the HttpClient extension methods like GetAsync with deserializer functions"
    },
    {
      "@type": "HowToStep",
      "name": "Handle the result",
      "text": "Pattern match on the Result type to handle success and error cases"
    }
  ]
}
</script>

# Getting Started with RestClient.Net

RestClient.Net is a type-safe REST client for C# that brings functional programming patterns to HTTP communication. Instead of throwing exceptions, every call returns a `Result` type that you must explicitly handle.

## Why RestClient.Net?

Traditional HTTP clients throw exceptions, which have problems:

- **Invisible errors** - Method signatures don't show what can go wrong
- **Easy to forget** - Miss a catch block and your app crashes
- **Inconsistent handling** - Try-catch scattered throughout the codebase

RestClient.Net solves this with **discriminated unions**:

```csharp
// The return type tells you everything that can happen
Result<User, HttpError<ApiError>> result = await httpClient.GetUserAsync(...);
```

## Quick Start

### 1. Install the Package

```bash
dotnet add package RestClient.Net
```

This includes:
- `HttpClient` extension methods
- `Result<TSuccess, HttpError<TError>>` types
- The **Exhaustion analyzer** for compile-time checking

### 2. Define Your Models

```csharp
// Success response
record Post(int UserId, int Id, string Title, string Body);

// Error response
record ApiError(string Message, string? Code = null);
```

### 3. Make a Request

```csharp
using System.Net.Http.Json;
using RestClient.Net;
using Urls;

using var httpClient = new HttpClient();

var result = await httpClient.GetAsync(
    url: "https://jsonplaceholder.typicode.com/posts/1".ToAbsoluteUrl(),
    deserializeSuccess: async (content, ct) =>
        await content.ReadFromJsonAsync<Post>(ct)
        ?? throw new InvalidOperationException("Null response"),
    deserializeError: async (content, ct) =>
        await content.ReadFromJsonAsync<ApiError>(ct)
        ?? new ApiError("Unknown error")
);
```

### 4. Handle the Result

Pattern match on the result to handle all cases:

```csharp
var message = result switch
{
    Outcome.Result<Post, Outcome.HttpError<ApiError>>.Ok(var post) =>
        $"Success: {post.Title}",

    Outcome.Result<Post, Outcome.HttpError<ApiError>>.Error(
        Outcome.HttpError<ApiError>.ResponseError(var err, var status, _)) =>
        $"API Error {status}: {err.Message}",

    Outcome.Result<Post, Outcome.HttpError<ApiError>>.Error(
        Outcome.HttpError<ApiError>.ExceptionError(var ex)) =>
        $"Exception: {ex.Message}",
};

Console.WriteLine(message);
```

### 5. Add Type Aliases (Recommended)

The full type names are verbose. Add type aliases to `GlobalUsings.cs`:

```csharp
global using OkPost = Outcome.Result<Post, Outcome.HttpError<ApiError>>
    .Ok<Post, Outcome.HttpError<ApiError>>;

global using ErrorPost = Outcome.Result<Post, Outcome.HttpError<ApiError>>
    .Error<Post, Outcome.HttpError<ApiError>>;

global using ResponseErrorPost = Outcome.HttpError<ApiError>.ErrorResponseError;

global using ExceptionErrorPost = Outcome.HttpError<ApiError>.ExceptionError;
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

## Exhaustiveness Checking

The Exhaustion analyzer ensures you don't miss any cases:

```csharp
// This won't compile!
var message = result switch
{
    OkPost(var post) => "Success",
    ErrorPost(ResponseErrorPost(...)) => "API Error",
    // COMPILE ERROR: Missing ExceptionErrorPost case!
};
```

```
error EXHAUSTION001: Switch on Result is not exhaustive;
Missing: Error<Post, HttpError<ApiError>> with ExceptionError
```

**Runtime crashes become compile-time errors.**

## What's in the Box

| Package | Description |
|---------|-------------|
| `RestClient.Net` | Core library with HttpClient extensions and Result types |
| `RestClient.Net.OpenApiGenerator` | Generate type-safe clients from OpenAPI 3.x specs |
| `RestClient.Net.McpGenerator` | Generate MCP servers for Claude Code integration |
| `Exhaustion` | Roslyn analyzer for switch exhaustiveness (included) |

## Next Steps

- [Installation](/docs/installation/) - Detailed setup instructions
- [Basic Usage](/docs/basic-usage/) - GET, POST, PUT, DELETE examples
- [Error Handling](/docs/error-handling/) - Deep dive into Result types
- [Advanced Usage](/docs/advanced-usage/) - Retry policies, authentication, caching
- [OpenAPI Generator](/docs/openapi/) - Generate clients from specs
- [Code Examples](/examples/) - Complete working examples
