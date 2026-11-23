---
layout: layouts/docs.njk
title: 安装指南
description: 安装 RestClient.Net 及相关包，实现 C# 中类型安全的 REST 调用。包含核心库、OpenAPI 生成器、MCP 服务器和穷尽性分析器的 NuGet 包。
keywords: RestClient.Net 安装, NuGet 包, dotnet add package, C# REST 客户端设置
lang: zh
permalink: /zh/docs/installation/
eleventyNavigation:
  key: 安装指南
  order: 2
faq:
  - question: 如何安装 RestClient.Net？
    answer: 在项目目录中运行 'dotnet add package RestClient.Net' 即可从 NuGet 安装核心库。
  - question: RestClient.Net 支持哪些 .NET 版本？
    answer: RestClient.Net 需要 .NET 8.0 或更高版本，以利用最新的 C# 模式匹配语言特性。
  - question: 需要单独安装 Exhaustion 分析器吗？
    answer: Exhaustion 分析器在安装 RestClient.Net 时会自动包含。您也可以通过 'dotnet add package Exhaustion' 单独安装。
---

<script type="application/ld+json">
{
  "@context": "https://schema.org",
  "@type": "HowTo",
  "name": "如何安装 RestClient.Net",
  "description": "安装 RestClient.Net NuGet 包的分步指南",
  "totalTime": "PT2M",
  "step": [
    {
      "@type": "HowToStep",
      "name": "安装核心包",
      "text": "运行 'dotnet add package RestClient.Net' 安装主库"
    },
    {
      "@type": "HowToStep",
      "name": "添加全局 using",
      "text": "创建 GlobalUsings.cs 文件，添加类型别名以实现更简洁的模式匹配"
    }
  ]
}
</script>

# 安装指南

RestClient.Net 以 NuGet 包形式分发。本指南涵盖所有可用包的安装方法。

## 前置条件

- **.NET 8.0** 或更高版本
- 代码编辑器（推荐使用 Visual Studio、VS Code 或 Rider）
- 对 C# 和 `HttpClient` 有基本了解

## 核心包

主包提供带有 Result 类型的 `HttpClient` 扩展方法：

```bash
dotnet add package RestClient.Net
```

自动包含以下内容：
- `HttpClient` 扩展方法（`GetAsync`、`PostAsync` 等）
- `Result<TSuccess, HttpError<TError>>` 类型
- `AbsoluteUrl` 类型及扩展
- **Exhaustion 分析器**，用于编译时穷尽性检查

## 包引用（csproj）

或者直接添加到 `.csproj` 文件：

```xml
<ItemGroup>
  <PackageReference Include="RestClient.Net" Version="6.*" />
</ItemGroup>
```

## 附加包

### OpenAPI 生成器

从 OpenAPI 3.x 规范生成类型安全的客户端：

```bash
dotnet add package RestClient.Net.OpenApiGenerator
```

### MCP 服务器生成器

生成用于 Claude Code 集成的模型上下文协议服务器：

```bash
dotnet add package RestClient.Net.McpGenerator
```

### 独立 Exhaustion 分析器

如果只需要穷尽性分析器而不需要 REST 客户端：

```bash
dotnet add package Exhaustion
```

## 快速设置

安装后，在项目根目录创建 `GlobalUsings.cs` 文件：

```csharp
// GlobalUsings.cs
global using System.Net.Http.Json;
global using System.Text.Json;
global using RestClient.Net;
global using Urls;
```

为每个使用的模型类型添加类型别名，以实现更简洁的模式匹配：

```csharp
// User 模型与 ApiError 的示例
global using OkUser = Outcome.Result<User, Outcome.HttpError<ApiError>>
    .Ok<User, Outcome.HttpError<ApiError>>;

global using ErrorUser = Outcome.Result<User, Outcome.HttpError<ApiError>>
    .Error<User, Outcome.HttpError<ApiError>>;

global using ResponseErrorUser = Outcome.HttpError<ApiError>.ErrorResponseError;

global using ExceptionErrorUser = Outcome.HttpError<ApiError>.ExceptionError;
```

## 验证安装

创建一个简单的测试来验证一切正常工作：

```csharp
using System.Text.Json;
using RestClient.Net;
using Urls;

// 定义模型
record Post(int UserId, int Id, string Title, string Body);
record ErrorResponse(string Message);

// 创建 HttpClient
using var httpClient = new HttpClient();

// 发起测试调用
var result = await httpClient.GetAsync(
    url: "https://jsonplaceholder.typicode.com/posts/1".ToAbsoluteUrl(),
    deserializeSuccess: async (content, ct) =>
        await content.ReadFromJsonAsync<Post>(ct) ?? throw new Exception("空响应"),
    deserializeError: async (content, ct) =>
        await content.ReadFromJsonAsync<ErrorResponse>(ct) ?? new ErrorResponse("未知错误")
);

Console.WriteLine(result switch
{
    Outcome.Result<Post, Outcome.HttpError<ErrorResponse>>.Ok(var post) =>
        $"成功: {post.Title}",
    Outcome.Result<Post, Outcome.HttpError<ErrorResponse>>.Error(var error) =>
        $"错误: {error}",
});
```

## IDE 支持

### Visual Studio

Exhaustion 分析器开箱即用。对于非穷尽的 switch 表达式，您将看到编译错误。

### VS Code

安装 C# 扩展（ms-dotnettools.csharp）以获得完整的分析器支持。

### Rider

JetBrains Rider 完全支持 Roslyn 分析器，包括 Exhaustion。

## 下一步

- [基本用法](/zh/docs/basic-usage/) - 学习基础知识
- [错误处理](/zh/docs/error-handling/) - 掌握 Result 类型
- [OpenAPI 生成器](/zh/docs/openapi/) - 从规范生成客户端
