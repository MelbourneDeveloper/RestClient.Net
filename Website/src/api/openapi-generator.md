---
layout: layouts/docs.njk
title: OpenAPI Generator
lang: en
permalink: /api/openapi-generator/
eleventyNavigation:
  key: OpenAPI Generator
  parent: API Reference
  order: 5
---

Generate type-safe C# clients from [OpenAPI 3.x](https://swagger.io/specification/) specifications.

## Installation

```bash
dotnet add package RestClient.Net.OpenApiGenerator
```

## CLI Usage

```bash
dotnet run --project RestClient.Net.OpenApiGenerator.Cli -- \
  -u api.yaml \
  -o Generated \
  -n YourApi.Generated
```

### CLI Options

| Option | Short | Description |
|--------|-------|-------------|
| `--openapi-url` | `-u` | Path to OpenAPI spec (YAML or JSON) |
| `--output` | `-o` | Output directory for generated files |
| `--namespace` | `-n` | C# namespace for generated code |
| `--client-name` | `-c` | Prefix for generated client class names |

## Generated Code

The generator creates:

1. **Model classes** - [Records](https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/record) for all schemas
2. **[HttpClient](/api/httpclient-extensions/) extension methods** - For each endpoint
3. **[Result type aliases](/api/result-types/#type-aliases)** - For concise pattern matching

### Example Output

For an OpenAPI spec with a `/users/{id}` endpoint:

```csharp
// Generated extension method
public static async Task<ResultUser> GetUserById(
    this HttpClient httpClient,
    string id,
    CancellationToken ct = default)
{
    return await httpClient.GetAsync(
        url: $"https://api.example.com/users/{id}".ToAbsoluteUrl(),
        deserializeSuccess: async (r, c) => await r.Content.ReadFromJsonAsync<User>(c),
        deserializeError: async (r, c) => await r.Content.ReadFromJsonAsync<ErrorResponse>(c),
        ct
    );
}

// Generated type alias
global using ResultUser = Outcome.Result<User, Outcome.HttpError<ErrorResponse>>;
global using OkUser = ResultUser.Ok<User, Outcome.HttpError<ErrorResponse>>;
global using ErrorUser = ResultUser.Error<User, Outcome.HttpError<ErrorResponse>>;
```

### Usage

```csharp
using YourApi.Generated;

var httpClient = factory.CreateClient();

// Type-safe API call
var result = await httpClient.GetUserById("123");

// Pattern match on result
var output = result switch
{
    OkUser(var user) => $"Found: {user.Name}",
    ErrorUser(ResponseErrorUser(var err, var status, _)) => $"Error {status}",
    ErrorUser(ExceptionErrorUser(var ex)) => $"Exception: {ex.Message}",
};
```

## Supported OpenAPI Features

- **HTTP Methods:** GET, POST, PUT, DELETE, PATCH
- **Parameters:** path, query, header
- **Request Bodies:** JSON, form data
- **Responses:** All status codes, multiple content types
- **Schemas:** objects, arrays, enums, [oneOf](https://swagger.io/docs/specification/data-models/oneof-anyof-allof-not/), [allOf](https://swagger.io/docs/specification/data-models/oneof-anyof-allof-not/), [anyOf](https://swagger.io/docs/specification/data-models/oneof-anyof-allof-not/)
- **References:** [$ref](https://swagger.io/docs/specification/using-ref/) for local and remote schemas

## See Also

- [HttpClient Extensions](/api/httpclient-extensions/) - The extension methods generated
- [Result Types](/api/result-types/) - Understanding the return types
- [MCP Generator](/api/mcp-generator/) - Generate AI tools from OpenAPI
