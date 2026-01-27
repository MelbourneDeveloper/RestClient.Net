---
layout: layouts/blog.njk
title: Why Discriminated Unions Are the Future of Error Handling in C#
date: 2025-12-20
author: Christian Findlay
excerpt: Explore how discriminated unions and pattern matching transform error handling from an afterthought into a first-class language feature.
tags: posts
---

# Why Discriminated Unions Are the Future of Error Handling in C#

Error handling in most C# codebases is a mess. We pretend exceptions are exceptional, scatter try-catch blocks throughout our code, and hope we've caught everything. There's a better way.

## The Exception Problem

Exceptions have served us well, but they have fundamental issues:

### 1. They're Invisible

Look at this method signature:

```csharp
public async Task<User> GetUserAsync(string userId)
```

What can go wrong? The signature doesn't tell you:
- Network failures?
- User not found (404)?
- Authentication expired (401)?
- Server errors (500)?
- JSON parsing errors?
- Timeout?

You have to read the documentation (if it exists) or the implementation to find out.

### 2. They're Easy to Forget

```csharp
var user = await userService.GetUserAsync("123");
// If this throws, we crash
Console.WriteLine(user.Name);
```

The compiler happily accepts this code. No warning. No error. Just a runtime crash waiting to happen.

### 3. They Encourage Bad Patterns

When exceptions are your only error mechanism, you end up with:

```csharp
try
{
    var user = await GetUserAsync(userId);
    var orders = await GetOrdersAsync(user.Id);
    var recommendations = await GetRecommendationsAsync(user.Id);
    return new Dashboard(user, orders, recommendations);
}
catch (HttpRequestException ex)
{
    // Which call failed? We don't know!
    return null;
}
catch (JsonException ex)
{
    // Deserialization failed... somewhere
    return null;
}
catch (Exception ex)
{
    // The "catch all" that catches nothing useful
    return null;
}
```

## Enter Discriminated Unions

A discriminated union is a type that can be one of several distinct cases. In RestClient.Net:

```csharp
Result<User, HttpError<ApiError>> result = await GetUserAsync(userId);
```

This type says: "I am either a successful User OR an error. I cannot be null. I cannot be something else."

### The Result Type

```csharp
public abstract record Result<TSuccess, TError>
{
    public sealed record Ok(TSuccess Value) : Result<TSuccess, TError>;
    public sealed record Error(TError Value) : Result<TSuccess, TError>;
}
```

Simple. Elegant. Complete.

### The HttpError Type

For HTTP operations, we further distinguish error types:

```csharp
public abstract record HttpError<TError>
{
    // Server returned an error response
    public sealed record ResponseError(
        TError Error,
        HttpStatusCode StatusCode,
        HttpResponseMessage Response
    ) : HttpError<TError>;

    // Exception occurred (network, timeout, etc.)
    public sealed record ExceptionError(
        Exception Exception
    ) : HttpError<TError>;
}
```

Now the type tells you exactly what can happen:
- Success with a User
- Error response from the server (with status code and body)
- Exception during the request (with the exception)

## Pattern Matching: The Key

C#'s pattern matching makes working with discriminated unions elegant:

```csharp
var message = result switch
{
    OkUser(var user) => $"Welcome, {user.Name}!",
    ErrorUser(ResponseErrorUser(_, HttpStatusCode.NotFound, _)) => "User not found",
    ErrorUser(ResponseErrorUser(_, HttpStatusCode.Unauthorized, _)) => "Please log in",
    ErrorUser(ResponseErrorUser(var err, var status, _)) => $"Error {status}: {err.Message}",
    ErrorUser(ExceptionErrorUser(var ex)) => $"Network error: {ex.Message}",
};
```

Every case is explicit. The compiler knows all possibilities.

## Exhaustiveness: The Game Changer

Here's where it gets powerful. With the Exhaustion analyzer, this code won't compile:

```csharp
var message = result switch
{
    OkUser(var user) => user.Name,
    ErrorUser(ResponseErrorUser(var err, _, _)) => err.Message,
    // COMPILE ERROR: Missing ExceptionErrorUser case!
};
```

```
error EXHAUSTION001: Switch on Result is not exhaustive;
Missing: Error<User, HttpError<ApiError>> with ExceptionError
```

**Runtime crashes become compile-time errors.**

## Real-World Benefits

### 1. Self-Documenting Code

The return type tells you everything:

```csharp
// This method can return a User, a 404 error, or a network exception
public Task<Result<User, HttpError<NotFoundError>>> GetUserAsync(string id)
```

### 2. Fearless Refactoring

Add a new error case? The compiler finds every place that needs updating:

```csharp
// Added a new error type
public abstract record HttpError<TError>
{
    public sealed record ResponseError(...) : HttpError<TError>;
    public sealed record ExceptionError(...) : HttpError<TError>;
    public sealed record TimeoutError(...) : HttpError<TError>; // NEW!
}
```

Every switch statement that handles `HttpError` now shows a compile error until you handle `TimeoutError`.

### 3. Composable Operations

Chain operations that might fail:

```csharp
var dashboard = await GetUserAsync(userId)
    .FlatMapAsync(user => GetOrdersAsync(user.Id))
    .MapAsync(orders => new Dashboard(orders));
```

If any step fails, the error propagates. No try-catch needed.

### 4. Better Testing

Test each case explicitly:

```csharp
[Fact]
public async Task GetUser_ReturnsResponseError_WhenNotFound()
{
    var result = await service.GetUserAsync("unknown");

    var error = Assert.IsType<ErrorUser>(result);
    var responseError = Assert.IsType<ResponseErrorUser>(error.Value);
    Assert.Equal(HttpStatusCode.NotFound, responseError.StatusCode);
}
```

### 5. Cleaner Code Reviews

Reviewers don't need to ask "did you handle errors?" The compiler already verified it.

## The Path Forward

C# is evolving toward better support for discriminated unions. But you don't have to wait:

1. **Use RestClient.Net** for HTTP operations
2. **Install the Exhaustion analyzer** for compile-time checking
3. **Create your own discriminated unions** for domain logic

Example domain union:

```csharp
public abstract record PaymentResult
{
    public sealed record Success(string TransactionId) : PaymentResult;
    public sealed record InsufficientFunds(decimal Available, decimal Required) : PaymentResult;
    public sealed record CardDeclined(string Reason) : PaymentResult;
    public sealed record Timeout(TimeSpan Duration) : PaymentResult;
}
```

Now payment handling is explicit, exhaustive, and documented in the type system.

## Conclusion

Discriminated unions transform error handling from:
- **Invisible** to **explicit**
- **Optional** to **required**
- **Runtime crashes** to **compile-time errors**

It's not just a different syntax. It's a fundamentally safer approach to building software.

Try RestClient.Net today and experience the future of error handling in C#.

---

*Related: [Functional Error Handling in C#](/blog/functional-error-handling/) | [Getting Started](/docs/)*
