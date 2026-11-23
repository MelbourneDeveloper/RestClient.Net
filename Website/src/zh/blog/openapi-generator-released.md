---
layout: layouts/blog.njk
title: OpenAPI 生成器发布
date: 2024-02-20
author: Christian Findlay
excerpt: 从 OpenAPI 3.x 规范生成类型安全的 C# 客户端，包含自动 Result 类型别名。
lang: zh
permalink: /zh/blog/openapi-generator-released/
tags:
  - zhposts
  - release
---

# OpenAPI 生成器发布

我们很高兴宣布 RestClient.Net OpenAPI 生成器的发布！

## 什么是 OpenAPI 生成器？

OpenAPI 生成器从 OpenAPI 3.x 规范自动生成类型安全的 C# 客户端代码。生成的代码包含：

- 所有端点的模型类
- 使用 RestClient.Net Result 类型的 HttpClient 扩展方法
- 用于简洁模式匹配的类型别名

## 安装

```bash
dotnet add package RestClient.Net.OpenApiGenerator
```

## 使用方法

```bash
dotnet run --project RestClient.Net.OpenApiGenerator.Cli -- \
  -u https://api.example.com/openapi.yaml \
  -o Generated \
  -n MyApi.Generated
```

## 生成的代码示例

```csharp
using MyApi.Generated;

var result = await httpClient.GetUserById("123", ct);

var message = result switch
{
    OkUser(var user) => $"找到用户: {user.Name}",
    ErrorUser(var err) => $"错误: {err.StatusCode}",
};
```

立即开始使用 OpenAPI 生成器！
