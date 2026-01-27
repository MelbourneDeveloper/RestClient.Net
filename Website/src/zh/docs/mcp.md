---
layout: layouts/docs.njk
title: MCP 服务器
lang: zh
permalink: /zh/docs/mcp/
eleventyNavigation:
  key: MCP 服务器
  order: 3
---

# MCP 服务器生成

从 OpenAPI 规范为 Claude Code 生成模型上下文协议服务器。

## 首先生成客户端

```bash
dotnet run --project RestClient.Net.OpenApiGenerator.Cli -- \
  -u api.yaml -o Generated -n YourApi.Generated
```

## 生成 MCP 工具

```bash
dotnet run --project RestClient.Net.McpGenerator.Cli -- \
  --openapi-url api.yaml \
  --output-file Generated/McpTools.g.cs \
  --namespace YourApi.Mcp \
  --server-name YourApi \
  --ext-namespace YourApi.Generated \
  --tags "Search,Resources"
```

## Claude Code 集成

添加到你的 Claude Code 配置:

```json
{
  "mcpServers": {
    "yourapi": {
      "command": "dotnet",
      "args": ["run", "--project", "YourApi.McpServer"]
    }
  }
}
```
