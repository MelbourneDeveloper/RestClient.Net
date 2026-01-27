---
layout: layouts/docs.njk
title: Result Type
eleventyNavigation:
  key: Result Type
  parent: API Reference
  order: 5
---

# Result&lt;TSuccess, TFailure&gt;

A discriminated union representing either a successful result or a failure. This type enforces explicit error handling and eliminates the need for exceptions in business logic.

**Namespace:** `Outcome`

## Overview

The `Result` type follows the Railway Oriented Programming pattern. Every operation returns either:
- `Ok<TSuccess>` - The "happy path" containing the success value
- `Error<TFailure>` - The "sad path" containing the error value

```csharp
public abstract partial record Result<TSuccess, TFailure>
```

## Nested Types

### Ok&lt;TSuccess, TFailure&gt;

Represents a successful computation.

```csharp
public sealed record Ok<TSuccess, TFailure>(TSuccess Value)
    : Result<TSuccess, TFailure>
```

### Error&lt;TSuccess, TFailure&gt;

Represents a failed computation.

```csharp
public sealed record Error<TSuccess, TFailure>(TFailure Value)
    : Result<TSuccess, TFailure>
```

---

## Properties

### IsOk

Returns `true` if this result represents a successful computation.

```csharp
public bool IsOk { get; }
```

### IsError

Returns `true` if this result represents a failed computation.

```csharp
public bool IsError { get; }
```

### Example

```csharp
if (result.IsOk)
{
    // Handle success
}
else
{
    // Handle error
}
```

---

## Static Methods

### Failure

Creates a failed result containing the specified error.

```csharp
public static Result<TSuccess, TFailure> Failure(TFailure error)
```

### Example

```csharp
Result<User, string> result = Result<User, string>.Failure("User not found");
```

---

## Instance Methods

### Map&lt;TNewSuccess&gt;

Transforms the success value using the provided function. If this is an Error, the error is propagated unchanged.

```csharp
public Result<TNewSuccess, TFailure> Map<TNewSuccess>(
    Func<TSuccess, TNewSuccess> mapper
)
```

This is the fundamental Functor operation for Result.

### Example

```csharp
Result<int, string> numberResult = GetNumber(); // Returns Ok(42)
Result<string, string> stringResult = numberResult.Map(n => n.ToString());
// stringResult is Ok("42")

Result<int, string> errorResult = Result<int, string>.Failure("Error");
Result<string, string> mappedError = errorResult.Map(n => n.ToString());
// mappedError is Error("Error") - map function is not called
```

---

### MapError&lt;TNewFailure&gt;

Transforms the error value using the provided function. If this is an Ok, the success value is propagated unchanged.

```csharp
public Result<TSuccess, TNewFailure> MapError<TNewFailure>(
    Func<TFailure, TNewFailure> mapper
)
```

### Example

```csharp
Result<User, string> result = GetUser(); // Returns Error("Not found")
Result<User, CustomError> mappedResult = result.MapError(msg => new CustomError(msg));
// mappedResult is Error(CustomError("Not found"))
```

---

### Bind&lt;TNewSuccess&gt;

Monadic bind operation. Applies a function that returns a Result to the success value, flattening the nested Result.

```csharp
public Result<TNewSuccess, TFailure> Bind<TNewSuccess>(
    Func<TSuccess, Result<TNewSuccess, TFailure>> binder
)
```

This is the core operation that makes Result a monad, enabling Railway Oriented Programming.

### Example

```csharp
Result<int, string> Validate(string input)
{
    return int.TryParse(input, out var number)
        ? new Result<int, string>.Ok<int, string>(number)
        : Result<int, string>.Failure("Invalid number");
}

Result<int, string> MultiplyByTwo(int n)
{
    return new Result<int, string>.Ok<int, string>(n * 2);
}

Result<int, string> result = Validate("21").Bind(MultiplyByTwo);
// result is Ok(42)

Result<int, string> errorResult = Validate("abc").Bind(MultiplyByTwo);
// errorResult is Error("Invalid number") - MultiplyByTwo is not called
```

---

### Match&lt;TResult&gt;

Applies one of two functions based on the state of this Result. This is the catamorphism for Result.

```csharp
public TResult Match<TResult>(
    Func<TSuccess, TResult> onSuccess,
    Func<TFailure, TResult> onError
)
```

### Example

```csharp
string message = result.Match(
    onSuccess: user => $"Hello, {user.Name}!",
    onError: error => $"Error: {error}"
);
```

---

### Tap

Performs a side effect based on the state without changing the Result.

```csharp
public Result<TSuccess, TFailure> Tap(
    Action<TSuccess>? onSuccess = null,
    Action<TFailure>? onError = null
)
```

Use this for logging, debugging, or other side effects while preserving the functional chain.

### Example

```csharp
var result = GetUser()
    .Tap(
        onSuccess: user => logger.LogInformation("Found user: {Name}", user.Name),
        onError: error => logger.LogWarning("User lookup failed: {Error}", error)
    )
    .Map(user => user.Email);
```

---

### GetValueOrDefault

Returns the success value if Ok, otherwise returns the provided default.

```csharp
public TSuccess GetValueOrDefault(TSuccess defaultValue)
public TSuccess GetValueOrDefault(Func<TSuccess> defaultProvider)
```

### Example

```csharp
string name = result.GetValueOrDefault("Unknown User");

// Lazy evaluation
string lazyName = result.GetValueOrDefault(() => ExpensiveDefaultCalculation());
```

---

## Operators

### ! (Logical Not)

Extracts the error value if Error, throws if Ok.

```csharp
public static TFailure operator !(Result<TSuccess, TFailure> result)
```

### Example

```csharp
Result<User, string> result = Result<User, string>.Failure("Not found");
string error = !result; // "Not found"
```

### + (Unary Plus)

Extracts the success value if Ok, throws if Error. **Use only in tests.**

```csharp
public static TSuccess operator +(Result<TSuccess, TFailure> result)
```

### Example

```csharp
// In tests only!
Result<User, string> result = GetUser();
User user = +result; // Throws if Error
```

---

## Pattern Matching

The recommended way to handle Results is with pattern matching:

```csharp
var output = result switch
{
    Result<User, ApiError>.Ok<User, ApiError>(var user)
        => $"Found: {user.Name}",

    Result<User, ApiError>.Error<User, ApiError>(var error)
        => $"Error: {error.Message}",
};
```

### With Type Aliases

Define type aliases in `GlobalUsings.cs` for cleaner code:

```csharp
global using OkUser = Outcome.Result<User, ApiError>
    .Ok<User, ApiError>;
global using ErrorUser = Outcome.Result<User, ApiError>
    .Error<User, ApiError>;
```

Then pattern match becomes:

```csharp
var output = result switch
{
    OkUser(var user) => $"Found: {user.Name}",
    ErrorUser(var error) => $"Error: {error.Message}",
};
```

---

## Railway Oriented Programming

Chain operations that may fail:

```csharp
var result = await ValidateInput(input)
    .Bind(ParseData)
    .Bind(TransformData)
    .Map(FormatOutput);

// If any step fails, subsequent steps are skipped
// and the first error is propagated
```

---

## Best Practices

1. **Prefer pattern matching** over `IsOk`/`IsError` checks
2. **Use `Bind`** for operations that return Results
3. **Use `Map`** for pure transformations
4. **Use `Tap`** for side effects (logging, metrics)
5. **Define type aliases** for frequently used Result types
6. **Never use `+` operator** outside of tests
