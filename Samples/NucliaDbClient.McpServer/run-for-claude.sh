#!/bin/bash

# Setup NucliaDB MCP server for Claude Code
# This script:
# 1. Starts NucliaDB via docker-compose (if not running)
# 2. Builds the MCP server
# 3. Adds it to Claude Code configuration

set -e

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
PROJECT_PATH="$SCRIPT_DIR/NucliaDbClient.McpServer.csproj"
DOCKER_DIR="$SCRIPT_DIR/../NucliaDbClient"
DOTNET_PATH=$(which dotnet)

echo "NucliaDB MCP Server Setup"
echo "========================="
echo ""

# Check if NucliaDB is running
if curl -s -f "http://localhost:8080" > /dev/null 2>&1; then
    echo "✓ NucliaDB is already running"
else
    echo "Starting NucliaDB..."
    cd "$DOCKER_DIR"
    docker-compose up -d

    # Wait for NucliaDB to be ready
    echo "Waiting for NucliaDB to start..."
    MAX_RETRIES=30
    RETRY_COUNT=0

    while [ $RETRY_COUNT -lt $MAX_RETRIES ]; do
        if curl -s -f "http://localhost:8080" > /dev/null 2>&1; then
            echo "✓ NucliaDB is ready!"
            break
        fi
        RETRY_COUNT=$((RETRY_COUNT + 1))
        if [ $RETRY_COUNT -eq $MAX_RETRIES ]; then
            echo "✗ Timeout waiting for NucliaDB"
            exit 1
        fi
        sleep 2
    done
fi

echo ""
echo "Building MCP server..."
cd "$SCRIPT_DIR"
dotnet build

echo ""
echo "Adding to Claude Code configuration..."
claude mcp add --transport stdio nucliadb-mcp --env NUCLIA_BASE_URL=http://localhost:8080/api/v1 -- "$DOTNET_PATH" run --project "$PROJECT_PATH" --no-build

echo ""
echo "✓ Setup complete!"
echo ""
echo "Verify with: claude mcp list"
echo "The server should appear as 'nucliadb-mcp' with a ✓ status"
echo ""
echo "Available MCP tools (filtered to Search operations only):"
echo "  - Ask questions on knowledge boxes"
echo "  - Search resources (full-text, semantic, catalog)"
echo "  - List and query knowledge box contents"
echo "  - 18 total Search-related operations"
echo ""
echo "Note: The MCP tools are filtered to Search operations only to avoid"
echo "      flooding Claude Code. To include more operations, regenerate"
echo "      with different tags (see README.md)."
