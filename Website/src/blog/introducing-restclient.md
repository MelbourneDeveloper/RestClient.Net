---
layout: layouts/blog.njk
title: Introducing RestClient.Net - Type-Safe REST Calls in C#
date: 2024-01-15
author: Christian Findlay
excerpt: Learn how RestClient.Net brings functional programming patterns to HTTP calls with discriminated unions and exhaustiveness checking.
tags: posts
---

# Introducing RestClient.Net

RestClient.Net is a modern, type-safe REST client for C# that brings functional programming patterns to HTTP communication.

## Why Another REST Client?

Traditional HTTP clients in .NET have a problem: they throw exceptions. This forces developers into try/catch blocks and makes error handling inconsistent and error-prone.

RestClient.Net takes a different approach. Instead of throwing exceptions, every HTTP call returns a `Result` type that must be explicitly handled.

## The Power of Discriminated Unions

```csharp
// Make a GET request
var result = await httpClient.GetAsync(
    url: "https://api.example.com/posts/1".ToAbsoluteUrl(),
    deserializeSuccess: DeserializePost,
    deserializeError: DeserializeError
);

// Pattern match - compiler enforces you handle ALL cases
var output = result switch
{
    OkPost(var post) => $"Success: {post.Title}",
    ErrorPost(ResponseErrorPost(var err, var status, _)) => $"API Error: {status}",
    ErrorPost(ExceptionErrorPost(var ex)) => $"Exception: {ex.Message}",
};
```

## Exhaustiveness Checking

With the [Exhaustion analyzer](https://www.nuget.org/packages/Exhaustion), your code won't compile if you miss a case. No more runtime surprises.

## Get Started

```bash
dotnet add package RestClient.Net
```

Check out the [documentation](/docs/) to learn more!
