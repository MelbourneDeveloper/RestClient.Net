---
layout: layouts/api.njk
title: HttpClientExtensions Class
description: Extension methods for HttpClient that return Result types instead of throwing exceptions.
keywords: HttpClientExtensions, HttpClient, REST API, C# HTTP client, extension methods
eleventyNavigation:
  key: HttpClient Extensions
  parent: API Reference
  order: 1
permalink: /api/httpclient-extensions/
---

Extension methods for [`HttpClient`](https://learn.microsoft.com/en-us/dotnet/api/system.net.http.httpclient) that return [`Result<TSuccess, HttpError<TError>>`](/api/reference/outcome-result-2/) instead of throwing exceptions.

## Namespace

`RestClient.Net`

## Methods

| Method | Description |
|--------|-------------|
| [GetAsync&lt;TSuccess, TError&gt;](/api/getasync/) | Make a type-safe GET request |
| [PostAsync&lt;TRequest, TSuccess, TError&gt;](/api/postasync/) | Make a type-safe POST request with body |
| [PutAsync&lt;TRequest, TSuccess, TError&gt;](/api/putasync/) | Make a type-safe PUT request for full replacement |
| [DeleteAsync&lt;TSuccess, TError&gt;](/api/deleteasync/) | Make a type-safe DELETE request |
| [PatchAsync&lt;TRequest, TSuccess, TError&gt;](/api/patchasync/) | Make a type-safe PATCH request for partial updates |

## See Also

- [Result&lt;TSuccess, TError&gt;](/api/reference/outcome-result-2/) - The discriminated union return type
- [HttpError&lt;TError&gt;](/api/reference/outcome-httperror-1/) - HTTP-specific error wrapper
- [Serialization](/api/serialization/) - Custom serialization and deserialization
