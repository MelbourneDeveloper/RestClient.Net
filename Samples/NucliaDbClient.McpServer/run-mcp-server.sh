#!/bin/bash

# Quick run script for the MCP server (assumes NucliaDB is already running)
set -e

SCRIPT_DIR="$( cd "$( dirname "${BASH_SOURCE[0]}" )" && pwd )"

echo "Starting NucliaDB MCP Server..."
echo "==============================="
echo ""

# Check if NucliaDB is running
if ! curl -s -f "http://localhost:8080" > /dev/null 2>&1; then
    echo "⚠ Warning: NucliaDB doesn't appear to be running on http://localhost:8080"
    echo "Run ./start-mcp-server.sh to start both docker-compose and the MCP server"
    echo ""
fi

cd "$SCRIPT_DIR"

# Set environment variable for NucliaDB URL
export NUCLIA_BASE_URL="http://localhost:8080/api/v1"

# Run the MCP server
dotnet run
