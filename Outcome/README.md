# Outcome

Result types with closed type hierarchies for type-safe error handling without exceptions, and exhaustive pattern matching.

## What it does

This library implements the Railway Oriented Programming pattern with Result types that force explicit error handling. A Result represents either success (Ok) or failure (Error), eliminating null checks and exception handling. When used with the Exhaustion analyzer, switches have exhaustive pattern matching checks.

## Features

- Type-safe error handling without exceptions
- Exhaustive pattern matching with closed type hierarchies
- Monadic operations (Map, Bind, Match)
- Sequence/traverse for collections
- Works with the Exhaustion analyzer for compile-time exhaustiveness checking

## Installation

```bash
dotnet add package Outcome
```

## Examples

### Basic Usage

```csharp
using Outcome;

// Create results
Result<int, string> success = new Result<int, string>.Ok<int, string>(42);
Result<int, string> failure = Result<int, string>.Failure("Something went wrong");

// Pattern matching (exhaustive with Exhaustion analyzer)
var message = result switch
{
    Result<int, string>.Ok(var value) => $"Success: {value}",
    Result<int, string>.Error(var error) => $"Error: {error}"
};
```

### Functional Composition

```csharp
// Map transforms success values
Result<int, string> GetUserId() => new Result<int, string>.Ok<int, string>(123);

var userAge = GetUserId()
    .Map(id => id * 2)
    .Map(id => $"User age: {id}");

// Bind chains operations that return Results
Result<User, string> GetUser(int id) => /* ... */;
Result<Order[], string> GetOrders(User user) => /* ... */;

var orders = GetUserId()
    .Bind(GetUser)
    .Bind(GetOrders);
```

### Error Handling

```csharp
// Match handles both cases
var output = result.Match(
    onSuccess: value => $"Got {value}",
    onError: error => $"Failed: {error}"
);

// GetValueOrDefault provides fallbacks
var value = result.GetValueOrDefault(0);

// Tap performs side effects without changing the result
result.Tap(
    onSuccess: v => Console.WriteLine($"Success: {v}"),
    onError: e => Console.WriteLine($"Error: {e}")
);
```

### Collections

```csharp
// Sequence converts List<Result<T, E>> to Result<List<T>, E>
// Returns first error or all successes
var results = new[] { result1, result2, result3 };
Result<IReadOnlyList<int>, string> combined = results.Sequence();
```

## HttpError Types

The library includes specialized error types for HTTP scenarios:

```csharp
// HttpError<TError> represents HTTP-specific failures
public abstract record HttpError<TError>
{
    public sealed record ExceptionError(Exception Exception) : HttpError<TError>;
    public sealed record ErrorResponseError(TError Body, int StatusCode) : HttpError<TError>;
}

// Usage with RestClient.Net
Result<User, HttpError<ApiError>> result = await client.GetAsync<User, ApiError>(...);

var message = result switch
{
    Result<User, HttpError<ApiError>>.Ok(var user) => $"User: {user.Name}",
    Result<User, HttpError<ApiError>>.Error(HttpError<ApiError>.ExceptionError(var ex)) =>
        $"Exception: {ex.Message}",
    Result<User, HttpError<ApiError>>.Error(HttpError<ApiError>.ErrorResponseError(var body, var status)) =>
        $"HTTP {status}: {body.Message}"
};
```

## Why Use Results?

- **No null reference exceptions**: Errors are explicit values
- **No uncaught exceptions**: All errors are handled or propagated as values
- **Compiler-enforced error handling**: Can't ignore errors
- **Composable**: Chain operations with Map/Bind
- **Exhaustive**: Works with Exhaustion analyzer for complete pattern matching
