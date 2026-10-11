---
layout: layouts/api.njk
title: Result Types
description: Complete reference for RestClient.Net Result types - discriminated unions for type-safe HTTP error handling with pattern matching.
keywords: Result types, HttpError, discriminated unions, pattern matching, C# error handling
eleventyNavigation:
  key: Result Types
  parent: API Reference
  order: 2
permalink: /api/result-types/
---

RestClient.Net uses [discriminated unions](https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/record) to represent HTTP responses. This forces you to handle all possible outcomes at compile time.

## Result&lt;TSuccess, TError&gt;

The core result type that represents either success or failure.

```csharp
public abstract record Result<TSuccess, TError>
{
    public record Ok(TSuccess Value) : Result<TSuccess, TError>;
    public record Error(TError Value) : Result<TSuccess, TError>;
}
```

### Pattern Matching

Use [switch expressions](https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/operators/switch-expression) to handle all cases:

```csharp
var message = result switch
{
    Result<User, HttpError<ApiError>>.Ok(var user) =>
        $"Got user: {user.Name}",

    Result<User, HttpError<ApiError>>.Error(var error) =>
        $"Error occurred: {error}"
};
```

## HttpError&lt;TError&gt;

Represents HTTP-specific errors. Can be either a response error (server returned an error status) or an exception error (network failure, timeout, etc.).

```csharp
public abstract record HttpError<TError>
{
    public record ResponseError(
        TError Error,
        HttpStatusCode StatusCode,
        HttpResponseHeaders Headers
    ) : HttpError<TError>;

    public record ExceptionError(Exception Exception) : HttpError<TError>;
}
```

### ResponseError Properties

| Property | Type | Description |
|----------|------|-------------|
| `Error` | `TError` | Your deserialized error model |
| `StatusCode` | [`HttpStatusCode`](https://learn.microsoft.com/en-us/dotnet/api/system.net.httpstatuscode) | The HTTP status code (e.g., 404, 500) |
| `Headers` | `HttpResponseHeaders` | Response headers for accessing metadata |

### ExceptionError Properties

| Property | Type | Description |
|----------|------|-------------|
| `Exception` | [`Exception`](https://learn.microsoft.com/en-us/dotnet/api/system.exception) | The caught exception (timeout, network error, etc.) |

### Full Pattern Matching Example

```csharp
var message = result switch
{
    Result<User, HttpError<ApiError>>.Ok(var user) =>
        $"Success: {user.Name}",

    Result<User, HttpError<ApiError>>.Error(
        HttpError<ApiError>.ResponseError(var err, var status, _)) =>
        $"API Error {status}: {err.Message}",

    Result<User, HttpError<ApiError>>.Error(
        HttpError<ApiError>.ExceptionError(var ex)) =>
        $"Exception: {ex.Message}",
};
```

## Type Aliases

The full type names are verbose. Define [global using aliases](https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/using-directive#global-modifier) in `GlobalUsings.cs`:

```csharp
// GlobalUsings.cs - Define once, use everywhere

// Result aliases
global using OkUser = Outcome.Result<User, Outcome.HttpError<ApiError>>
    .Ok<User, Outcome.HttpError<ApiError>>;

global using ErrorUser = Outcome.Result<User, Outcome.HttpError<ApiError>>
    .Error<User, Outcome.HttpError<ApiError>>;

// HttpError aliases
global using ResponseErrorUser = Outcome.HttpError<ApiError>.ResponseError;
global using ExceptionErrorUser = Outcome.HttpError<ApiError>.ExceptionError;
```

### Using Type Aliases

With aliases defined, pattern matching becomes much cleaner:

```csharp
var message = result switch
{
    OkUser(var user) => $"Success: {user.Name}",
    ErrorUser(ResponseErrorUser(var err, var status, _)) => $"API Error {status}: {err.Message}",
    ErrorUser(ExceptionErrorUser(var ex)) => $"Exception: {ex.Message}",
};
```

## Exhaustion Analyzer

The Exhaustion [Roslyn analyzer](https://learn.microsoft.com/en-us/dotnet/csharp/roslyn-sdk/) ensures you handle all cases:

```csharp
// This won't compile!
var message = result switch
{
    OkUser(var user) => "Success",
    ErrorUser(ResponseErrorUser(...)) => "API Error",
    // COMPILE ERROR: Missing ExceptionError case!
};
```

The compiler error:

```
error EXHAUSTION001: Switch on Result is not exhaustive;
Missing: Error<User, HttpError<ApiError>> with ExceptionError
```

## Handling Specific Status Codes

```csharp
var message = result switch
{
    OkUser(var user) => $"Success: {user.Name}",

    ErrorUser(ResponseErrorUser(_, HttpStatusCode.NotFound, _)) =>
        "User not found",

    ErrorUser(ResponseErrorUser(_, HttpStatusCode.Unauthorized, _)) =>
        "Authentication required",

    ErrorUser(ResponseErrorUser(var err, var status, _)) =>
        $"Error {(int)status}: {err.Message}",

    ErrorUser(ExceptionErrorUser(var ex)) =>
        $"Network error: {ex.Message}",
};
```

## See Also

- [HttpClient Extensions](/api/httpclient-extensions/) - Extension methods that return Result types
- [Serialization](/api/serialization/) - Custom serialization and deserialization
