---
layout: layouts/blog.njk
title: OpenAPI Generator Now Available
date: 2024-02-20
author: Christian Findlay
excerpt: Generate type-safe C# clients from OpenAPI 3.x specifications with automatic Result type aliases.
tags: posts
---

# OpenAPI Generator Now Available

We're excited to announce the release of RestClient.Net.OpenApiGenerator - a tool that generates type-safe C# clients from OpenAPI 3.x specifications.

## Why Generate Clients?

Writing HTTP client code by hand is tedious and error-prone. With OpenAPI specifications becoming the standard for API documentation, it makes sense to generate clients automatically.

## How It Works

```bash
# Install the generator
dotnet add package RestClient.Net.OpenApiGenerator

# Generate from your OpenAPI spec
dotnet run --project RestClient.Net.OpenApiGenerator.Cli -- \
  -u api.yaml \
  -o Generated \
  -n YourApi.Generated
```

## Generated Code Example

The generator creates extension methods for `HttpClient` with proper Result types:

```csharp
using YourApi.Generated;

var httpClient = factory.CreateClient();

// Type-safe, exhaustive error handling
var user = await httpClient.GetUserById("123", ct);

switch (user)
{
    case OkUser(var success):
        Console.WriteLine($"Found: {success.Name}");
        break;
    case ErrorUser(ResponseErrorUser(var err, var status, _)):
        Console.WriteLine($"API Error: {status}");
        break;
    case ErrorUser(ExceptionErrorUser(var ex)):
        Console.WriteLine($"Exception: {ex.Message}");
        break;
}
```

## Learn More

Read the full [OpenAPI Generator documentation](/docs/openapi/) to get started.
