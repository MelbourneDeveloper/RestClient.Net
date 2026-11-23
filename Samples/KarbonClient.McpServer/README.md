# Karbon MCP Server

This is a Model Context Protocol (MCP) server for the Karbon API, built using RestClient.Net and the automatic code generation tools.

## What is this?

This MCP server provides Claude Code and other MCP clients with access to Karbon's practice management API. It enables AI assistants to:

- Manage contacts and client groups
- Create and update work items
- Handle tasks and assignments
- Search and organize client information

## Prerequisites

1. .NET 9.0 SDK
2. Karbon API credentials:
   - Authorization token (Bearer token)
   - Access key

To obtain API credentials:
1. [Register for API Access](https://developers.karbonhq.com/register) to receive your Authorization token
2. Find your [API Access Key](https://help.karbonhq.com/en/articles/4580888-karbon-api-integration) in Karbon under Settings -> Connected Apps

## Project Structure

- **KarbonClient/** - SDK generated from Karbon OpenAPI specification
  - `Generated/KarbonApiExtensions.g.cs` - Type-safe API client methods
  - `Generated/KarbonApiModels.g.cs` - Data transfer objects
  - `Generated/KarbonMcpTools.g.cs` - MCP tool definitions
- **KarbonClient.McpServer/** - MCP server executable
  - `Program.cs` - Server configuration and startup
  - `claude-mcp-config.example.json` - Example Claude Code configuration

## Configuration

### Claude Code Setup

1. Copy `claude-mcp-config.example.json` to your Claude Code configuration location
2. Update the file with your credentials:

```json
{
  "mcpServers": {
    "karbon": {
      "command": "/usr/local/share/dotnet/dotnet",
      "args": [
        "run",
        "--project",
        "/path/to/RestClient.Net/Samples/KarbonClient.McpServer/KarbonClient.McpServer.csproj"
      ],
      "env": {
        "KARBON_BASE_URL": "https://api.karbonhq.com",
        "KARBON_AUTHORIZATION_TOKEN": "your-bearer-token-here",
        "KARBON_ACCESS_KEY": "your-access-key-here"
      }
    }
  }
}
```

3. Replace:
   - `/path/to/RestClient.Net` with the actual path to this repository
   - `your-bearer-token-here` with your Karbon authorization token
   - `your-access-key-here` with your Karbon access key

### Environment Variables

The server uses the following environment variables:

- `KARBON_BASE_URL` (optional) - API base URL, defaults to `https://api.karbonhq.com`
- `KARBON_AUTHORIZATION_TOKEN` (required) - Bearer token for authentication
- `KARBON_ACCESS_KEY` (required) - Karbon tenant access key

## Building and Running

### Build the project

```bash
dotnet build
```

### Run the MCP server

```bash
cd Samples/KarbonClient.McpServer
dotnet run
```

The server communicates via stdio and is designed to be launched by MCP clients like Claude Code.

## Available Tools

The MCP server exposes tools for the following Karbon API operations (filtered by tags):

- **Contacts** - Manage individual and organizational contacts
- **Client Groups** - Create and manage client groups
- **Work** - Handle work items and workflows
- **Tasks** - Create, update, and manage tasks

All tools are automatically generated from the Karbon OpenAPI specification and provide type-safe access to the API.

## Code Generation

The client SDK and MCP tools are automatically generated during build from:

- Source: `Samples/KarbonClient/openapi-spec.json`
- OpenAPI Generator: `RestClient.Net.OpenApiGenerator.Cli`
- MCP Generator: `RestClient.Net.McpGenerator.Cli`

To regenerate after updating the OpenAPI spec:

```bash
dotnet build Samples/KarbonClient/KarbonClient.csproj
```

## Architecture

This implementation follows the RestClient.Net functional patterns:

- **Result types** - All API calls return `Result<TSuccess, HttpError<TError>>` for exhaustive error handling
- **Delegate pattern** - API methods return reusable delegates for composition
- **IHttpClientFactory** - Proper HTTP client lifecycle management
- **Pure functions** - Immutable types and functional programming style

## Learn More

- [Karbon API Documentation](https://developers.karbonhq.com)
- [RestClient.Net Documentation](../../README.md)
- [Model Context Protocol](https://modelcontextprotocol.io)
- [Claude Code](https://claude.com/claude-code)

## License

This sample is part of the RestClient.Net project and follows the same license terms.
