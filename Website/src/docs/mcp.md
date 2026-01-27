---
layout: layouts/docs.njk
title: MCP Server
description: Generate Model Context Protocol servers for Claude Code integration. Turn your REST APIs into AI-accessible tools with automatic type-safe bindings.
keywords: MCP server, Model Context Protocol, Claude Code, AI tools, REST to MCP
eleventyNavigation:
  key: MCP Server
  order: 7
faq:
  - question: What is the Model Context Protocol (MCP)?
    answer: MCP is a protocol that allows AI assistants like Claude to interact with external tools and data sources. RestClient.Net can generate MCP servers from your OpenAPI specs.
  - question: How does the MCP generator work?
    answer: It reads your OpenAPI specification and generates C# code that exposes each API operation as an MCP tool that Claude Code can call.
  - question: Can I filter which operations become MCP tools?
    answer: Yes, use the --tags option to select specific operation tags, or --exclude-tags to skip certain operations.
---

# MCP Server Generation

Generate Model Context Protocol (MCP) servers from OpenAPI specifications, enabling Claude Code to interact with your REST APIs.

## What is MCP?

The Model Context Protocol allows AI assistants to:
- Call external APIs as "tools"
- Access data sources
- Perform actions on behalf of users

With RestClient.Net's MCP generator, your REST APIs become Claude Code tools automatically.

## Prerequisites

1. An OpenAPI 3.x specification
2. Generated RestClient.Net client (see [OpenAPI Generator](/docs/openapi/))
3. RestClient.Net.McpGenerator.Cli tool

## Installation

Install the MCP generator CLI:

```bash
dotnet tool install -g RestClient.Net.McpGenerator.Cli
```

## Quick Start

### Step 1: Generate the REST Client

First, generate the RestClient.Net client from your OpenAPI spec:

```bash
restclient-gen \
  --input api.yaml \
  --output Generated \
  --namespace MyApi.Client
```

### Step 2: Generate MCP Server

Generate the MCP server code:

```bash
restclient-mcp \
  --openapi api.yaml \
  --output McpServer \
  --namespace MyApi.Mcp \
  --server-name MyApiServer \
  --client-namespace MyApi.Client
```

### Step 3: Create MCP Server Project

Create a new console project:

```bash
dotnet new console -n MyApi.McpServer
cd MyApi.McpServer
dotnet add package Microsoft.Extensions.Hosting
dotnet add package RestClient.Net
```

Add the generated files and create the host:

```csharp
// Program.cs
using Microsoft.Extensions.Hosting;
using MyApi.Mcp;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddHttpClient("api", client =>
        {
            client.BaseAddress = new Uri("https://api.example.com");
        });

        services.AddMcpServer<MyApiServer>();
    })
    .Build();

await host.RunAsync();
```

### Step 4: Configure Claude Code

Add to your Claude Code configuration (`.claude/mcp.json`):

```json
{
  "mcpServers": {
    "myapi": {
      "command": "dotnet",
      "args": ["run", "--project", "path/to/MyApi.McpServer"]
    }
  }
}
```

## CLI Options

| Option | Description |
|--------|-------------|
| `--openapi` | OpenAPI spec URL or file path (required) |
| `--output` | Output directory (default: `McpServer`) |
| `--namespace` | C# namespace for generated code (required) |
| `--server-name` | MCP server class name (required) |
| `--client-namespace` | Namespace of the generated REST client |
| `--tags` | Include only these operation tags (comma-separated) |
| `--exclude-tags` | Exclude these operation tags (comma-separated) |

### Examples

Include only specific operations:

```bash
restclient-mcp \
  --openapi api.yaml \
  --output McpServer \
  --namespace MyApi.Mcp \
  --server-name MyApiServer \
  --client-namespace MyApi.Client \
  --tags "Users,Search"
```

Exclude admin operations:

```bash
restclient-mcp \
  --openapi api.yaml \
  --output McpServer \
  --namespace MyApi.Mcp \
  --server-name MyApiServer \
  --client-namespace MyApi.Client \
  --exclude-tags "Admin,Internal"
```

## Generated Code Structure

```
McpServer/
├── Tools/
│   ├── GetUserTool.cs
│   ├── CreateUserTool.cs
│   └── ...
├── MyApiServer.cs
└── ServiceCollectionExtensions.cs
```

### Generated Tool Example

```csharp
// McpServer/Tools/GetUserTool.cs
namespace MyApi.Mcp.Tools;

public class GetUserTool(IHttpClientFactory httpClientFactory) : IMcpTool
{
    public string Name => "get_user";
    public string Description => "Retrieves a user by their ID";

    public ToolParameters Parameters => new()
    {
        Properties = new Dictionary<string, ToolParameter>
        {
            ["userId"] = new()
            {
                Type = "string",
                Description = "The unique identifier of the user",
                Required = true
            }
        }
    };

    public async Task<ToolResult> ExecuteAsync(
        JsonElement arguments,
        CancellationToken ct)
    {
        var userId = arguments.GetProperty("userId").GetString()!;
        var client = httpClientFactory.CreateClient("api");

        var result = await client.GetUserByIdAsync(userId, ct);

        return result switch
        {
            OkUser(var user) => ToolResult.Success(JsonSerializer.Serialize(user)),
            ErrorUser(var error) => ToolResult.Error($"Failed: {error}"),
        };
    }
}
```

## Authentication

For authenticated APIs, configure the HttpClient:

```csharp
services.AddHttpClient("api", client =>
{
    client.BaseAddress = new Uri("https://api.example.com");
})
.AddHttpMessageHandler<AuthenticationHandler>();

services.AddTransient<AuthenticationHandler>();
```

### Environment Variables

Use environment variables for secrets:

```csharp
public class AuthenticationHandler : DelegatingHandler
{
    protected override Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        var token = Environment.GetEnvironmentVariable("API_TOKEN");
        request.Headers.Authorization =
            new AuthenticationHeaderValue("Bearer", token);

        return base.SendAsync(request, cancellationToken);
    }
}
```

Configure in Claude Code:

```json
{
  "mcpServers": {
    "myapi": {
      "command": "dotnet",
      "args": ["run", "--project", "MyApi.McpServer"],
      "env": {
        "API_TOKEN": "your-api-token"
      }
    }
  }
}
```

## Tool Naming

The generator creates tool names from operation IDs:

| Operation ID | Tool Name |
|--------------|-----------|
| `getUserById` | `get_user_by_id` |
| `createUser` | `create_user` |
| `deleteUserById` | `delete_user_by_id` |

## Best Practices

1. **Filter operations** - Only expose necessary tools to Claude
2. **Use descriptive operation summaries** - These become tool descriptions
3. **Secure sensitive operations** - Use authentication and authorization
4. **Handle errors gracefully** - Return meaningful error messages
5. **Test tools manually** - Verify behavior before Claude uses them

## Debugging

Run the MCP server locally to test:

```bash
cd MyApi.McpServer
dotnet run
```

Use the MCP inspector to test tools:

```bash
npx @anthropic-ai/mcp-inspector
```

## Next Steps

- [Exhaustion Analyzer](/docs/exhaustion/) - Compile-time exhaustiveness
- [Advanced Usage](/docs/advanced-usage/) - Retry policies and middleware
- [API Reference](/api/) - Complete documentation
```
