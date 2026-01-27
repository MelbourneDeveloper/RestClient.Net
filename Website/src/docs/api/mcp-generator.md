---
layout: layouts/docs.njk
title: MCP Generator API Reference
description: Generate Model Context Protocol (MCP) servers from OpenAPI specifications
eleventyNavigation:
  key: MCP Generator
  parent: API Reference
  order: 31
---

# MCP Generator

Generates Model Context Protocol (MCP) server code from OpenAPI specifications.

**Namespace:** `RestClient.Net.McpGenerator`

## Overview

The MCP Generator creates MCP server tools that wrap RestClient.Net extension methods generated from OpenAPI specifications. This enables AI assistants like Claude to interact with your APIs using type-safe, auto-generated tools.

## McpServerGenerator

The main entry point for generating MCP server code.

### Generate

Generates MCP server tools code from an OpenAPI document.

```csharp
public static Result<string, string> Generate(
    string openApiContent,
    string @namespace,
    string serverName,
    string extensionsNamespace,
    ISet<string>? includeTags = null
)
```

**Parameters:**
| Parameter | Type | Description |
|-----------|------|-------------|
| `openApiContent` | `string` | The OpenAPI document content (JSON or YAML) |
| `namespace` | `string` | The namespace for generated MCP tools |
| `serverName` | `string` | The MCP server name |
| `extensionsNamespace` | `string` | The namespace of the pre-generated RestClient.Net extensions |
| `includeTags` | `ISet<string>?` | Optional set of tags to include. If specified, only operations with these tags are generated |

**Returns:** `Result<string, string>` - A Result containing the generated C# code or an error message.

**Example:**
```csharp
var openApiSpec = await File.ReadAllTextAsync("petstore.yaml");

var result = McpServerGenerator.Generate(
    openApiContent: openApiSpec,
    @namespace: "PetStore.McpServer",
    serverName: "PetStoreServer",
    extensionsNamespace: "PetStore.Client",
    includeTags: new HashSet<string> { "pets", "store" }
);

result.Match(
    onSuccess: code =>
    {
        Console.WriteLine("MCP server generated!");
        await File.WriteAllTextAsync("McpServer.cs", code);
    },
    onError: error => Console.WriteLine($"Generation failed: {error}")
);
```

---

## McpToolGenerator

Generates MCP tool classes that use RestClient.Net extensions.

### GenerateTools

Generates MCP tools that wrap generated extension methods.

```csharp
public static string GenerateTools(
    OpenApiDocument document,
    string @namespace,
    string serverName,
    string extensionsNamespace,
    ISet<string>? includeTags = null
)
```

**Parameters:**
| Parameter | Type | Description |
|-----------|------|-------------|
| `document` | `OpenApiDocument` | The OpenAPI document |
| `namespace` | `string` | The namespace for the MCP server |
| `serverName` | `string` | The MCP server name |
| `extensionsNamespace` | `string` | The namespace of the extensions |
| `includeTags` | `ISet<string>?` | Optional set of tags to filter operations. If specified, only operations with these tags are generated |

**Returns:** `string` - The generated MCP tools code.

**Example:**
```csharp
using Microsoft.OpenApi.Reader;

var document = OpenApiDocument.Load("petstore.yaml").OpenApiDocument;

var toolsCode = McpToolGenerator.GenerateTools(
    document: document,
    @namespace: "PetStore.McpServer",
    serverName: "PetStoreServer",
    extensionsNamespace: "PetStore.Client",
    includeTags: null  // Generate all tools
);

Console.WriteLine(toolsCode);
```

---

## Generated Code Structure

The MCP Generator produces code with the following structure:

### Tool Classes

For each OpenAPI operation, a tool class is generated:

```csharp
[McpServerToolType]
public static class GetPetByIdTool
{
    [McpServerTool(Name = "getPetById")]
    [Description("Returns a single pet")]
    public static async Task<string> Execute(
        HttpClient httpClient,
        [Description("ID of pet to return")] long petId,
        CancellationToken cancellationToken = default)
    {
        var result = await httpClient.GetPetByIdAsync(petId, cancellationToken);
        return result.Match(
            onSuccess: pet => JsonSerializer.Serialize(pet),
            onError: error => $"Error: {error}"
        );
    }
}
```

### Server Registration

The generated code includes server registration:

```csharp
public static class McpServerRegistration
{
    public static void RegisterTools(IMcpServer server)
    {
        server.AddTool<GetPetByIdTool>();
        server.AddTool<CreatePetTool>();
        server.AddTool<UpdatePetTool>();
        // ... more tools
    }
}
```

---

## Tag Filtering

Use the `includeTags` parameter to generate tools for specific API sections:

```csharp
// Only generate tools for "pets" and "store" tagged operations
var result = McpServerGenerator.Generate(
    openApiContent: openApiSpec,
    @namespace: "PetStore.McpServer",
    serverName: "PetStoreServer",
    extensionsNamespace: "PetStore.Client",
    includeTags: new HashSet<string> { "pets", "store" }
);
```

This is useful for:
- Creating focused MCP servers for specific API domains
- Reducing the number of tools exposed to AI assistants
- Generating separate MCP servers for different user roles

---

## Prerequisites

Before using the MCP Generator, you must first generate the RestClient.Net extensions:

```bash
# Step 1: Generate RestClient.Net extensions
restclient-openapi generate \
  --input petstore.yaml \
  --output ./Generated \
  --namespace PetStore.Client

# Step 2: Generate MCP server
restclient-mcp generate \
  --input petstore.yaml \
  --output ./McpServer \
  --namespace PetStore.McpServer \
  --server-name PetStoreServer \
  --extensions-namespace PetStore.Client
```

---

## CLI Usage

The MCP Generator is available as a command-line tool:

```bash
restclient-mcp generate \
  --input petstore.yaml \
  --output ./McpServer \
  --namespace PetStore.McpServer \
  --server-name PetStoreServer \
  --extensions-namespace PetStore.Client \
  --tags pets,store
```

### CLI Options

| Option | Description |
|--------|-------------|
| `--input` | Path to the OpenAPI specification file |
| `--output` | Output directory for generated code |
| `--namespace` | Namespace for the generated MCP server |
| `--server-name` | Name of the MCP server |
| `--extensions-namespace` | Namespace of the pre-generated extensions |
| `--tags` | Comma-separated list of tags to include (optional) |

---

## Integration with Claude

The generated MCP server can be used with Claude Desktop or other MCP-compatible clients:

### claude_desktop_config.json

```json
{
  "mcpServers": {
    "petstore": {
      "command": "dotnet",
      "args": ["run", "--project", "/path/to/PetStore.McpServer"],
      "env": {
        "PETSTORE_API_KEY": "your-api-key"
      }
    }
  }
}
```

### Using with Claude

Once configured, Claude can use the generated tools:

```
Claude: I'll look up the pet with ID 123 for you.

[Using tool: getPetById with petId=123]

The pet with ID 123 is a golden retriever named "Buddy" who is available for adoption.
```

---

## Best Practices

1. **Generate extensions first** - Always generate RestClient.Net extensions before MCP tools
2. **Use tag filtering** - Generate focused servers for specific API domains
3. **Document your API** - Good OpenAPI descriptions become good tool descriptions
4. **Handle authentication** - Configure HttpClient with appropriate auth headers
5. **Error handling** - The generated tools return error messages as strings for AI consumption

## See Also

- [OpenAPI Generator](./openapi-generator) - Generate RestClient.Net extensions
- [Getting Started with MCP](/docs/mcp) - Tutorial and examples
