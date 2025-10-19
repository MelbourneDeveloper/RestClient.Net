#!/bin/bash

# Add the NucliaDB MCP server to Claude Code
# This allows Claude Code to interact with NucliaDB via the Model Context Protocol

# Get the absolute path to the project directory
SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
PROJECT_PATH="$SCRIPT_DIR/NucliaDbClient.McpServer.csproj"

# Add the MCP server to Claude Code configuration
claude mcp add nuclia-db dotnet run --project "$PROJECT_PATH" --env NUCLIA_BASE_URL=http://localhost:8080/api/v1

echo "NucliaDB MCP server added to Claude Code!"
echo ""
echo "Make sure NucliaDB is running before using the MCP tools:"
echo "  cd $SCRIPT_DIR && ./start-mcp-server.sh"
echo ""
echo "You can verify the server is configured by running:"
echo "  claude-code mcp list"
