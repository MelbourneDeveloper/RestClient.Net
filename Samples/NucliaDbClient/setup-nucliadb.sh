#!/bin/bash

# Start NucliaDB
echo "Starting NucliaDB..."
docker-compose up -d

# Wait for NucliaDB to be ready
echo "Waiting for NucliaDB to be ready..."
until curl -s http://localhost:8080/ > /dev/null; do
  echo "Waiting..."
  sleep 2
done

echo "NucliaDB is ready!"

# Create a knowledge box
echo "Creating test knowledge box..."
response=$(curl -s 'http://localhost:8080/api/v1/kbs' \
  -X POST \
  -H "X-NUCLIADB-ROLES: MANAGER" \
  -H "Content-Type: application/json" \
  --data-raw '{"slug": "test-kb", "title": "Test Knowledge Box"}')

# Extract the UUID from the response
kbid=$(echo $response | grep -o '"uuid":"[^"]*' | cut -d'"' -f4)

echo ""
echo "=========================================="
echo "NucliaDB is running!"
echo "=========================================="
echo "Admin UI: http://localhost:8080/admin"
echo "API Base URL: http://localhost:8080/api/v1"
echo "Knowledge Box ID: $kbid"
echo ""
echo "Set these environment variables:"
echo "export NUCLIA_BASE_URL=\"http://localhost:8080/api/v1\""
echo "export NUCLIA_KBID=\"$kbid\""
echo ""
echo "Or run tests directly with:"
echo "NUCLIA_BASE_URL=\"http://localhost:8080/api/v1\" NUCLIA_KBID=\"$kbid\" dotnet test"
echo "=========================================="
