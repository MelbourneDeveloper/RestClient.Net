---
layout: layouts/blog.njk
title: Functional Error Handling in C# with RestClient.Net
date: 2026-01-15
author: Christian Findlay
tags: posts
excerpt: Learn how discriminated unions and exhaustiveness checking eliminate runtime crashes in your REST calls.
---

# Functional Error Handling in C#

Traditional C# error handling relies heavily on exceptions. But exceptions have problems:

1. **They're invisible** - Nothing in the type signature tells you what might throw
2. **They're easy to forget** - Miss a catch block and your app crashes
3. **They're expensive** - Stack traces have performance overhead

## A Better Way: Result Types

RestClient.Net uses discriminated unions to represent all possible outcomes:

```csharp
// The type tells you everything that can happen
Result<Post, HttpError<ErrorResponse>> result = await httpClient.GetAsync(...);
```

## Pattern Matching

C#'s pattern matching makes working with results elegant:

```csharp
var message = result switch
{
    OkPost(var post) => $"Got post: {post.Title}",
    ErrorPost(ResponseErrorPost(var err, var status, _)) => $"API error {status}: {err.Message}",
    ErrorPost(ExceptionErrorPost(var ex)) => $"Network error: {ex.Message}",
};
```

## Exhaustiveness = Safety

The Exhaustion analyzer turns forgotten cases into compile errors. This is the key insight: **runtime crashes become compile-time errors**.

```csharp
// This won't compile!
var message = result switch
{
    OkPost(var post) => "Success",
    // Missing ErrorPost cases - COMPILE ERROR
};
```

## Why This Matters

When every possible outcome is explicit in the type system:

- New team members can't accidentally skip error handling
- Refactoring is safe - the compiler catches missing cases
- Code review focuses on logic, not "did they handle errors?"

Try RestClient.Net today and experience the safety of functional error handling in C#.
