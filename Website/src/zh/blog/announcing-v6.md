---
layout: layouts/blog.njk
title: RestClient.Net v6 发布
date: 2024-03-15
author: Christian Findlay
excerpt: v6 版本带来了 MCP 服务器生成、改进的错误处理和更好的性能。
lang: zh
permalink: /zh/blog/announcing-v6/
tags:
  - zhposts
  - release
---

# RestClient.Net v6 发布

我们很高兴宣布 RestClient.Net v6 的发布，这是迄今为止最大的更新！

## 新功能

### MCP 服务器生成

现在你可以从 OpenAPI 规范生成模型上下文协议 (MCP) 服务器。这意味着你可以立即将 API 与 Claude Code 和其他 AI 工具集成。

```bash
dotnet run --project RestClient.Net.McpGenerator.Cli -- \
  --openapi-url api.yaml \
  --output-file Generated/McpTools.g.cs
```

### 改进的错误处理

v6 引入了更精细的错误类型，让你可以更精确地处理不同类型的失败：

```csharp
var output = result switch
{
    OkPost(var post) => HandleSuccess(post),
    ErrorPost(ResponseErrorPost(var err, var status, _)) => HandleApiError(err, status),
    ErrorPost(ExceptionErrorPost(var ex)) => HandleException(ex),
};
```

### 性能改进

- 减少内存分配
- 更快的序列化
- 更好的连接池利用

## 升级

```bash
dotnet add package RestClient.Net --version 6.0.0
```

查看[完整文档](/zh/docs/)了解所有新功能！
