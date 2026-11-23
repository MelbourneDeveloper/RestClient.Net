---
layout: layouts/docs.njk
title: API Reference
eleventyNavigation:
  key: API Reference
  parent: Documentation
  order: 1
---

# API Reference

Complete reference documentation for RestClient.Net and the Outcome library.

## Core Packages

### RestClient.Net

The main library providing type-safe HTTP operations with discriminated union return types.

- [HttpClient Extensions](/docs/api/httpclient-extensions/) - Extension methods for `HttpClient`
- [IHttpClientFactory Extensions](/docs/api/httpclientfactory-extensions/) - Extension methods for `IHttpClientFactory`
- [Delegates](/docs/api/delegates/) - Function delegates for serialization/deserialization
- [Utilities](/docs/api/utilities/) - Helper classes like `ProgressReportingHttpContent`

### Outcome

The functional programming library providing discriminated unions for error handling.

- [Result Type](/docs/api/result/) - The core `Result<TSuccess, TFailure>` type
- [HttpError Type](/docs/api/httperror/) - HTTP-specific error representation
- [Result Extensions](/docs/api/result-extensions/) - Extension methods for `Result`
- [Unit Type](/docs/api/unit/) - The void equivalent for functional programming

### Code Generators

Tools for generating type-safe clients from API specifications.

- [OpenAPI Generator](/docs/api/openapi-generator/) - Generate C# clients from OpenAPI/Swagger specs
- [MCP Generator](/docs/api/mcp-generator/) - Generate Model Context Protocol servers from OpenAPI specs

## Quick Start

```csharp
using RestClient.Net;
using Outcome;

// Make a type-safe GET request
var result = await httpClient.GetAsync<User, ApiError>(
    url: "https://api.example.com/users/1".ToAbsoluteUrl(),
    deserializeSuccess: DeserializeJson<User>,
    deserializeError: DeserializeJson<ApiError>
);

// Handle the result with pattern matching
var message = result switch
{
    Result<User, HttpError<ApiError>>.Ok<User, HttpError<ApiError>>(var user)
        => $"Found user: {user.Name}",
    Result<User, HttpError<ApiError>>.Error<User, HttpError<ApiError>>(var error)
        => error switch
        {
            HttpError<ApiError>.ErrorResponseError(var body, var status, _)
                => $"API Error {status}: {body.Message}",
            HttpError<ApiError>.ExceptionError(var ex)
                => $"Exception: {ex.Message}",
            _ => "Unknown error"
        },
    _ => "Unexpected"
};
```

## Design Philosophy

RestClient.Net follows Railway Oriented Programming principles:

1. **No exceptions for expected errors** - HTTP errors are returned as values, not thrown
2. **Complete error handling** - Pattern matching ensures all cases are handled
3. **Type safety** - Generic parameters provide compile-time type checking
4. **Composability** - Results can be mapped, bound, and combined functionally
