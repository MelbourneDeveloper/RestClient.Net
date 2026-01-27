---
layout: layouts/docs.njk
title: Unit Type
eleventyNavigation:
  key: Unit Type
  parent: API Reference
  order: 8
---

# Unit

Represents a void return type or a type with exactly one value. Unit is the functional programming equivalent of `void`.

**Namespace:** `Outcome`

## Overview

In functional programming, every function must return a value. When an operation does not have a meaningful return value (like deleting a resource), we use `Unit` instead of `void`.

```csharp
public sealed record Unit
{
    public static Unit Value { get; }
    private Unit() { }
}
```

## Why Unit?

### The Problem with Void

`void` cannot be used as a type parameter:

```csharp
// This doesn't compile!
Result<void, Error> result = DeleteUser(id);
```

### The Solution: Unit

```csharp
// This works!
Result<Unit, Error> result = DeleteUser(id);
```

---

## Properties

### Value

The single instance of Unit.

```csharp
public static Unit Value { get; }
```

### Example

```csharp
Unit unit = Unit.Value;
```

---

## Usage with Result

### Delete Operations

```csharp
public async Task<Result<Unit, HttpError<ApiError>>> DeleteUserAsync(int id)
{
    return await httpClient.DeleteAsync(
        url: $"https://api.example.com/users/{id}".ToAbsoluteUrl(),
        deserializeSuccess: async (response, ct) => Unit.Value,
        deserializeError: DeserializeJson<ApiError>
    );
}

// Usage
var result = await DeleteUserAsync(42);

var message = result switch
{
    Result<Unit, HttpError<ApiError>>.Ok<Unit, HttpError<ApiError>>(_)
        => "User deleted successfully",
    Result<Unit, HttpError<ApiError>>.Error<Unit, HttpError<ApiError>>(var error)
        => $"Failed to delete: {error}"
};
```

### Download Operations

```csharp
public Task<Result<Unit, HttpError<ApiError>>> DownloadFileAsync(
    string url,
    Stream destination
) =>
    httpClient.DownloadFileAsync(
        url: url.ToAbsoluteUrl(),
        destinationStream: destination,
        deserializeError: DeserializeJson<ApiError>
    );
```

### Fire-and-Forget with Confirmation

```csharp
public async Task<Result<Unit, HttpError<ApiError>>> SendNotificationAsync(
    Notification notification
)
{
    return await httpClient.PostAsync(
        url: "https://api.example.com/notifications".ToAbsoluteUrl(),
        requestBody: SerializeJson(notification),
        deserializeSuccess: async (response, ct) => Unit.Value,
        deserializeError: DeserializeJson<ApiError>
    );
}
```

---

## Pattern Matching

```csharp
var result = await DeleteUserAsync(42);

result.Match(
    onSuccess: _ =>
    {
        Console.WriteLine("Deleted successfully");
        return Unit.Value; // Must return something
    },
    onError: error =>
    {
        Console.WriteLine($"Error: {error}");
        return Unit.Value;
    }
);
```

### Ignoring the Unit Value

```csharp
if (result.IsOk)
{
    Console.WriteLine("Success!");
}

// Or with pattern matching
if (result is Result<Unit, HttpError<ApiError>>.Ok<Unit, HttpError<ApiError>>)
{
    Console.WriteLine("Success!");
}
```

---

## Creating Unit Results

### Success

```csharp
var success = new Result<Unit, string>.Ok<Unit, string>(Unit.Value);
```

### From Async Operations

```csharp
async Task<Result<Unit, Error>> DoSomethingAsync()
{
    try
    {
        await SomeOperation();
        return new Result<Unit, Error>.Ok<Unit, Error>(Unit.Value);
    }
    catch (Exception ex)
    {
        return Result<Unit, Error>.Failure(new Error(ex.Message));
    }
}
```

---

## Comparison with Void

| Void | Unit |
|------|------|
| Cannot be used as type parameter | Can be used as type parameter |
| No value | Has exactly one value |
| `void Method()` | `Unit Method()` |
| Cannot be stored in variables | Can be stored in variables |
| Cannot be returned from lambdas in some contexts | Always returnable |

---

## Functional Programming Context

In functional programming languages like F# and Haskell, Unit (or `()`) is fundamental:

```fsharp
// F#
let deleteUser id : Async<Result<unit, Error>> = ...
```

```haskell
-- Haskell
deleteUser :: Int -> IO (Either Error ())
```

RestClient.Net brings this same pattern to C# for consistent functional programming.

---

## Best Practices

1. **Use `Unit.Value`** - Never create new Unit instances
2. **Prefer Unit over null** - Unit explicitly indicates "no meaningful value"
3. **Pattern match on success** - Use `_` to ignore the Unit value
4. **Keep return types consistent** - All HTTP operations return `Result<T, Error>`
