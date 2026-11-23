# Exhaustion Analyzer

A Roslyn analyzer that enforces exhaustive pattern matching for closed type hierarchies in C#.

## What it does

This analyzer ensures that switch expressions and statements are exhaustive when matching against closed type hierarchies. A closed hierarchy is a type with:

- An abstract or record type with private/protected constructors
- At least one derived type nested within it

## Features

- Detects missing cases in pattern matching
- Reports warnings when a discard pattern (`_`) is used instead of explicit matching
- Detects redundant default arms when all types are already matched
- Detects redundant default arms on sealed types (leaf nodes in hierarchies)
- Supports nested closed hierarchies
- Works with both switch expressions and switch statements
- Handles generic types and type parameters

## Installation

Install via NuGet:

```bash
dotnet add package Exhaustion
```

## Diagnostic ID

**EXHAUSTION001**: Switch expression/statement must be exhaustive for closed type hierarchies

## Examples

### Missing Cases

```csharp
public abstract record Result<TSuccess, TError>
{
    private Result() { }

    public sealed record Ok(TSuccess Value) : Result<TSuccess, TError>;
    public sealed record Error(TError Value) : Result<TSuccess, TError>;
}

// ❌ Error: missing Error case
var message = result switch
{
    Result<int, string>.Ok(var value) => $"Success: {value}"
    // Missing: Result<int, string>.Error case
};
```

### Redundant Default Arms

```csharp
// ❌ Error: redundant default arm - all cases already matched
var message = result switch
{
    Result<int, string>.Ok(var value) => $"Success: {value}",
    Result<int, string>.Error(var error) => $"Error: {error}",
    _ => throw new InvalidOperationException() // Redundant!
};

// ✅ Correct: no default arm needed
var message = result switch
{
    Result<int, string>.Ok(var value) => $"Success: {value}",
    Result<int, string>.Error(var error) => $"Error: {error}"
};
```

### Sealed Types

```csharp
public abstract record HttpError<TError>
{
    private HttpError() { }

    public sealed record ExceptionError(Exception Exception) : HttpError<TError>;
    public sealed record ErrorResponseError(TError Body, int StatusCode) : HttpError<TError>;
}

// If httpError is already known to be ErrorResponseError:
var httpError = (HttpError<string>.ErrorResponseError)!result;

// ❌ Error: redundant pattern matching on sealed type
var (body, statusCode) = httpError switch
{
    HttpError<string>.ErrorResponseError(var b, var sc) => (b, sc),
    _ => throw new InvalidOperationException() // Redundant!
};

// ✅ Correct: direct deconstruction
var (body, statusCode) = httpError;
```
