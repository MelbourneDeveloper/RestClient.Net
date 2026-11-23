---
layout: layouts/docs.njk
title: Result Extensions
eleventyNavigation:
  key: Result Extensions
  parent: API Reference
  order: 7
---

# ResultExtensions

Extension methods that provide additional functional programming utilities for the Result type.

**Namespace:** `Outcome`

## Sequence

Transforms a sequence of Results into a Result of a sequence. If all Results are Ok, returns Ok with all values. If any Result is Error, returns the first Error encountered.

```csharp
public static Result<IReadOnlyList<TSuccess>, TFailure> Sequence<TSuccess, TFailure>(
    this IEnumerable<Result<TSuccess, TFailure>> results
)
```

This is the sequence/traverse operation for Result, enabling all-or-nothing semantics for collections.

### Example

```csharp
var results = new[]
{
    new Result<int, string>.Ok<int, string>(1),
    new Result<int, string>.Ok<int, string>(2),
    new Result<int, string>.Ok<int, string>(3),
};

var combined = results.Sequence();
// combined is Ok([1, 2, 3])

var withError = new[]
{
    new Result<int, string>.Ok<int, string>(1),
    Result<int, string>.Failure("Error on item 2"),
    new Result<int, string>.Ok<int, string>(3),
};

var combinedWithError = withError.Sequence();
// combinedWithError is Error("Error on item 2")
```

### Use Case: Batch Operations

```csharp
var userIds = new[] { 1, 2, 3, 4, 5 };

var results = await Task.WhenAll(
    userIds.Select(id => httpClient.GetAsync<User, ApiError>(...))
);

var allUsers = results.Sequence();
// Either Ok with all users, or Error with first failure
```

---

## Flatten

Flattens a nested Result structure.

```csharp
public static Result<TSuccess, TFailure> Flatten<TSuccess, TFailure>(
    this Result<Result<TSuccess, TFailure>, TFailure> result
)
```

### Example

```csharp
Result<Result<int, string>, string> nested =
    new Result<Result<int, string>, string>.Ok<Result<int, string>, string>(
        new Result<int, string>.Ok<int, string>(42)
    );

Result<int, string> flattened = nested.Flatten();
// flattened is Ok(42)
```

---

## Combine

Combines two Results using a combining function. Both Results must be Ok for the combination to succeed.

```csharp
public static Result<TSuccess, TFailure> Combine<TSuccess1, TSuccess2, TSuccess, TFailure>(
    this Result<TSuccess1, TFailure> result1,
    Result<TSuccess2, TFailure> result2,
    Func<TSuccess1, TSuccess2, TSuccess> combiner
)
```

### Example

```csharp
var nameResult = new Result<string, string>.Ok<string, string>("John");
var ageResult = new Result<int, string>.Ok<int, string>(30);

var combined = nameResult.Combine(
    ageResult,
    (name, age) => new Person(name, age)
);
// combined is Ok(Person("John", 30))

var errorName = Result<string, string>.Failure("Name required");
var combinedWithError = errorName.Combine(
    ageResult,
    (name, age) => new Person(name, age)
);
// combinedWithError is Error("Name required")
```

### Use Case: Validation

```csharp
var validatedName = ValidateName(input.Name);
var validatedEmail = ValidateEmail(input.Email);
var validatedAge = ValidateAge(input.Age);

// Combine all validations
var userResult = validatedName
    .Combine(validatedEmail, (name, email) => (name, email))
    .Combine(validatedAge, (pair, age) => new User(pair.name, pair.email, age));
```

---

## Filter

Filters a Result based on a predicate.

```csharp
public static Result<TSuccess, TFailure> Filter<TSuccess, TFailure>(
    this Result<TSuccess, TFailure> result,
    Func<TSuccess, bool> predicate,
    TFailure errorOnFalse
)
```

- If Ok and predicate returns true: returns the original Result
- If Ok and predicate returns false: returns Error with the provided error
- If already Error: returns unchanged

### Example

```csharp
var result = new Result<int, string>.Ok<int, string>(5);

var filtered = result.Filter(
    n => n > 0,
    "Number must be positive"
);
// filtered is Ok(5)

var filteredNegative = result.Filter(
    n => n > 10,
    "Number must be greater than 10"
);
// filteredNegative is Error("Number must be greater than 10")
```

### Use Case: Post-Validation

```csharp
var user = await GetUserAsync(id);

var validUser = user.Filter(
    u => u.IsActive,
    HttpError<ApiError>.FromErrorResponse(
        new ApiError("User account is deactivated"),
        HttpStatusCode.Forbidden,
        null!
    )
);
```

---

## GetValueOrThrow

**Warning: Dangerous method - use with caution!**

Extracts the success value or throws an exception. Only use in tests.

```csharp
public static TSuccess GetValueOrThrow<TSuccess, TFailure>(
    this Result<TSuccess, TFailure> result,
    string errorMessage = "Expected success result"
)
```

### Example

```csharp
// In tests only!
[Test]
public async Task GetUser_ReturnsUser()
{
    var result = await _userService.GetUserAsync(1);

    var user = result.GetValueOrThrow("Expected user to be found");

    Assert.AreEqual("John", user.Name);
}
```

---

## GetErrorOrThrow

**Warning: Dangerous method - use with caution!**

Extracts the error value or throws an exception. Only use in tests.

```csharp
public static TFailure GetErrorOrThrow<TSuccess, TFailure>(
    this Result<TSuccess, TFailure> result,
    string errorMessage = "Expected error result"
)
```

### Example

```csharp
// In tests only!
[Test]
public async Task GetUser_InvalidId_ReturnsNotFound()
{
    var result = await _userService.GetUserAsync(-1);

    var error = result.GetErrorOrThrow("Expected error for invalid ID");

    Assert.IsTrue(error.IsErrorResponse);
}
```

---

## Chaining Extensions

Extensions can be chained for expressive code:

```csharp
var result = await httpClient.GetAsync<User, ApiError>(...)
    .Map(user => user with { LastAccessed = DateTime.UtcNow })
    .Filter(user => user.IsActive, CreateDeactivatedError())
    .Tap(user => logger.LogInformation("Fetched user {Id}", user.Id));
```

---

## Async Extensions (Custom)

You can create async versions for your project:

```csharp
public static class AsyncResultExtensions
{
    public static async Task<Result<TNew, TFailure>> MapAsync<TSuccess, TNew, TFailure>(
        this Task<Result<TSuccess, TFailure>> resultTask,
        Func<TSuccess, Task<TNew>> mapper
    )
    {
        var result = await resultTask;
        return result switch
        {
            Result<TSuccess, TFailure>.Ok<TSuccess, TFailure>(var value)
                => new Result<TNew, TFailure>.Ok<TNew, TFailure>(await mapper(value)),
            Result<TSuccess, TFailure>.Error<TSuccess, TFailure>(var error)
                => Result<TNew, TFailure>.Failure(error),
            _ => throw new InvalidOperationException()
        };
    }

    public static async Task<Result<TNew, TFailure>> BindAsync<TSuccess, TNew, TFailure>(
        this Task<Result<TSuccess, TFailure>> resultTask,
        Func<TSuccess, Task<Result<TNew, TFailure>>> binder
    )
    {
        var result = await resultTask;
        return result switch
        {
            Result<TSuccess, TFailure>.Ok<TSuccess, TFailure>(var value)
                => await binder(value),
            Result<TSuccess, TFailure>.Error<TSuccess, TFailure>(var error)
                => Result<TNew, TFailure>.Failure(error),
            _ => throw new InvalidOperationException()
        };
    }
}
```

---

## Best Practices

1. **Use `Sequence` for batch operations** - Fail fast on first error
2. **Use `Combine` for validation** - Collect multiple validations
3. **Use `Filter` for conditional checks** - Add post-operation validation
4. **Never use `GetValueOrThrow` in production** - Only for tests
5. **Create async extensions** - Add project-specific async helpers
