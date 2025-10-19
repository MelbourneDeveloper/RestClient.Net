#!/bin/bash
set -e

SCRIPT_DIR="$( cd "$( dirname "${BASH_SOURCE[0]}" )" && pwd )"
COMPOSE_DIR="$SCRIPT_DIR/../NucliaDbClient"

cd "$COMPOSE_DIR"
docker compose down -v
docker compose up -d

until curl -s -f "http://localhost:8080" > /dev/null 2>&1; do sleep 1; done

cd "$SCRIPT_DIR"
export NUCLIA_BASE_URL="http://localhost:8080/api/v1"
dotnet run
