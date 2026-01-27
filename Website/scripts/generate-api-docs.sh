#!/bin/bash

# Generate API documentation for RestClient.Net
# This script generates markdown files from C# source in the RestClient.Net repo

set -e

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
WEBSITE_DIR="$(dirname "$SCRIPT_DIR")"

cd "$WEBSITE_DIR"

# Run the Node.js generator
node "$SCRIPT_DIR/generate-api-docs.js"
