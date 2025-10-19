#!/bin/bash
# Test if MCP server exposes tools correctly

# Send tools/list request to the MCP server
echo '{"jsonrpc":"2.0","id":1,"method":"tools/list","params":{}}' | \
  dotnet run --project /Users/christianfindlay/Documents/Code/RestClient.Net/Samples/NucliaDbClient.McpServer/NucliaDbClient.McpServer.csproj --no-build 2>/dev/null | \
  grep -v "^info:" | \
  head -20
