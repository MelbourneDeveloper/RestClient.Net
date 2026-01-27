---
layout: layouts/docs.njk
title: OpenAPI Generator API Reference
description: Code generation tools for creating type-safe C# clients from OpenAPI specifications
eleventyNavigation:
  key: OpenAPI Generator
  parent: API Reference
  order: 30
---

# OpenAPI Generator

Generates C# extension methods and model classes from OpenAPI specifications.

**Namespace:** `RestClient.Net.OpenApiGenerator`

## Overview

The OpenAPI Generator parses OpenAPI (Swagger) documents and generates strongly-typed C# code that integrates with RestClient.Net. It uses the Microsoft.OpenApi library for parsing.

## OpenApiCodeGenerator

The main entry point for code generation from OpenAPI specifications.

### Generate

Generates code from an OpenAPI document.

```csharp
public static Result<GeneratorResult, string> Generate(
    string openApiContent,
    string @namespace,
    string className,
    string outputPath,
    string? baseUrlOverride = null,
    string jsonNamingPolicy = "camelCase",
    bool caseInsensitive = false
)
```

**Parameters:**
| Parameter | Type | Description |
|-----------|------|-------------|
| `openApiContent` | `string` | The OpenAPI document content (JSON or YAML) |
| `namespace` | `string` | The namespace for generated code |
| `className` | `string` | The class name for extension methods |
| `outputPath` | `string` | The directory path where generated files will be saved |
| `baseUrlOverride` | `string?` | Optional base URL override. Use when the OpenAPI spec has a relative server URL |
| `jsonNamingPolicy` | `string` | JSON naming policy: "camelCase", "PascalCase", or "snake_case" |
| `caseInsensitive` | `bool` | Enable case-insensitive JSON deserialization |

**Returns:** `Result<GeneratorResult, string>` - A Result containing either the generated code or an error message.

**Example:**
```csharp
var openApiSpec = await File.ReadAllTextAsync("petstore.yaml");

var result = OpenApiCodeGenerator.Generate(
    openApiContent: openApiSpec,
    @namespace: "PetStore.Client",
    className: "PetStoreExtensions",
    outputPath: "./Generated",
    baseUrlOverride: "https://api.petstore.com",
    jsonNamingPolicy: "camelCase",
    caseInsensitive: true
);

result.Match(
    onSuccess: generated =>
    {
        Console.WriteLine("Extension methods generated!");
        Console.WriteLine(generated.ExtensionMethodsCode);
    },
    onError: error => Console.WriteLine($"Generation failed: {error}")
);
```

---

## GeneratorResult

Represents the result of code generation.

```csharp
public record GeneratorResult(
    string ExtensionMethodsCode,
    string ModelsCode
)
```

**Properties:**
| Property | Type | Description |
|----------|------|-------------|
| `ExtensionMethodsCode` | `string` | The generated extension methods code |
| `ModelsCode` | `string` | The generated models code |

---

## ExtensionMethodGenerator

Generates C# extension methods from OpenAPI operations.

### GenerateExtensionMethods

Generates extension methods from an OpenAPI document.

```csharp
public static (string ExtensionMethods, string TypeAliases) GenerateExtensionMethods(
    OpenApiDocument document,
    string @namespace,
    string className,
    string baseUrl,
    string basePath,
    string jsonNamingPolicy,
    bool caseInsensitive
)
```

**Parameters:**
| Parameter | Type | Description |
|-----------|------|-------------|
| `document` | `OpenApiDocument` | The OpenAPI document |
| `namespace` | `string` | The namespace for generated code |
| `className` | `string` | The class name for extension methods |
| `baseUrl` | `string` | The base URL for API requests (not used, kept for API compatibility) |
| `basePath` | `string` | The base path for API requests |
| `jsonNamingPolicy` | `string` | JSON naming policy: "camelCase", "PascalCase", or "snake_case" |
| `caseInsensitive` | `bool` | Enable case-insensitive JSON deserialization |

**Returns:** A tuple containing the extension methods code and type aliases code.

---

## ModelGenerator

Generates C# model classes from OpenAPI schemas.

### GenerateModels

Generates C# models from an OpenAPI document.

```csharp
public static string GenerateModels(OpenApiDocument document, string @namespace)
```

**Parameters:**
| Parameter | Type | Description |
|-----------|------|-------------|
| `document` | `OpenApiDocument` | The OpenAPI document |
| `namespace` | `string` | The namespace for the generated models |

**Returns:** `string` - The generated models code.

---

### GenerateModel

Generates a single C# model record from an OpenAPI schema.

```csharp
public static string GenerateModel(
    string name,
    OpenApiSchema schema,
    IDictionary<string, IOpenApiSchema>? schemas = null
)
```

**Parameters:**
| Parameter | Type | Description |
|-----------|------|-------------|
| `name` | `string` | The name of the model |
| `schema` | `OpenApiSchema` | The OpenAPI schema |
| `schemas` | `IDictionary<string, IOpenApiSchema>?` | Optional schemas dictionary to check for string enums |

**Returns:** `string` - The generated model code.

---

### MapOpenApiType

Maps an OpenAPI schema to a C# type.

```csharp
public static string MapOpenApiType(
    IOpenApiSchema schema,
    IDictionary<string, IOpenApiSchema>? schemas = null
)
```

**Parameters:**
| Parameter | Type | Description |
|-----------|------|-------------|
| `schema` | `IOpenApiSchema` | The OpenAPI schema |
| `schemas` | `IDictionary<string, IOpenApiSchema>?` | Optional schemas dictionary to check for string enums |

**Returns:** `string` - The C# type name.

---

### IsStringEnum

Checks if a schema is a string enum.

```csharp
public static bool IsStringEnum(OpenApiSchema schema)
```

**Parameters:**
| Parameter | Type | Description |
|-----------|------|-------------|
| `schema` | `OpenApiSchema` | The schema to check |

**Returns:** `bool` - True if the schema is a string enum.

---

### SanitizeDescription

Sanitizes a description for use in XML comments.

```csharp
public static string SanitizeDescription(string? description)
```

**Parameters:**
| Parameter | Type | Description |
|-----------|------|-------------|
| `description` | `string?` | The description to sanitize |

**Returns:** `string` - A single-line description safe for XML comments.

---

## ParameterInfo

Parameter information for OpenAPI operations.

```csharp
public record ParameterInfo(
    string Name,
    string Type,
    bool IsPath,
    bool IsHeader,
    string? OriginalName,
    bool Required,
    string? DefaultValue
)
```

**Properties:**
| Property | Type | Description |
|----------|------|-------------|
| `Name` | `string` | The parameter name (sanitized for C#) |
| `Type` | `string` | The parameter type |
| `IsPath` | `bool` | Whether the parameter is a path parameter |
| `IsHeader` | `bool` | Whether the parameter is a header parameter |
| `OriginalName` | `string?` | The original parameter name from the OpenAPI spec |
| `Required` | `bool` | Whether the parameter is required |
| `DefaultValue` | `string?` | The default value for the parameter |

---

## CodeGenerationHelpers

Helper methods for code generation.

### ToPascalCase

Converts a string to PascalCase.

```csharp
public static string ToPascalCase(string text)
```

**Parameters:**
| Parameter | Type | Description |
|-----------|------|-------------|
| `text` | `string` | The text to convert |

**Returns:** `string` - The PascalCase version of the text.

---

### ToCamelCase

Converts a string to camelCase.

```csharp
public static string ToCamelCase(string text)
```

**Parameters:**
| Parameter | Type | Description |
|-----------|------|-------------|
| `text` | `string` | The text to convert |

**Returns:** `string` - The camelCase version of the text.

---

### Indent

Indents text by the specified number of levels.

```csharp
public static string Indent(string text, int level)
```

**Parameters:**
| Parameter | Type | Description |
|-----------|------|-------------|
| `text` | `string` | The text to indent |
| `level` | `int` | The indentation level (1 level = 4 spaces) |

**Returns:** `string` - The indented text.

---

### BuildPathExpression

Builds a path expression from a path template.

```csharp
public static string BuildPathExpression(string path)
```

**Parameters:**
| Parameter | Type | Description |
|-----------|------|-------------|
| `path` | `string` | The path template |

**Returns:** `string` - The path expression.

---

### SanitizePathParameters

Replaces path parameter names with their sanitized C# equivalents.

```csharp
public static string SanitizePathParameters(string path, List<ParameterInfo> parameters)
```

**Parameters:**
| Parameter | Type | Description |
|-----------|------|-------------|
| `path` | `string` | The path template with original parameter names |
| `parameters` | `List<ParameterInfo>` | List of parameters with original and sanitized names |

**Returns:** `string` - The path with sanitized parameter names.

---

## UrlParser

Parses base URLs and paths from OpenAPI documents.

### GetBaseUrlAndPath

Gets the base URL and path from an OpenAPI document.

```csharp
public static Result<(string BaseUrl, string BasePath), string> GetBaseUrlAndPath(
    OpenApiDocument document,
    string? baseUrlOverride = null
)
```

**Parameters:**
| Parameter | Type | Description |
|-----------|------|-------------|
| `document` | `OpenApiDocument` | The OpenAPI document |
| `baseUrlOverride` | `string?` | Optional base URL override |

**Returns:** `Result<(string BaseUrl, string BasePath), string>` - A result containing the base URL and path, or an error message.

---

## CLI Usage

The OpenAPI Generator is also available as a command-line tool:

```bash
restclient-openapi generate \
  --input petstore.yaml \
  --output ./Generated \
  --namespace PetStore.Client \
  --class PetStoreExtensions \
  --base-url https://api.petstore.com \
  --naming camelCase \
  --case-insensitive
```

## See Also

- [MCP Generator](./mcp-generator) - Generate MCP servers from OpenAPI specs
- [Getting Started with OpenAPI](/docs/openapi) - Tutorial and examples
