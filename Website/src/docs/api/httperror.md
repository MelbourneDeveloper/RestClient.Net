---
layout: layouts/docs.njk
title: HttpError Type
eleventyNavigation:
  key: HttpError Type
  parent: API Reference
  order: 6
---

# HttpError&lt;TError&gt;

Represents HTTP-specific errors that can occur during web requests. This sealed hierarchy ensures complete error handling coverage for HTTP operations.

**Namespace:** `Outcome`

## Overview

`HttpError` is a discriminated union with exactly two cases:
- `ErrorResponseError` - The server returned an error response (4xx, 5xx)
- `ExceptionError` - An exception occurred (network, timeout, etc.)

```csharp
public abstract partial record HttpError<TError>
```

## Type Parameter

| Parameter | Description |
|-----------|-------------|
| `TError` | The type used to deserialize error response bodies |

---

## Nested Types

### ErrorResponseError

Represents an HTTP error response from the server.

```csharp
public sealed record ErrorResponseError(
    TError Body,
    HttpStatusCode StatusCode,
    HttpResponseHeaders Headers
) : HttpError<TError>
```

#### Properties

| Property | Type | Description |
|----------|------|-------------|
| `Body` | `TError` | The deserialized error response body |
| `StatusCode` | `HttpStatusCode` | The HTTP status code (400, 404, 500, etc.) |
| `Headers` | `HttpResponseHeaders` | The response headers |

### ExceptionError

Represents an error caused by an exception during the HTTP operation.

```csharp
public sealed record ExceptionError(Exception Exception) : HttpError<TError>
```

#### Properties

| Property | Type | Description |
|----------|------|-------------|
| `Exception` | `Exception` | The exception that was caught |

---

## Properties

### IsExceptionError

Returns `true` if this error was caused by an exception.

```csharp
public bool IsExceptionError { get; }
```

### IsErrorResponse

Returns `true` if this error was caused by an HTTP error response.

```csharp
public bool IsErrorResponse { get; }
```

---

## Static Factory Methods

### FromException

Creates an HttpError from an exception.

```csharp
public static HttpError<TError> FromException(Exception exception)
```

### Example

```csharp
try
{
    // HTTP operation
}
catch (HttpRequestException ex)
{
    return Result<User, HttpError<ApiError>>.Failure(
        HttpError<ApiError>.FromException(ex)
    );
}
```

### FromErrorResponse

Creates an HttpError from a server error response.

```csharp
public static HttpError<TError> FromErrorResponse(
    TError body,
    HttpStatusCode statusCode,
    HttpResponseHeaders headers
)
```

### Example

```csharp
if (!response.IsSuccessStatusCode)
{
    var errorBody = await DeserializeError(response);
    return Result<User, HttpError<ApiError>>.Failure(
        HttpError<ApiError>.FromErrorResponse(
            errorBody,
            response.StatusCode,
            response.Headers
        )
    );
}
```

---

## Instance Methods

### Match

Transforms this HttpError using the provided functions.

```csharp
public TError Match(
    Func<Exception, TError> onException,
    Func<TError, HttpStatusCode, HttpResponseHeaders, TError> onErrorResponse
)
```

### Example

```csharp
string errorMessage = httpError.Match(
    onException: ex => $"Network error: {ex.Message}",
    onErrorResponse: (body, status, headers) =>
        $"API error {(int)status}: {body.Message}"
);
```

### IsErrorResponseError

Attempts to extract ErrorResponseError details using out parameters.

```csharp
public bool IsErrorResponseError(
    out TError body,
    out HttpStatusCode statusCode,
    out HttpResponseHeaders? headers
)
```

### Example

```csharp
if (httpError.IsErrorResponseError(out var body, out var status, out var headers))
{
    Console.WriteLine($"Status: {status}, Message: {body.Message}");
}
```

---

## Pattern Matching

The recommended way to handle HttpError:

```csharp
var message = httpError switch
{
    HttpError<ApiError>.ErrorResponseError(var body, var status, var headers)
        => status switch
        {
            HttpStatusCode.NotFound => "Resource not found",
            HttpStatusCode.Unauthorized => "Please log in",
            HttpStatusCode.Forbidden => "Access denied",
            HttpStatusCode.BadRequest => $"Invalid request: {body.Message}",
            _ => $"Server error ({(int)status}): {body.Message}"
        },

    HttpError<ApiError>.ExceptionError(var ex)
        => ex switch
        {
            HttpRequestException => "Network error - check your connection",
            TaskCanceledException => "Request timed out",
            _ => $"Unexpected error: {ex.Message}"
        }
};
```

### With Type Aliases

```csharp
global using ResponseError = Outcome.HttpError<ApiError>.ErrorResponseError;
global using ExceptionError = Outcome.HttpError<ApiError>.ExceptionError;

var message = httpError switch
{
    ResponseError(var body, var status, _) => $"Error {status}: {body.Message}",
    ExceptionError(var ex) => $"Exception: {ex.Message}",
};
```

---

## Common Error Types

Define a strongly-typed error model:

```csharp
public record ApiError(
    string Message,
    string? Code,
    IDictionary<string, string[]>? ValidationErrors
);
```

### Handling Validation Errors

```csharp
var result = await httpClient.PostAsync(...);

if (result is Result<User, HttpError<ApiError>>.Error<User, HttpError<ApiError>>(
    HttpError<ApiError>.ErrorResponseError(var error, HttpStatusCode.BadRequest, _)))
{
    if (error.ValidationErrors is not null)
    {
        foreach (var (field, messages) in error.ValidationErrors)
        {
            Console.WriteLine($"{field}: {string.Join(", ", messages)}");
        }
    }
}
```

---

## Integration with Result

HttpError is typically used as the failure type in Result:

```csharp
Result<TSuccess, HttpError<TError>>
```

### Complete Pattern Matching

```csharp
var output = result switch
{
    Result<User, HttpError<ApiError>>.Ok<User, HttpError<ApiError>>(var user)
        => $"Success: {user.Name}",

    Result<User, HttpError<ApiError>>.Error<User, HttpError<ApiError>>(
        HttpError<ApiError>.ErrorResponseError(var body, var status, _))
        => $"API Error {status}: {body.Message}",

    Result<User, HttpError<ApiError>>.Error<User, HttpError<ApiError>>(
        HttpError<ApiError>.ExceptionError(var ex))
        => $"Exception: {ex.Message}",
};
```

---

## Best Practices

1. **Define error models** - Create typed classes for your API's error responses
2. **Handle both cases** - Always handle both `ErrorResponseError` and `ExceptionError`
3. **Check status codes** - Different status codes often require different handling
4. **Log exceptions** - `ExceptionError` contains the full exception for debugging
5. **Use type aliases** - Simplify pattern matching with global using statements
6. **Inspect headers** - Rate limit info and other metadata are in response headers
