# NucliaDB MCP Server

This is a Model Context Protocol (MCP) server that provides Claude Code with access to NucliaDB REST API operations.

## Prerequisites

- .NET 9.0 SDK
- Docker and Docker Compose
- Claude Code (latest version)

## Quick Start

### 1. Start NucliaDB and MCP Server

```bash
cd Samples/NucliaDbClient.McpServer
./start-mcp-server.sh
```

This will:
- Start NucliaDB via docker-compose (PostgreSQL + NucliaDB containers)
- Wait for NucliaDB to be ready
- Build and run the MCP server

### 2. Alternative: Run MCP Server Only

If NucliaDB is already running:

```bash
./run-mcp-server.sh
```

### 3. Stop NucliaDB

```bash
./stop-nucliadb.sh
```

## Configuration for Claude Code

### Option 1: Add to your existing MCP settings

Open your Claude Code MCP settings file and add the NucliaDB server configuration:

**Location:** `~/.config/Claude/claude_desktop_config.json` (Linux/macOS) or `%APPDATA%/Claude/claude_desktop_config.json` (Windows)

Add this to the `mcpServers` section:

```json
{
  "mcpServers": {
    "nuclia-db": {
      "command": "/usr/local/share/dotnet/dotnet",
      "args": [
        "run",
        "--project",
        "/Users/christianfindlay/Documents/Code/RestClient.Net/Samples/NucliaDbClient.McpServer/NucliaDbClient.McpServer.csproj"
      ],
      "env": {
        "NUCLIA_BASE_URL": "http://localhost:8080/api/v1"
      }
    }
  }
}
```

**Note:** Update the paths to match your system:
- Replace `/usr/local/share/dotnet/dotnet` with your `dotnet` path (run `which dotnet` to find it)
- Replace the project path with the actual path on your system

### Option 2: Use the provided configuration file

Copy the example configuration:

```bash
cp claude-mcp-config.example.json ~/.config/Claude/claude_desktop_config.json
```

Then edit the file to update paths for your system.

## Testing the MCP Server

1. Ensure NucliaDB is running:
   ```bash
   curl http://localhost:8080
   ```

2. In Claude Code, you should now see NucliaDB tools available in the MCP tools list

3. Try asking Claude to:
   - "List all knowledge boxes"
   - "Search for documents in knowledge box X"
   - "Ask a question on knowledge box Y"

## Available Tools

The MCP server provides access to all NucliaDB REST API operations, including:

- **Knowledge Box Management**: Get, create, delete knowledge boxes
- **Search**: Full-text search, semantic search, catalog search
- **Ask**: Question-answering on knowledge bases
- **Resources**: Create, read, update, delete resources
- **Labels & Entities**: Manage labels and entity recognition
- **Configuration**: Configure models and settings

See the [generated MCP tools](../NucliaDbClient/Generated/NucliaDbMcpTools.g.cs) for the complete list.

## Environment Variables

- `NUCLIA_BASE_URL`: NucliaDB API base URL (default: `http://localhost:8080/api/v1`)

## Troubleshooting

### NucliaDB won't start

```bash
# Check if ports are in use
lsof -i :8080
lsof -i :5432

# Check docker logs
docker-compose logs
```

### MCP Server connection issues

1. Check that NucliaDB is accessible:
   ```bash
   curl http://localhost:8080/api/v1
   ```

2. Verify the `dotnet` path in your Claude Code config:
   ```bash
   which dotnet
   ```

3. Check Claude Code logs for MCP connection errors

### Build errors

```bash
cd /Users/christianfindlay/Documents/Code/RestClient.Net
dotnet build
```

## Architecture

This MCP server is automatically generated from the NucliaDB OpenAPI specification:

1. **OpenAPI Spec** (`Samples/NucliaDbClient/api.yaml`) defines the NucliaDB REST API
2. **RestClient.Net.OpenApiGenerator** generates C# extension methods from the spec
3. **RestClient.Net.McpGenerator** generates MCP tool wrappers around the extension methods
4. **NucliaDbClient.McpServer** hosts the MCP server using the generated tools

## Development

To regenerate the MCP tools after updating the OpenAPI spec:

```bash
cd /Users/christianfindlay/Documents/Code/RestClient.Net

# Regenerate the API client
dotnet run --project RestClient.Net.OpenApiGenerator.Cli/RestClient.Net.OpenApiGenerator.Cli.csproj -- \
  -u Samples/NucliaDbClient/api.yaml \
  -o Samples/NucliaDbClient/Generated \
  -n NucliaDB.Generated \
  -c NucliaDBApiExtensions

# Regenerate the MCP tools
dotnet run --project RestClient.Net.McpGenerator.Cli/RestClient.Net.McpGenerator.Cli.csproj -- \
  --openapi-url Samples/NucliaDbClient/api.yaml \
  --output-file Samples/NucliaDbClient/Generated/NucliaDbMcpTools.g.cs \
  --namespace NucliaDB.Mcp \
  --server-name NucliaDb \
  --ext-namespace NucliaDB.Generated

# Rebuild the MCP server
dotnet build Samples/NucliaDbClient.McpServer
```
