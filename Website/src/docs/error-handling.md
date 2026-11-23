---
layout: layouts/docs.njk
title: Error Handling
description: Master functional error handling with RestClient.Net Result types. Learn pattern matching, discriminated unions, and exhaustiveness checking for C# REST calls.
keywords: C# error handling, Result type, discriminated unions, pattern matching, exhaustiveness checking
eleventyNavigation:
  key: Error Handling
  order: 4
faq:
  - question: What is a Result type in RestClient.Net?
    answer: A Result type is a discriminated union that represents either success (Ok) or failure (Error). It forces you to handle all possible outcomes at compile time.
  - question: What is exhaustiveness checking?
    answer: The Exhaustion analyzer verifies that your switch expressions handle all possible cases. Missing a case causes a compile error, not a runtime crash.
  - question: What is the difference between ResponseError and ExceptionError?
    answer: ResponseError occurs when the server returns an error status code (4xx, 5xx) with a response body. ExceptionError occurs when an exception is thrown (network error, timeout, etc.).
---

# Error Handling

RestClient.Net uses discriminated unions and exhaustiveness checking to make error handling safe and explicit. This guide explains the patterns and best practices.

## The Problem with Traditional Error Handling

Traditional HTTP clients throw exceptions:

```csharp
// Traditional approach - dangerous!
try
{
    var response = await httpClient.GetAsync("https://api.example.com/user/1");
    response.EnsureSuccessStatusCode(); // Throws on error!
    var user = await response.Content.ReadFromJsonAsync<User>();
}
catch (HttpRequestException ex)
{
    // Network error
}
catch (JsonException ex)
{
    // Deserialization error
}
catch (Exception ex)
{
    // What else could happen? Who knows!
}
```

Problems:
- Nothing in the type signature tells you what might throw
- Easy to forget a catch block
- The happy path and error path have different structures
- Runtime crashes if you miss an exception type

## The RestClient.Net Approach

Every operation returns a `Result<TSuccess, HttpError<TError>>`:

```csharp
// The type tells you everything that can happen
Result<User, HttpError<ApiError>> result = await httpClient.GetAsync(...);

// Pattern matching ensures you handle all cases
var output = result switch
{
    OkUser(var user) => user.Name,
    ErrorUser(ResponseErrorUser(var err, var status, _)) => $"API Error {status}",
    ErrorUser(ExceptionErrorUser(var ex)) => $"Exception: {ex.Message}",
};
```

## Understanding the Result Type

### Result<TSuccess, TError>

The base discriminated union:

```csharp
public abstract record Result<TSuccess, TError>
{
    public sealed record Ok<TSuccess, TError>(TSuccess Value) : Result<TSuccess, TError>;
    public sealed record Error<TSuccess, TError>(TError Value) : Result<TSuccess, TError>;
}
```

### HttpError<TError>

The error type for HTTP operations:

```csharp
public abstract record HttpError<TError>
{
    // Server returned an error response (4xx, 5xx)
    public sealed record ResponseError(
        TError Error,
        HttpStatusCode StatusCode,
        HttpResponseMessage Response
    ) : HttpError<TError>;

    // Exception occurred (network error, timeout, etc.)
    public sealed record ExceptionError(
        Exception Exception
    ) : HttpError<TError>;
}
```

## Exhaustiveness Checking

The Exhaustion analyzer ensures you handle all cases:

```csharp
// This won't compile!
var message = result switch
{
    OkUser(var user) => "Success",
    ErrorUser(ResponseErrorUser(...)) => "Response Error",
    // COMPILE ERROR: Missing ExceptionErrorUser
};
```

The error message tells you exactly what's missing:

```
error EXHAUSTION001: Switch on Result is not exhaustive;
Matched: Ok<User, HttpError<ApiError>>, Error<User, HttpError<ApiError>> with ErrorResponseError
Missing: Error<User, HttpError<ApiError>> with ExceptionError
```

## Pattern Matching Patterns

### Basic Pattern

Handle all three cases explicitly:

```csharp
var message = result switch
{
    OkUser(var user) => $"Welcome, {user.Name}",
    ErrorUser(ResponseErrorUser(var err, var status, _)) =>
        $"Server error {(int)status}: {err.Message}",
    ErrorUser(ExceptionErrorUser(var ex)) =>
        $"Network error: {ex.Message}",
};
```

### With Status Code Matching

Handle specific status codes differently:

```csharp
var message = result switch
{
    OkUser(var user) => $"Found: {user.Name}",

    ErrorUser(ResponseErrorUser(_, HttpStatusCode.NotFound, _)) =>
        "User not found",

    ErrorUser(ResponseErrorUser(_, HttpStatusCode.Unauthorized, _)) =>
        "Please log in",

    ErrorUser(ResponseErrorUser(var err, var status, _)) =>
        $"Server error {(int)status}: {err.Message}",

    ErrorUser(ExceptionErrorUser(var ex)) =>
        $"Network error: {ex.Message}",
};
```

### Accessing the Full Response

The `ResponseError` includes the full `HttpResponseMessage`:

```csharp
var message = result switch
{
    OkUser(var user) => user.Name,

    ErrorUser(ResponseErrorUser(var err, var status, var response)) =>
    {
        // Access response headers
        if (response.Headers.TryGetValues("X-Rate-Limit-Remaining", out var values))
        {
            Console.WriteLine($"Rate limit remaining: {values.First()}");
        }
        return $"Error: {err.Message}";
    },

    ErrorUser(ExceptionErrorUser(var ex)) => $"Exception: {ex.Message}",
};
```

### Exception Type Matching

Handle specific exception types:

```csharp
var message = result switch
{
    OkUser(var user) => user.Name,

    ErrorUser(ResponseErrorUser(var err, _, _)) => err.Message,

    ErrorUser(ExceptionErrorUser(TaskCanceledException ex)) when ex.CancellationToken.IsCancellationRequested =>
        "Request was cancelled",

    ErrorUser(ExceptionErrorUser(TaskCanceledException)) =>
        "Request timed out",

    ErrorUser(ExceptionErrorUser(HttpRequestException)) =>
        "Network connectivity issue",

    ErrorUser(ExceptionErrorUser(var ex)) =>
        $"Unexpected error: {ex.Message}",
};
```

## Common Error Handling Patterns

### Default Value on Error

Return a default when any error occurs:

```csharp
User user = result switch
{
    OkUser(var u) => u,
    _ => User.Guest,
};
```

### Throw on Error (Escape Hatch)

When you genuinely can't handle the error:

```csharp
User user = result switch
{
    OkUser(var u) => u,
    ErrorUser(var error) => throw new InvalidOperationException($"Failed to get user: {error}"),
};
```

### Convert to Nullable

Useful for optional data:

```csharp
User? user = result switch
{
    OkUser(var u) => u,
    _ => null,
};

if (user is not null)
{
    // Use user
}
```

### Logging Errors

Log errors while still handling them:

```csharp
var message = result switch
{
    OkUser(var user) => user.Name,

    ErrorUser(ResponseErrorUser(var err, var status, _)) =>
    {
        logger.LogWarning("API returned {Status}: {Error}", status, err.Message);
        return "Service temporarily unavailable";
    },

    ErrorUser(ExceptionErrorUser(var ex)) =>
    {
        logger.LogError(ex, "Network error occurred");
        return "Connection failed";
    },
};
```

## Chaining Operations

### Map - Transform Success

Transform the success value without touching errors:

```csharp
// Convert Result<User, HttpError<ApiError>> to Result<string, HttpError<ApiError>>
var nameResult = userResult.Map(user => user.Name);
```

### FlatMap / Bind - Chain Async Operations

Chain operations that each return a Result:

```csharp
// Get user, then get their orders
var ordersResult = await userResult
    .FlatMapAsync(user => GetOrdersAsync(user.Id));
```

### Aggregate Multiple Results

Combine multiple Results:

```csharp
var userResult = await GetUserAsync(userId);
var ordersResult = await GetOrdersAsync(userId);
var settingsResult = await GetSettingsAsync(userId);

var combined = (userResult, ordersResult, settingsResult) switch
{
    (OkUser(var user), OkOrders(var orders), OkSettings(var settings)) =>
        new UserDashboard(user, orders, settings),

    (ErrorUser(var e), _, _) => throw new Exception($"User error: {e}"),
    (_, ErrorOrders(var e), _) => throw new Exception($"Orders error: {e}"),
    (_, _, ErrorSettings(var e)) => throw new Exception($"Settings error: {e}"),
};
```

## Error Response Models

Define clear error models for your API:

```csharp
// Simple error
record ApiError(string Message, string Code);

// Detailed error with validation
record ValidationError(
    string Message,
    Dictionary<string, string[]> Errors
);

// Standard problem details (RFC 7807)
record ProblemDetails(
    string Type,
    string Title,
    int Status,
    string Detail,
    string Instance
);
```

## Best Practices

1. **Define type aliases** for cleaner pattern matching
2. **Always handle all cases** - the compiler enforces this
3. **Be specific with error handling** - don't just catch everything
4. **Log errors** before returning user-friendly messages
5. **Use the response object** for headers and advanced scenarios
6. **Chain operations** with Map and FlatMap when appropriate

## Next Steps

- [Advanced Usage](/docs/advanced-usage/) - Retry policies and middleware
- [Exhaustion Analyzer](/docs/exhaustion/) - Deep dive into exhaustiveness checking
- [API Reference](/api/result-types/) - Complete Result type documentation
