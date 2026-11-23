---
layout: layouts/docs.njk
title: MCP Generator
lang: en
permalink: /api/mcp-generator/
eleventyNavigation:
  key: MCP Generator
  parent: API Reference
  order: 6
---

Generate [Model Context Protocol (MCP)](https://modelcontextprotocol.io/) servers from OpenAPI specifications for AI integration with Claude Code and other tools.

## Installation

```bash
dotnet add package RestClient.Net.McpGenerator
```

## Prerequisites

First, generate the REST client using the [OpenAPI Generator](/api/openapi-generator/):

```bash
dotnet run --project RestClient.Net.OpenApiGenerator.Cli -- \
  -u api.yaml -o Generated -n YourApi.Generated
```

## CLI Usage

```bash
dotnet run --project RestClient.Net.McpGenerator.Cli -- \
  --openapi-url api.yaml \
  --output-file Generated/McpTools.g.cs \
  --namespace YourApi.Mcp \
  --server-name YourApi \
  --ext-namespace YourApi.Generated \
  --tags "Search,Resources"
```

### CLI Options

| Option | Description |
|--------|-------------|
| `--openapi-url` | Path to [OpenAPI](https://swagger.io/specification/) specification |
| `--output-file` | Output file for generated MCP tools |
| `--namespace` | C# namespace for MCP server |
| `--server-name` | Name of the MCP server |
| `--ext-namespace` | Namespace of generated REST client |
| `--tags` | OpenAPI tags to include (comma-separated) |

## Generated Code

The generator creates MCP tool definitions that wrap the [HttpClient extensions](/api/httpclient-extensions/):

```csharp
[McpServerToolType]
public static partial class McpTools
{
    [McpServerTool(Name = "get_user")]
    [Description("Get user by ID")]
    public static async Task<string> GetUser(
        [Description("User ID")] string id,
        HttpClient httpClient,
        CancellationToken ct)
    {
        var result = await httpClient.GetUserById(id, ct);
        return result switch
        {
            OkUser(var user) => JsonSerializer.Serialize(user),
            ErrorUser(var error) => $"Error: {error}"
        };
    }
}
```

## Claude Code Integration

Add to your Claude Code configuration:

```json
{
  "mcpServers": {
    "yourapi": {
      "command": "dotnet",
      "args": ["run", "--project", "YourApi.McpServer"]
    }
  }
}
```

## Tool Naming

OpenAPI operations are converted to MCP tool names:

| OpenAPI | MCP Tool |
|---------|----------|
| `GET /users/{id}` | `get_user` |
| `POST /users` | `create_user` |
| `PUT /users/{id}` | `update_user` |
| `DELETE /users/{id}` | `delete_user` |

## See Also

- [OpenAPI Generator](/api/openapi-generator/) - Generate the REST client first
- [Result Types](/api/result-types/) - How results are handled
- [HttpClient Extensions](/api/httpclient-extensions/) - The underlying HTTP methods
