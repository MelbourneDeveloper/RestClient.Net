---
layout: layouts/docs.njk
title: OpenAPI 生成器
lang: zh
permalink: /zh/docs/openapi/
eleventyNavigation:
  key: OpenAPI 生成器
  order: 2
---

# OpenAPI 客户端生成

从 OpenAPI 3.x 规范生成类型安全的 C# 客户端。

## 安装

```bash
dotnet add package RestClient.Net.OpenApiGenerator
```

## 使用方法

```bash
dotnet run --project RestClient.Net.OpenApiGenerator.Cli -- \
  -u api.yaml \
  -o Generated \
  -n YourApi.Generated
```

## 生成的代码

```csharp
using YourApi.Generated;

var httpClient = factory.CreateClient();

var user = await httpClient.GetUserById("123", ct);
var created = await httpClient.CreateUser(newUser, ct);

switch (user)
{
    case OkUser(var success):
        Console.WriteLine($"用户: {success.Name}");
        break;
    case ErrorUser(var error):
        Console.WriteLine($"错误: {error.StatusCode}");
        break;
}
```
