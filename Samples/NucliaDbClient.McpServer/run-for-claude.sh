#!/bin/bash

# Run script for Claude MCP integration
# This script is called by Claude to start the MCP server

SCRIPT_DIR="$( cd "$( dirname "${BASH_SOURCE[0]}" )" && pwd )"

# Set environment variable for NucliaDB URL
export NUCLIA_BASE_URL="http://localhost:8080/api/v1"

# Run the MCP server
cd "$SCRIPT_DIR"
exec dotnet run --no-build
