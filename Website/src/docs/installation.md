---
layout: layouts/docs.njk
title: Installation
description: Install RestClient.Net and related packages for type-safe REST calls in C#. NuGet packages for the core library, OpenAPI generator, MCP server, and exhaustiveness analyzer.
keywords: RestClient.Net installation, NuGet package, dotnet add package, C# REST client setup
eleventyNavigation:
  key: Installation
  order: 2
faq:
  - question: How do I install RestClient.Net?
    answer: Run 'dotnet add package RestClient.Net' in your project directory to install the core library from NuGet.
  - question: What .NET versions does RestClient.Net support?
    answer: RestClient.Net requires .NET 8.0 or higher, taking advantage of the latest C# language features for pattern matching.
  - question: Do I need to install the Exhaustion analyzer separately?
    answer: The Exhaustion analyzer is included automatically when you install RestClient.Net. You can also install it separately with 'dotnet add package Exhaustion'.
---

<script type="application/ld+json">
{
  "@context": "https://schema.org",
  "@type": "HowTo",
  "name": "How to Install RestClient.Net",
  "description": "Step-by-step guide to installing RestClient.Net NuGet packages",
  "totalTime": "PT2M",
  "step": [
    {
      "@type": "HowToStep",
      "name": "Install core package",
      "text": "Run 'dotnet add package RestClient.Net' to install the main library"
    },
    {
      "@type": "HowToStep",
      "name": "Add global usings",
      "text": "Create GlobalUsings.cs with type aliases for cleaner pattern matching"
    }
  ]
}
</script>

# Installation

RestClient.Net is distributed as NuGet packages. This guide covers installing all available packages.

## Prerequisites

- **.NET 8.0** or higher
- A code editor (Visual Studio, VS Code, or Rider recommended)
- Basic familiarity with C# and `HttpClient`

## Core Package

The main package provides `HttpClient` extension methods with Result types:

```bash
dotnet add package RestClient.Net
```

This automatically includes:
- `HttpClient` extension methods (`GetAsync`, `PostAsync`, etc.)
- `Result<TSuccess, HttpError<TError>>` types
- `AbsoluteUrl` type and extensions
- **Exhaustion analyzer** for compile-time exhaustiveness checking

## Package Reference (csproj)

Alternatively, add directly to your `.csproj`:

```xml
<ItemGroup>
  <PackageReference Include="RestClient.Net" Version="6.*" />
</ItemGroup>
```

## Additional Packages

### OpenAPI Generator

Generate type-safe clients from OpenAPI 3.x specifications:

```bash
dotnet add package RestClient.Net.OpenApiGenerator
```

### MCP Server Generator

Generate Model Context Protocol servers for Claude Code integration:

```bash
dotnet add package RestClient.Net.McpGenerator
```

### Standalone Exhaustion Analyzer

If you only want the exhaustiveness analyzer without the REST client:

```bash
dotnet add package Exhaustion
```

## Quick Setup

After installing, create a `GlobalUsings.cs` file in your project root:

```csharp
// GlobalUsings.cs
global using System.Net.Http.Json;
global using System.Text.Json;
global using RestClient.Net;
global using Urls;
```

For each model type you use, add type aliases for cleaner pattern matching:

```csharp
// Example for a User model with ApiError
global using OkUser = Outcome.Result<User, Outcome.HttpError<ApiError>>
    .Ok<User, Outcome.HttpError<ApiError>>;

global using ErrorUser = Outcome.Result<User, Outcome.HttpError<ApiError>>
    .Error<User, Outcome.HttpError<ApiError>>;

global using ResponseErrorUser = Outcome.HttpError<ApiError>.ErrorResponseError;

global using ExceptionErrorUser = Outcome.HttpError<ApiError>.ExceptionError;
```

## Verify Installation

Create a simple test to verify everything is working:

```csharp
using System.Text.Json;
using RestClient.Net;
using Urls;

// Define models
record Post(int UserId, int Id, string Title, string Body);
record ErrorResponse(string Message);

// Create HttpClient
using var httpClient = new HttpClient();

// Make a test call
var result = await httpClient.GetAsync(
    url: "https://jsonplaceholder.typicode.com/posts/1".ToAbsoluteUrl(),
    deserializeSuccess: async (content, ct) =>
        await content.ReadFromJsonAsync<Post>(ct) ?? throw new Exception("Null response"),
    deserializeError: async (content, ct) =>
        await content.ReadFromJsonAsync<ErrorResponse>(ct) ?? new ErrorResponse("Unknown error")
);

Console.WriteLine(result switch
{
    Outcome.Result<Post, Outcome.HttpError<ErrorResponse>>.Ok(var post) =>
        $"Success: {post.Title}",
    Outcome.Result<Post, Outcome.HttpError<ErrorResponse>>.Error(var error) =>
        $"Error: {error}",
});
```

## IDE Support

### Visual Studio

The Exhaustion analyzer works out of the box. You'll see compile errors for non-exhaustive switch expressions.

### VS Code

Install the C# extension (ms-dotnettools.csharp) for full analyzer support.

### Rider

JetBrains Rider fully supports Roslyn analyzers, including Exhaustion.

## Next Steps

- [Basic Usage](/docs/basic-usage/) - Learn the fundamentals
- [Error Handling](/docs/error-handling/) - Master Result types
- [OpenAPI Generator](/docs/openapi/) - Generate clients from specs
