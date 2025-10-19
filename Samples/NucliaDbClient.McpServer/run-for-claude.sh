#!/bin/bash

# Add the NucliaDB MCP server to Claude Code
# This allows Claude Code to interact with NucliaDB via the Model Context Protocol

set -e

# Get the absolute path to the project directory
SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
PROJECT_PATH="$SCRIPT_DIR/NucliaDbClient.McpServer.csproj"

# Find the dotnet executable
DOTNET_PATH=$(which dotnet)

echo "Configuring NucliaDB MCP server for Claude Code..."
echo "  Script directory: $SCRIPT_DIR"
echo "  Project path: $PROJECT_PATH"
echo "  Dotnet path: $DOTNET_PATH"
echo ""

# Add the MCP server to Claude Code configuration
# The command structure is: claude mcp add [options] <name> [--env KEY=value] -- <command> [args...]
claude mcp add --transport stdio nucliadb-mcp --env NUCLIA_BASE_URL=http://localhost:8080/api/v1 -- "$DOTNET_PATH" run --project "$PROJECT_PATH" --no-build

echo ""
echo "✓ NucliaDB MCP server added to Claude Code!"
echo ""
echo "Next steps:"
echo "  1. Make sure NucliaDB is running:"
echo "     cd $SCRIPT_DIR && docker-compose up -d"
echo ""
echo "  2. Verify the MCP server is configured:"
echo "     claude mcp list"
echo ""
echo "  3. Test the connection:"
echo "     The server should appear as 'nucliadb-mcp' with a ✓ or ✗ status"
echo ""
echo "Available MCP tools:"
echo "  - Knowledge box management (get, create, delete)"
echo "  - Search (full-text, semantic, catalog)"
echo "  - Ask (question-answering)"
echo "  - Resources (CRUD operations)"
echo "  - And 100+ more NucliaDB API operations!"
