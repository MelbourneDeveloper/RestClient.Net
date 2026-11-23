---
layout: layouts/blog.njk
title: Announcing RestClient.Net v6 - Type-Safe REST for C#
date: 2026-01-20
author: Christian Findlay
tags: posts
excerpt: RestClient.Net v6 brings discriminated unions to C# REST calls with compile-time exhaustiveness checking.
---

# Announcing RestClient.Net v6

We're excited to announce RestClient.Net v6, a major release that brings functional programming patterns to C# REST calls.

## What's New

### Result Types Instead of Exceptions

Gone are the days of try-catch blocks around HTTP calls. RestClient.Net v6 returns `Result<TSuccess, HttpError<TError>>` - a discriminated union that forces you to handle all outcomes:

```csharp
var result = await httpClient
    .GetAsync(
        url: "https://api.example.com/posts/1".ToAbsoluteUrl(),
        deserializeSuccess: DeserializePost,
        deserializeError: DeserializeError
    );

var output = result switch
{
    OkPost(var post) => $"Success: {post.Title}",
    ErrorPost(ResponseErrorPost(var err, var status, _)) => $"Error {status}",
    ErrorPost(ExceptionErrorPost(var ex)) => $"Exception: {ex.Message}",
};
```

### Exhaustiveness Checking

The Exhaustion analyzer ensures you don't miss any cases. If you forget to handle `ExceptionErrorPost`, your code won't compile:

```
error EXHAUSTION001: Switch on Result is not exhaustive;
Missing: Error<Post, HttpError<ErrorResponse>> with ExceptionError<ErrorResponse>
```

### OpenAPI Generator

Generate type-safe clients from OpenAPI 3.x specs:

```bash
dotnet run --project RestClient.Net.OpenApiGenerator.Cli -- \
  -u api.yaml -o Generated -n YourApi.Generated
```

## Getting Started

Install via NuGet:

```bash
dotnet add package RestClient.Net
```

Check out our [documentation](/docs/) to learn more!
