# RestClient.Net

![diagram](https://github.com/MelbourneDeveloper/Restclient.Net/blob/main/Images/Logo.jpg) 

**The safest way to make REST calls in C#**

Built from the ground up with functional programming, type safety, and modern .NET patterns. Successor to the [original RestClient.Net](https://github.com/MelbourneDeveloper/RestClient.Net).

## What Makes It Different

This library is uncompromising in its approach to type safety and functional design:
- **HttpClient extensions** - Works with [HttpClient lifecycle management](https://learn.microsoft.com/en-us/dotnet/fundamentals/networking/http/httpclient-guidelines#recommended-use) via `IHttpClientFactory.CreateClient()`
- **Result types** - Explicit error handling without exceptions
- **Exhaustiveness checking** - Compile-time guarantees via [Exhaustion](https://github.com/MelbourneDeveloper/Exhaustion)
- **Functional composition** - Delegate factories, pure functions, no OOP ceremony

## Features

- **Result Types** - Returns `Result<TSuccess, HttpError<TError>>` with closed hierarchy types for compile-time safety (Outcome package)
- **Zero Exceptions** - No exception throwing for predictable error handling
- **Progress Reporting** - Built-in download/upload progress tracking
- **Polly Integration** - Support for convention-based [retry policies and resilience patterns](https://learn.microsoft.com/en-us/dotnet/fundamentals/networking/http/httpclient-guidelines#resilience-with-static-clients)
- **Async/Await Only** - Modern async patterns throughout
- **HttpClient Extensions** - Works with `IHttpClientFactory.CreateClient()` for proper [pooled connections](https://learn.microsoft.com/en-us/dotnet/fundamentals/networking/http/httpclient-guidelines#pooled-connections) and [DNS behavior](https://learn.microsoft.com/en-us/dotnet/fundamentals/networking/http/httpclient-guidelines#dns-behavior) handling
- **Exhaustiveness Checking** - Uses [Exhaustion](https://github.com/MelbourneDeveloper/Exhaustion) for compile-time completeness guarantees (stopgap until C# adds discriminated unions)

The design focuses on [discriminated unions](https://github.com/dotnet/csharplang/issues/8928) for results, and adding the [Exhaustion analyzer](https://www.nuget.org/packages/Exhaustion) package gives you exhaustive pattern matching on type in C#. RestClient.Net is well ahead of the game. You can use discriminated unions with exhaustiveness checks in C# right now.

## Installation

```bash
dotnet add package RestClient.Net
```

## Usage

### Basic GET Request

The simplest way to make a GET request to JSONPlaceholder:

```csharp
using System.Net.Http.Json;
using RestClient.Net;

// Define a simple User model
public record User(int Id, string Name, string Email);

// Get HttpClient from IHttpClientFactory
var httpClient = httpClientFactory.CreateClient();

// Make a direct GET request
var result = await httpClient.GetAsync<User, string>(
    url: "https://jsonplaceholder.typicode.com/users/1".ToAbsoluteUrl(),
    deserializeSuccess: (response, ct) => response.Content.ReadFromJsonAsync<User>(ct),
    deserializeError: (response, ct) => response.Content.ReadAsStringAsync(ct)
);

switch (result)
{
    case OkUser(var user):
        Console.WriteLine($"Success: {user.Name}");
        break;
    case ErrorUser(var error):
        Console.WriteLine($"Failed: {error.StatusCode} - {error.Body}");
        break;
}
```

### Result Type and Type Aliases

C# doesn't officially support discriminated unions, but you can achieve closed type hierarchies with the [sealed modifer](https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/sealed). The [Outcome package](https://www.nuget.org/packages/Outcome/) gives you a set of Result types designed for exhaustiveness checks. Until C# gains full discriminated union support, you need to add type aliases like this. If you use the generator, it will generate the type aliases for you.

```cs
// Type aliases for concise pattern matching
using OkUser = Result<User, HttpError<string>>.Ok<User, HttpError<string>>;
using ErrorUser = Result<User, HttpError<string>>.Error<User, HttpError<string>>;
```

### OpenAPI Code Generation

Generate type-safe extension methods from OpenAPI specs:

```csharp
using JSONPlaceholder.Generated;

// Get HttpClient from factory
var httpClient = factory.CreateClient();

// GET all todos
var todos = await httpClient.GetTodos(ct);

// GET todo by ID
var todo = await httpClient.GetTodoById(1, ct);
switch (todo)
{
    case OkTodo(var success):
        Console.WriteLine($"Todo: {success.Title}");
        break;
    case ErrorTodo(var error):
        Console.WriteLine($"Error: {error.StatusCode} - {error.Body}");
        break;
}

// POST - create a new todo
var newTodo = new TodoInput { Title = "New Task", UserId = 1, Completed = false };
var created = await httpClient.CreateTodo(newTodo, ct);

// PUT - update with path param and body
var updated = await httpClient.UpdateTodo((Params: 1, Body: newTodo), ct);

// DELETE - returns Unit
var deleted = await httpClient.DeleteTodo(1, ct);
```

```bash
dotnet add package RestClient.Net.OpenApiGenerator
```

Define your schema (OpenAPI 3.x):
```yaml
openapi: 3.0.0
paths:
  /users/{id}:
    get:
      operationId: getUserById
      parameters:
        - name: id
          in: path
          required: true
          schema:
            type: string
      responses:
        '200':
          content:
            application/json:
              schema:
                $ref: '#/components/schemas/User'
  /users:
    post:
      operationId: createUser
      requestBody:
        content:
          application/json:
            schema:
              $ref: '#/components/schemas/User'
      responses:
        '201':
          content:
            application/json:
              schema:
                $ref: '#/components/schemas/User'
```

The generator creates:
1. **Extension methods** - Strongly-typed methods on `HttpClient`
2. **Model classes** - DTOs from schema definitions
3. **Result type aliases** - Convenient `OkUser` and `ErrorUser` types

Generated usage:
```csharp
// Get HttpClient from factory
var httpClient = factory.CreateClient();

// GET with path parameter
var user = await httpClient.GetUserById("123", ct);

// POST with body
var created = await httpClient.CreateUser(newUser, ct);

// PUT with path param and body
var updated = await httpClient.UpdateUser((Params: "123", Body: user), ct);

// DELETE returns Unit
var deleted = await httpClient.DeleteUser("123", ct);
```

All generated methods:
- Create extension methods on `HttpClient` (use with `IHttpClientFactory.CreateClient()`)
- Return `Result<TSuccess, HttpError<TError>>` for functional error handling
- Bundle URL/body/headers into `HttpRequestParts` via `buildRequest`
- Support progress reporting through `ProgressReportingHttpContent`

### Progress Reporting

You can track upload progress with `ProgressReportingHttpContent`. This example writes to the console when there is a progress report.

```csharp
var fileBytes = await File.ReadAllBytesAsync("document.pdf");

using var content = new ProgressReportingHttpContent(
    fileBytes,
    progress: (current, total) =>
        Console.WriteLine($"Progress: {current}/{total} bytes ({current * 100 / total}%)")
);

var httpClient = httpClientFactory.CreateClient();
var result = await httpClient.PostAsync<UploadResponse, string>(
    url: "https://api.example.com/upload".ToAbsoluteUrl(),
    requestBody: content,
    deserializeSuccess: (r, ct) => r.Content.ReadFromJsonAsync<UploadResponse>(ct),
    deserializeError: (r, ct) => r.Content.ReadAsStringAsync(ct)
);
```

## Upgrading from RestClient.Net 6.x

You can continue to use the V6 `IClient` interface with RestClient .Net 7. RestClient.Net 7 is a complete rewrite with a functional architecture. For existing v6 users, **RestClient.Net.Original** provides a polyfill that implements the v6 `IClient` interface using v7 under the hood.

### Using the Polyfill

Install both packages:
```bash
dotnet add package RestClient.Net.Original
dotnet add package RestClient.Net.Abstractions --version 6.0.0
```

Use `Client` to maintain v6 compatibility while benefiting from v7's improvements:
```csharp
using RestClient.Net;

var client = new Client(
    httpClientFactory,
    baseUrl: "https://api.example.com".ToAbsoluteUrl(),
    defaultRequestHeaders: new HeadersCollection(),
    throwExceptionOnFailure: true
);

// Continue using your existing v6 IClient extension methods
var user = await client.GetAsync<User>("users/123");
var created = await client.PostAsync<User, CreateUserRequest>(newUser, "users");
var updated = await client.PutAsync<User, UpdateUserRequest>(updateUser, "users/123");
var deleted = await client.DeleteAsync("users/123");
```

This approach allows gradual migration. You can keep your existing implementations working while incrementally adopting v7's functional patterns.
