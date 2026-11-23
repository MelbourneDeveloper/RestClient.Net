---
layout: layouts/docs.njk
title: MCP 生成器
lang: zh
permalink: /zh/api/mcp-generator/
eleventyNavigation:
  key: MCP 生成器
  parent: API 参考
  order: 6
---

# MCP 生成器

从 OpenAPI 规范生成模型上下文协议（MCP）服务器，用于与 Claude Code 和其他工具的 AI 集成。

## 安装

```bash
dotnet add package RestClient.Net.McpGenerator
```

## 前置条件

首先，生成 REST 客户端：

```bash
dotnet run --project RestClient.Net.OpenApiGenerator.Cli -- \
  -u api.yaml -o Generated -n YourApi.Generated
```

## 命令行使用

```bash
dotnet run --project RestClient.Net.McpGenerator.Cli -- \
  --openapi-url api.yaml \
  --output-file Generated/McpTools.g.cs \
  --namespace YourApi.Mcp \
  --server-name YourApi \
  --ext-namespace YourApi.Generated \
  --tags "Search,Resources"
```

### 命令行选项

| 选项 | 描述 |
|------|------|
| \`--openapi-url\` | OpenAPI 规范的路径 |
| \`--output-file\` | 生成的 MCP 工具输出文件 |
| \`--namespace\` | MCP 服务器的 C# 命名空间 |
| \`--server-name\` | MCP 服务器的名称 |
| \`--ext-namespace\` | 生成的 REST 客户端的命名空间 |
| \`--tags\` | 要包含的 OpenAPI 标签（逗号分隔） |

## 生成的代码

生成器创建包装 REST 客户端的 MCP 工具定义：

```csharp
[McpServerToolType]
public static partial class McpTools
{
    [McpServerTool(Name = "get_user")]
    [Description("通过 ID 获取用户")]
    public static async Task<string> GetUser(
        [Description("用户 ID")] string id,
        HttpClient httpClient,
        CancellationToken ct)
    {
        var result = await httpClient.GetUserById(id, ct);
        return result switch
        {
            OkUser(var user) => JsonSerializer.Serialize(user),
            ErrorUser(var error) => \$"错误: {error}"
        };
    }
}
```

## Claude Code 集成

添加到您的 Claude Code 配置（\`.claude/settings.json\` 或 \`claude_desktop_config.json\`）：

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

## MCP 服务器项目

创建控制台应用程序来托管 MCP 服务器：

```csharp
// Program.cs
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using YourApi.Mcp;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddHttpClient();
builder.Services.AddMcpServer()
    .WithStdioServerTransport()
    .WithToolsFromAssembly(typeof(McpTools).Assembly);

var host = builder.Build();
await host.RunAsync();
```

## 工具命名

OpenAPI 操作会转换为 MCP 工具名称：

| OpenAPI | MCP 工具 |
|---------|----------|
| \`GET /users/{id}\` | \`get_user\` |
| \`POST /users\` | \`create_user\` |
| \`PUT /users/{id}\` | \`update_user\` |
| \`DELETE /users/{id}\` | \`delete_user\` |

## 按标签筛选

使用 \`--tags\` 为特定 API 部分生成工具：

```bash
# 仅生成搜索端点的工具
--tags "Search"

# 多个标签
--tags "Search,Users,Products"
```

## 错误处理

生成的工具在成功时返回 JSON，失败时返回错误字符串，便于 AI 理解：

```csharp
// 成功：JSON 响应
{"id": "123", "name": "Alice", "email": "alice@example.com"}

// 错误：人类可读的消息
"错误: 用户未找到 (HTTP 404)"
```
