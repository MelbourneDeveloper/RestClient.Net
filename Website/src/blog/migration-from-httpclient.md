---
layout: layouts/blog.njk
title: Migrating from Traditional HttpClient to RestClient.Net
date: 2026-01-18
author: Christian Findlay
excerpt: Step-by-step guide to migrating your existing HttpClient code to RestClient.Net with functional error handling.
tags: posts
---

# Migrating from Traditional HttpClient to RestClient.Net

If you have existing code using `HttpClient` with traditional try/catch error handling, this guide will help you migrate to RestClient.Net's functional approach.

## Before: Traditional HttpClient

Here's typical HttpClient code you might have today:

```csharp
public class UserService
{
    private readonly HttpClient _httpClient;

    public UserService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<User?> GetUserAsync(string userId)
    {
        try
        {
            var response = await _httpClient.GetAsync($"/users/{userId}");

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<User>();
            }

            // How do we handle errors? Return null? Throw?
            return null;
        }
        catch (HttpRequestException)
        {
            // Network error - return null? throw? log?
            return null;
        }
        catch (JsonException)
        {
            // Deserialization error
            return null;
        }
    }
}
```

Problems with this approach:

1. **Null return hides errors** - Callers don't know if user wasn't found or network failed
2. **Error handling is scattered** - Multiple catch blocks, inconsistent handling
3. **Easy to miss exceptions** - What about `TaskCanceledException`?
4. **Response status lost** - We can't tell 404 from 500

## After: RestClient.Net

Here's the same service with RestClient.Net:

```csharp
public class UserService
{
    private readonly HttpClient _httpClient;

    public UserService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public Task<Result<User, HttpError<ApiError>>> GetUserAsync(
        string userId,
        CancellationToken ct = default) =>
        _httpClient.GetAsync(
            url: $"/users/{userId}".ToAbsoluteUrl(),
            deserializeSuccess: async (content, ct) =>
                await content.ReadFromJsonAsync<User>(ct)
                ?? throw new InvalidOperationException("Null response"),
            deserializeError: async (content, ct) =>
                await content.ReadFromJsonAsync<ApiError>(ct)
                ?? new ApiError("Unknown error"),
            cancellationToken: ct
        );
}
```

Benefits:

1. **Return type tells the full story** - Success or error, with details
2. **No exceptions to catch** - Everything is in the return type
3. **Compiler enforces handling** - With Exhaustion analyzer
4. **Status codes preserved** - In `ResponseError`

## Step-by-Step Migration

### Step 1: Install RestClient.Net

```bash
dotnet add package RestClient.Net
```

### Step 2: Define Error Types

Create a model for your API errors:

```csharp
// Models/ApiError.cs
public sealed record ApiError(string Message, string? Code = null);
```

### Step 3: Create Type Aliases

Add to `GlobalUsings.cs`:

```csharp
// For User
global using OkUser = Outcome.Result<User, Outcome.HttpError<ApiError>>
    .Ok<User, Outcome.HttpError<ApiError>>;
global using ErrorUser = Outcome.Result<User, Outcome.HttpError<ApiError>>
    .Error<User, Outcome.HttpError<ApiError>>;
global using ResponseErrorUser = Outcome.HttpError<ApiError>.ErrorResponseError;
global using ExceptionErrorUser = Outcome.HttpError<ApiError>.ExceptionError;
```

### Step 4: Update Service Methods

Change from:

```csharp
public async Task<User?> GetUserAsync(string userId)
```

To:

```csharp
public Task<Result<User, HttpError<ApiError>>> GetUserAsync(
    string userId,
    CancellationToken ct = default)
```

### Step 5: Update Callers

Before:

```csharp
var user = await userService.GetUserAsync("123");
if (user == null)
{
    // Handle error... somehow
    return;
}
Console.WriteLine(user.Name);
```

After:

```csharp
var result = await userService.GetUserAsync("123");

var message = result switch
{
    OkUser(var user) => user.Name,
    ErrorUser(ResponseErrorUser(_, HttpStatusCode.NotFound, _)) => "User not found",
    ErrorUser(ResponseErrorUser(var err, var status, _)) => $"Error {status}: {err.Message}",
    ErrorUser(ExceptionErrorUser(var ex)) => $"Network error: {ex.Message}",
};
```

## Common Patterns

### Returning Early on Error

Before:

```csharp
var user = await GetUserAsync(userId);
if (user == null) return null;

var orders = await GetOrdersAsync(user.Id);
if (orders == null) return null;

return new Dashboard(user, orders);
```

After:

```csharp
var userResult = await GetUserAsync(userId);
var ordersResult = await GetOrdersAsync(userId);

return (userResult, ordersResult) switch
{
    (OkUser(var user), OkOrders(var orders)) => new Dashboard(user, orders),
    (ErrorUser(var e), _) => throw new Exception($"User error: {e}"),
    (_, ErrorOrders(var e)) => throw new Exception($"Orders error: {e}"),
};
```

### Converting to Nullable (Escape Hatch)

If you need to maintain the old interface temporarily:

```csharp
public async Task<User?> GetUserOrNullAsync(string userId)
{
    var result = await GetUserAsync(userId);
    return result switch
    {
        OkUser(var user) => user,
        _ => null,
    };
}
```

### Logging Errors

```csharp
var result = await GetUserAsync(userId);

var user = result switch
{
    OkUser(var u) => u,
    ErrorUser(ResponseErrorUser(var err, var status, _)) =>
    {
        _logger.LogWarning("API error {Status}: {Message}", status, err.Message);
        return null;
    },
    ErrorUser(ExceptionErrorUser(var ex)) =>
    {
        _logger.LogError(ex, "Network error getting user {UserId}", userId);
        return null;
    },
};
```

## Gradual Migration

You don't have to migrate everything at once:

1. **Start with new code** - Use RestClient.Net for new endpoints
2. **Migrate high-traffic paths** - Critical code benefits most
3. **Create adapters** - Wrap old code if needed
4. **Update tests** - Add tests for all cases

## Testing After Migration

Ensure you test all cases:

```csharp
[Fact]
public async Task GetUser_ReturnsUser_WhenFound()
{
    // Arrange - setup mock to return 200
    var result = await service.GetUserAsync("123");
    Assert.IsType<OkUser>(result);
}

[Fact]
public async Task GetUser_ReturnsResponseError_WhenNotFound()
{
    // Arrange - setup mock to return 404
    var result = await service.GetUserAsync("unknown");
    Assert.IsType<ErrorUser>(result);
    var error = ((ErrorUser)result).Value;
    Assert.IsType<ResponseErrorUser>(error);
}

[Fact]
public async Task GetUser_ReturnsExceptionError_WhenNetworkFails()
{
    // Arrange - setup mock to throw
    var result = await service.GetUserAsync("123");
    var error = ((ErrorUser)result).Value;
    Assert.IsType<ExceptionErrorUser>(error);
}
```

## Summary

Migrating to RestClient.Net provides:

- **Explicit error handling** - The type system enforces it
- **Better error information** - Status codes, response bodies, exceptions
- **Compile-time safety** - With Exhaustion analyzer
- **Cleaner code** - No scattered try/catch blocks

Start small, migrate gradually, and enjoy the safety of functional error handling.
