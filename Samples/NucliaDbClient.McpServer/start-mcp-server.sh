#!/bin/bash

set -e

echo "Starting NucliaDB MCP Server Setup"
echo "==================================="
echo ""

# Get the directory where this script is located
SCRIPT_DIR="$( cd "$( dirname "${BASH_SOURCE[0]}" )" && pwd )"
PROJECT_ROOT="$SCRIPT_DIR/../.."
NUCLIA_DIR="$SCRIPT_DIR/.."

# Start docker-compose
echo "Starting NucliaDB via docker-compose..."
cd "$NUCLIA_DIR"
docker-compose up -d

# Wait for NucliaDB to be ready
echo ""
echo "Waiting for NucliaDB to be ready..."
MAX_RETRIES=30
RETRY_COUNT=0
NUCLIA_URL="http://localhost:8080"

while [ $RETRY_COUNT -lt $MAX_RETRIES ]; do
    if curl -s -f "$NUCLIA_URL" > /dev/null 2>&1; then
        echo "✓ NucliaDB is ready!"
        break
    fi

    RETRY_COUNT=$((RETRY_COUNT + 1))
    if [ $RETRY_COUNT -eq $MAX_RETRIES ]; then
        echo "✗ Timeout waiting for NucliaDB to start"
        exit 1
    fi

    echo "  Waiting... ($RETRY_COUNT/$MAX_RETRIES)"
    sleep 2
done

echo ""
echo "Building MCP server..."
cd "$SCRIPT_DIR"
dotnet build

echo ""
echo "Starting NucliaDB MCP Server..."
echo "Server will communicate via stdio"
echo "==================================="
echo ""

# Set environment variable for NucliaDB URL
export NUCLIA_BASE_URL="http://localhost:8080/api/v1"

# Run the MCP server
dotnet run --no-build
