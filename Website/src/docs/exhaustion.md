---
layout: layouts/docs.njk
title: Exhaustion Analyzer
description: Roslyn analyzer for compile-time exhaustiveness checking in C# switch expressions. Catch missing cases before runtime with the Exhaustion analyzer.
keywords: exhaustiveness checking, C# analyzer, Roslyn analyzer, switch expression, pattern matching
eleventyNavigation:
  key: Exhaustion
  order: 8
faq:
  - question: What is exhaustiveness checking?
    answer: Exhaustiveness checking verifies that a switch expression handles all possible cases of a discriminated union, turning runtime crashes into compile-time errors.
  - question: Is Exhaustion included with RestClient.Net?
    answer: Yes, the Exhaustion analyzer is automatically included when you install RestClient.Net. You can also install it separately for other use cases.
  - question: What error code does Exhaustion use?
    answer: The analyzer uses EXHAUSTION001 for non-exhaustive switch expressions, providing details about which cases are matched and which are missing.
---

# Exhaustion Analyzer

The Exhaustion analyzer is a Roslyn analyzer that ensures your switch expressions handle all possible cases of discriminated unions at compile time.

## The Problem

C#'s default switch exhaustiveness checking is limited. This code compiles but crashes at runtime:

```csharp
// This compiles without warnings!
var output = result switch
{
    OkPost(var post) => $"Success: {post.Title}",
    ErrorPost(ResponseErrorPost(var err, var status, _)) => $"Error {status}",
    // Missing ExceptionErrorPost case - runtime crash!
};
```

When an `ExceptionError` occurs, you get a `MatchException` at runtime. In production. On a Friday afternoon.

## The Solution

With Exhaustion installed, the same code produces a compile error:

```
error EXHAUSTION001: Switch on Result is not exhaustive;
Matched: Ok<Post, HttpError<ErrorResponse>>, Error<Post, HttpError<ErrorResponse>> with ErrorResponseError<ErrorResponse>
Missing: Error<Post, HttpError<ErrorResponse>> with ExceptionError<ErrorResponse>
```

**Runtime crashes become compile-time errors.**

## Installation

### With RestClient.Net

Exhaustion is automatically included:

```bash
dotnet add package RestClient.Net
```

### Standalone

For use with your own discriminated unions:

```bash
dotnet add package Exhaustion
```

## How It Works

Exhaustion uses Roslyn's analyzer API to:

1. Detect switch expressions on Result types
2. Analyze which patterns are matched
3. Determine which cases are missing
4. Report compile-time errors for incomplete switches

### Supported Types

Exhaustion works with:

- `Result<TSuccess, TError>` from RestClient.Net
- `HttpError<TError>` from RestClient.Net
- Custom discriminated unions following the same pattern

## Error Messages

### EXHAUSTION001: Non-exhaustive switch

```
error EXHAUSTION001: Switch on Result is not exhaustive;
Matched: Ok<User, HttpError<ApiError>>
Missing: Error<User, HttpError<ApiError>> with ErrorResponseError, Error<User, HttpError<ApiError>> with ExceptionError
```

The error tells you:
- What type the switch is on
- Which cases ARE matched
- Which cases are MISSING

## Examples

### Missing HttpError Cases

```csharp
// ERROR: Missing both error cases
var message = result switch
{
    OkUser(var user) => user.Name,
};

// ERROR: Missing ExceptionError
var message = result switch
{
    OkUser(var user) => user.Name,
    ErrorUser(ResponseErrorUser(var err, _, _)) => err.Message,
};

// CORRECT: All cases handled
var message = result switch
{
    OkUser(var user) => user.Name,
    ErrorUser(ResponseErrorUser(var err, _, _)) => err.Message,
    ErrorUser(ExceptionErrorUser(var ex)) => ex.Message,
};
```

### Using Discard Pattern

You can use discard for cases you want to handle uniformly:

```csharp
// CORRECT: Discard covers remaining cases
var message = result switch
{
    OkUser(var user) => user.Name,
    _ => "Error occurred",
};
```

However, this defeats the purpose of exhaustiveness checking. Use it sparingly.

### Multiple Result Types

Each Result type needs its own handling:

```csharp
var (userResult, ordersResult) = await (GetUserAsync(), GetOrdersAsync());

// Both must be exhaustively handled
var userMessage = userResult switch
{
    OkUser(var user) => user.Name,
    ErrorUser(ResponseErrorUser(var err, _, _)) => err.Message,
    ErrorUser(ExceptionErrorUser(var ex)) => ex.Message,
};

var ordersMessage = ordersResult switch
{
    OkOrders(var orders) => $"{orders.Count} orders",
    ErrorOrders(ResponseErrorOrders(var err, _, _)) => err.Message,
    ErrorOrders(ExceptionErrorOrders(var ex)) => ex.Message,
};
```

## Creating Exhaustive Custom Types

You can use Exhaustion with your own discriminated unions:

```csharp
// Define a discriminated union
public abstract record PaymentResult
{
    public sealed record Success(string TransactionId) : PaymentResult;
    public sealed record Declined(string Reason) : PaymentResult;
    public sealed record Error(Exception Exception) : PaymentResult;
}

// Exhaustion will check switches on PaymentResult
var message = paymentResult switch
{
    PaymentResult.Success(var txId) => $"Paid: {txId}",
    PaymentResult.Declined(var reason) => $"Declined: {reason}",
    PaymentResult.Error(var ex) => $"Error: {ex.Message}",
};
```

## Configuration

### Suppress for Specific Lines

In rare cases, suppress the warning:

```csharp
#pragma warning disable EXHAUSTION001
var message = result switch
{
    OkUser(var user) => user.Name,
    // Intentionally incomplete
};
#pragma warning restore EXHAUSTION001
```

### Global Suppression

In `.editorconfig` (not recommended):

```ini
[*.cs]
dotnet_diagnostic.EXHAUSTION001.severity = none
```

## IDE Integration

### Visual Studio

Exhaustion errors appear in the Error List with full details. Click the error to navigate to the problematic switch.

### VS Code

With the C# extension, errors appear in the Problems panel and as red squiggles in the editor.

### Rider

JetBrains Rider fully supports Roslyn analyzers. Errors appear in the inspection results.

## Benefits

1. **Catch errors early** - Compile time, not runtime
2. **Safer refactoring** - Add a new case? Compiler finds all switches
3. **Self-documenting** - The types show all possible outcomes
4. **Better code review** - Reviewers don't need to check for missing cases

## Best Practices

1. **Never suppress without a comment** explaining why
2. **Avoid discard patterns** (`_`) unless intentional
3. **Handle each case explicitly** for clarity
4. **Use type aliases** for cleaner pattern matching
5. **Let the analyzer guide you** when types change

## Troubleshooting

### Analyzer Not Working

Ensure the package is installed:

```bash
dotnet list package | grep Exhaustion
```

Rebuild the project:

```bash
dotnet build --no-incremental
```

### False Positives

If Exhaustion reports an error incorrectly, please [report an issue](https://github.com/MelbourneDeveloper/RestClient.Net/issues).

## Next Steps

- [Error Handling](/docs/error-handling/) - Result type patterns
- [Basic Usage](/docs/basic-usage/) - Getting started guide
- [API Reference](/api/result-types/) - Result type documentation
