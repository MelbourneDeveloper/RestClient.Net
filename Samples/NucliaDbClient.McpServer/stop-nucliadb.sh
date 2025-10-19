#!/bin/bash

set -e

echo "Stopping NucliaDB..."

# Get the directory where this script is located
SCRIPT_DIR="$( cd "$( dirname "${BASH_SOURCE[0]}" )" && pwd )"
NUCLIA_DIR="$SCRIPT_DIR/.."

cd "$NUCLIA_DIR"
docker-compose down

echo "✓ NucliaDB stopped"
