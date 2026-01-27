---
layout: layouts/docs.njk
title: OpenAPI 生成器
lang: zh
permalink: /zh/api/openapi-generator/
eleventyNavigation:
  key: OpenAPI 生成器
  parent: API 参考
  order: 5
---

# OpenAPI 生成器

从 OpenAPI 3.x 规范生成类型安全的 C# 客户端。

## 安装

```bash
dotnet add package RestClient.Net.OpenApiGenerator
```

## 命令行使用

```bash
dotnet run --project RestClient.Net.OpenApiGenerator.Cli -- \
  -u api.yaml \
  -o Generated \
  -n YourApi.Generated
```

### 命令行选项

| 选项 | 简写 | 描述 |
|------|------|------|
| \`--openapi-url\` | \`-u\` | OpenAPI 规范的路径（YAML 或 JSON） |
| \`--output\` | \`-o\` | 生成文件的输出目录 |
| \`--namespace\` | \`-n\` | 生成代码的 C# 命名空间 |
| \`--client-name\` | \`-c\` | 生成的客户端类名称前缀 |

## 生成的代码

生成器会创建：

1. **模型类** - 所有 schema 对应的类
2. **HttpClient 扩展方法** - 每个端点对应的方法
3. **Result 类型别名** - 简洁的模式匹配

### 输出示例

对于包含 \`/users/{id}\` 端点的 OpenAPI 规范：

```csharp
// 生成的扩展方法
public static async Task<ResultUser> GetUserById(
    this HttpClient httpClient,
    string id,
    CancellationToken ct = default)
{
    return await httpClient.GetAsync(
        url: \$"https://api.example.com/users/{id}".ToAbsoluteUrl(),
        deserializeSuccess: async (r, c) => await r.Content.ReadFromJsonAsync<User>(c),
        deserializeError: async (r, c) => await r.Content.ReadFromJsonAsync<ErrorResponse>(c),
        ct
    );
}

// 生成的类型别名
global using ResultUser = Outcome.Result<User, Outcome.HttpError<ErrorResponse>>;
global using OkUser = ResultUser.Ok<User, Outcome.HttpError<ErrorResponse>>;
global using ErrorUser = ResultUser.Error<User, Outcome.HttpError<ErrorResponse>>;
```

### 使用方法

```csharp
using YourApi.Generated;

var httpClient = factory.CreateClient();

// 类型安全的 API 调用
var result = await httpClient.GetUserById("123");

// 模式匹配处理结果
var output = result switch
{
    OkUser(var user) => \$"找到: {user.Name}",
    ErrorUser(ResponseErrorUser(var err, var status, _)) => \$"错误 {status}",
    ErrorUser(ExceptionErrorUser(var ex)) => \$"异常: {ex.Message}",
};
```

## 支持的 OpenAPI 特性

- **HTTP 方法：** GET、POST、PUT、DELETE、PATCH
- **参数：** path、query、header
- **请求体：** JSON、表单数据
- **响应：** 所有状态码、多种内容类型
- **Schema：** objects、arrays、enums、oneOf、allOf、anyOf
- **引用：** \$ref 本地和远程 schema

## 配置

### 自定义基础 URL

```csharp
// 生成的代码使用 OpenAPI 中的服务器 URL
// 运行时通过 HttpClient.BaseAddress 覆盖
httpClient.BaseAddress = new Uri("https://staging.api.example.com/");
```

### 自定义序列化

生成的代码默认使用 \`System.Text.Json\`。如需自定义序列化，可修改生成的反序列化器。
