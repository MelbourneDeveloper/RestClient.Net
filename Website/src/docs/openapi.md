---
layout: layouts/docs.njk
title: OpenAPI Generator
description: Generate type-safe C# REST clients from OpenAPI 3.x specifications. Automatic Result types, type aliases, and exhaustiveness checking.
keywords: OpenAPI generator, C# client generator, OpenAPI to C#, REST client generator, code generation
eleventyNavigation:
  key: OpenAPI Generator
  order: 6
faq:
  - question: What OpenAPI versions are supported?
    answer: RestClient.Net.OpenApiGenerator supports OpenAPI 3.0 and 3.1 specifications in both YAML and JSON formats.
  - question: Does the generator create type aliases automatically?
    answer: Yes, the generator creates GlobalUsings.cs with all necessary type aliases for clean pattern matching.
  - question: Can I customize the generated code?
    answer: Yes, you can specify namespace, output directory, and filter operations by tags. The generated code follows RestClient.Net patterns.
---

# OpenAPI Client Generation

Generate type-safe C# clients from OpenAPI 3.x specifications with automatic Result types and exhaustiveness checking.

## Installation

Install the generator CLI tool:

```bash
dotnet tool install -g RestClient.Net.OpenApiGenerator.Cli
```

Or add to your project:

```bash
dotnet add package RestClient.Net.OpenApiGenerator
```

## Quick Start

Generate a client from an OpenAPI spec:

```bash
restclient-gen \
  --input https://petstore3.swagger.io/api/v3/openapi.json \
  --output Generated \
  --namespace PetStore.Client
```

Or from a local file:

```bash
restclient-gen \
  --input api.yaml \
  --output Generated \
  --namespace MyApi.Client
```

## CLI Options

| Option | Short | Description |
|--------|-------|-------------|
| `--input` | `-i` | OpenAPI spec URL or file path (required) |
| `--output` | `-o` | Output directory (default: `Generated`) |
| `--namespace` | `-n` | C# namespace for generated code (required) |
| `--tags` | `-t` | Filter operations by tags (comma-separated) |
| `--skip-validation` | | Skip OpenAPI spec validation |

### Examples

Generate only specific tags:

```bash
restclient-gen \
  -i api.yaml \
  -o Generated \
  -n MyApi.Client \
  --tags "Users,Orders"
```

## Generated Files

The generator creates:

```
Generated/
├── Models/
│   ├── User.cs
│   ├── Order.cs
│   └── ...
├── HttpClientExtensions.g.cs
├── Deserializers.g.cs
└── GlobalUsings.g.cs
```

### Models

Data transfer objects from OpenAPI schemas:

```csharp
// Generated/Models/User.cs
namespace MyApi.Client.Models;

public sealed record User(
    string Id,
    string Name,
    string Email,
    DateTime CreatedAt
);
```

### HttpClient Extensions

Extension methods for each operation:

```csharp
// Generated/HttpClientExtensions.g.cs
namespace MyApi.Client;

public static class HttpClientExtensions
{
    public static Task<Result<User, HttpError<ApiError>>> GetUserByIdAsync(
        this HttpClient httpClient,
        string userId,
        CancellationToken ct = default) =>
        httpClient.GetAsync(
            url: $"/users/{userId}".ToAbsoluteUrl(),
            deserializeSuccess: Deserializers.User,
            deserializeError: Deserializers.ApiError,
            cancellationToken: ct
        );

    public static Task<Result<User, HttpError<ApiError>>> CreateUserAsync(
        this HttpClient httpClient,
        CreateUserRequest request,
        CancellationToken ct = default) =>
        httpClient.PostAsync(
            url: "/users".ToAbsoluteUrl(),
            body: request,
            serializeRequest: Serializers.Json,
            deserializeSuccess: Deserializers.User,
            deserializeError: Deserializers.ApiError,
            cancellationToken: ct
        );
}
```

### Type Aliases

Automatically generated for clean pattern matching:

```csharp
// Generated/GlobalUsings.g.cs
global using OkUser = Outcome.Result<User, Outcome.HttpError<ApiError>>
    .Ok<User, Outcome.HttpError<ApiError>>;

global using ErrorUser = Outcome.Result<User, Outcome.HttpError<ApiError>>
    .Error<User, Outcome.HttpError<ApiError>>;

global using ResponseErrorUser = Outcome.HttpError<ApiError>.ErrorResponseError;
global using ExceptionErrorUser = Outcome.HttpError<ApiError>.ExceptionError;
```

## Using Generated Code

```csharp
using MyApi.Client;
using MyApi.Client.Models;

// Create HttpClient (use IHttpClientFactory in production)
using var httpClient = new HttpClient
{
    BaseAddress = new Uri("https://api.example.com")
};

// Use generated extension methods
var result = await httpClient.GetUserByIdAsync("123");

// Pattern match with generated type aliases
var message = result switch
{
    OkUser(var user) => $"Found: {user.Name} ({user.Email})",
    ErrorUser(ResponseErrorUser(var err, var status, _)) => $"API Error {status}: {err.Message}",
    ErrorUser(ExceptionErrorUser(var ex)) => $"Exception: {ex.Message}",
};

Console.WriteLine(message);
```

## OpenAPI Spec Requirements

### Supported Features

- Path parameters
- Query parameters
- Request bodies (JSON)
- Response bodies (JSON)
- Schema references (`$ref`)
- Enums
- Arrays and nested objects
- Required/optional properties

### Example OpenAPI Spec

```yaml
openapi: 3.0.3
info:
  title: My API
  version: 1.0.0

paths:
  /users/{id}:
    get:
      operationId: getUserById
      tags: [Users]
      parameters:
        - name: id
          in: path
          required: true
          schema:
            type: string
      responses:
        '200':
          description: User found
          content:
            application/json:
              schema:
                $ref: '#/components/schemas/User'
        '404':
          description: User not found
          content:
            application/json:
              schema:
                $ref: '#/components/schemas/ApiError'

components:
  schemas:
    User:
      type: object
      required: [id, name, email]
      properties:
        id:
          type: string
        name:
          type: string
        email:
          type: string
          format: email
        createdAt:
          type: string
          format: date-time

    ApiError:
      type: object
      required: [message]
      properties:
        message:
          type: string
        code:
          type: string
```

## Build Integration

### MSBuild Target

Add to your `.csproj` to regenerate on build:

```xml
<Target Name="GenerateApiClient" BeforeTargets="BeforeCompile">
  <Exec Command="restclient-gen -i api.yaml -o Generated -n MyApi.Client" />
</Target>
```

### Pre-build Event

Or use a pre-build event:

```xml
<PropertyGroup>
  <PreBuildEvent>
    restclient-gen -i $(ProjectDir)api.yaml -o $(ProjectDir)Generated -n MyApi.Client
  </PreBuildEvent>
</PropertyGroup>
```

## Best Practices

1. **Version your OpenAPI specs** alongside your code
2. **Regenerate clients** when specs change
3. **Use tags** to organize and filter operations
4. **Review generated code** before committing
5. **Add generated files to .gitignore** or commit them based on your workflow

## Next Steps

- [MCP Server Generation](/docs/mcp/) - Generate Claude Code tools
- [Error Handling](/docs/error-handling/) - Work with Result types
- [API Reference](/api/) - Complete documentation
