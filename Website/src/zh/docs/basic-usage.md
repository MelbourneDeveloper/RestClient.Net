---
layout: layouts/docs.njk
title: 基本用法
description: 学习使用 RestClient.Net 进行类型安全 REST 调用的基础知识。包含 GET、POST、PUT、DELETE 示例及正确的错误处理。
keywords: RestClient.Net 教程, C# REST 客户端, HttpClient 扩展, 类型安全 HTTP 调用
lang: zh
permalink: /zh/docs/basic-usage/
eleventyNavigation:
  key: 基本用法
  order: 3
faq:
  - question: 如何使用 RestClient.Net 发起 GET 请求？
    answer: 使用 HttpClient 上的 GetAsync 扩展方法，传入 URL、成功反序列化器和错误反序列化器。返回的结果是一个必须进行模式匹配的可辨识联合。
  - question: ToAbsoluteUrl() 扩展是什么？
    answer: ToAbsoluteUrl() 将字符串转换为 AbsoluteUrl 类型，在编译时验证 URL 格式并提供类型安全。
  - question: 为什么需要反序列化器函数？
    answer: 反序列化器函数允许您精确控制响应的解析方式，支持任何序列化格式（JSON、XML 等）和自定义验证。
---

# 基本用法

本指南涵盖使用 RestClient.Net 进行 REST 调用的基础知识。

## 第一个请求

让我们发起一个简单的 GET 请求来获取帖子：

```csharp
using System.Net.Http.Json;
using RestClient.Net;
using Urls;

// 定义模型
record Post(int UserId, int Id, string Title, string Body);
record ErrorResponse(string Message);

// 创建 HttpClient（在生产环境中使用 IHttpClientFactory）
using var httpClient = new HttpClient();

// 发起请求
var result = await httpClient.GetAsync(
    url: "https://jsonplaceholder.typicode.com/posts/1".ToAbsoluteUrl(),
    deserializeSuccess: async (content, ct) =>
        await content.ReadFromJsonAsync<Post>(ct)
        ?? throw new InvalidOperationException("空响应"),
    deserializeError: async (content, ct) =>
        await content.ReadFromJsonAsync<ErrorResponse>(ct)
        ?? new ErrorResponse("未知错误")
);

// 处理结果 - 必须处理所有情况
var message = result switch
{
    Outcome.Result<Post, Outcome.HttpError<ErrorResponse>>.Ok(var post) =>
        $"成功: {post.Title}",

    Outcome.Result<Post, Outcome.HttpError<ErrorResponse>>.Error(
        Outcome.HttpError<ErrorResponse>.ResponseError(var err, var status, _)) =>
        $"API 错误 {status}: {err.Message}",

    Outcome.Result<Post, Outcome.HttpError<ErrorResponse>>.Error(
        Outcome.HttpError<ErrorResponse>.ExceptionError(var ex)) =>
        $"异常: {ex.Message}",
};

Console.WriteLine(message);
```

## 使用类型别名

完整类型名称很冗长。在 `GlobalUsings.cs` 中添加类型别名：

```csharp
// GlobalUsings.cs
global using OkPost = Outcome.Result<Post, Outcome.HttpError<ErrorResponse>>
    .Ok<Post, Outcome.HttpError<ErrorResponse>>;

global using ErrorPost = Outcome.Result<Post, Outcome.HttpError<ErrorResponse>>
    .Error<Post, Outcome.HttpError<ErrorResponse>>;

global using ResponseErrorPost = Outcome.HttpError<ErrorResponse>.ErrorResponseError;

global using ExceptionErrorPost = Outcome.HttpError<ErrorResponse>.ExceptionError;
```

现在模式匹配更简洁了：

```csharp
var message = result switch
{
    OkPost(var post) => $"成功: {post.Title}",
    ErrorPost(ResponseErrorPost(var err, var status, _)) => $"API 错误 {status}: {err.Message}",
    ErrorPost(ExceptionErrorPost(var ex)) => $"异常: {ex.Message}",
};
```

## POST 请求

使用 POST 创建新资源：

```csharp
record CreatePostRequest(string Title, string Body, int UserId);

var newPost = new CreatePostRequest("我的标题", "我的内容", 1);

var result = await httpClient.PostAsync(
    url: "https://jsonplaceholder.typicode.com/posts".ToAbsoluteUrl(),
    body: newPost,
    serializeRequest: body => JsonContent.Create(body),
    deserializeSuccess: async (content, ct) =>
        await content.ReadFromJsonAsync<Post>(ct)
        ?? throw new InvalidOperationException("空响应"),
    deserializeError: async (content, ct) =>
        await content.ReadFromJsonAsync<ErrorResponse>(ct)
        ?? new ErrorResponse("未知错误")
);
```

## PUT 请求

替换现有资源：

```csharp
record UpdatePostRequest(int Id, string Title, string Body, int UserId);

var updatedPost = new UpdatePostRequest(1, "更新的标题", "更新的内容", 1);

var result = await httpClient.PutAsync(
    url: "https://jsonplaceholder.typicode.com/posts/1".ToAbsoluteUrl(),
    body: updatedPost,
    serializeRequest: body => JsonContent.Create(body),
    deserializeSuccess: async (content, ct) =>
        await content.ReadFromJsonAsync<Post>(ct)
        ?? throw new InvalidOperationException("空响应"),
    deserializeError: async (content, ct) =>
        await content.ReadFromJsonAsync<ErrorResponse>(ct)
        ?? new ErrorResponse("未知错误")
);
```

## DELETE 请求

删除资源：

```csharp
record DeleteResponse(bool Success);

var result = await httpClient.DeleteAsync(
    url: "https://jsonplaceholder.typicode.com/posts/1".ToAbsoluteUrl(),
    deserializeSuccess: async (content, ct) =>
        new DeleteResponse(true),
    deserializeError: async (content, ct) =>
        await content.ReadFromJsonAsync<ErrorResponse>(ct)
        ?? new ErrorResponse("未知错误")
);
```

## PATCH 请求

部分更新资源：

```csharp
record PatchPostRequest(string? Title = null, string? Body = null);

var patch = new PatchPostRequest(Title: "只更新标题");

var result = await httpClient.PatchAsync(
    url: "https://jsonplaceholder.typicode.com/posts/1".ToAbsoluteUrl(),
    body: patch,
    serializeRequest: body => JsonContent.Create(body),
    deserializeSuccess: async (content, ct) =>
        await content.ReadFromJsonAsync<Post>(ct)
        ?? throw new InvalidOperationException("空响应"),
    deserializeError: async (content, ct) =>
        await content.ReadFromJsonAsync<ErrorResponse>(ct)
        ?? new ErrorResponse("未知错误")
);
```

## 可复用的反序列化器

创建可复用的反序列化器方法：

```csharp
public static class Deserializers
{
    public static async Task<T> Json<T>(HttpContent content, CancellationToken ct)
        where T : class =>
        await content.ReadFromJsonAsync<T>(ct)
        ?? throw new InvalidOperationException($"反序列化 {typeof(T).Name} 失败");

    public static async Task<ErrorResponse> Error(HttpContent content, CancellationToken ct) =>
        await content.ReadFromJsonAsync<ErrorResponse>(ct)
        ?? new ErrorResponse("未知错误");
}

// 使用方式
var result = await httpClient.GetAsync(
    url: "https://api.example.com/posts/1".ToAbsoluteUrl(),
    deserializeSuccess: Deserializers.Json<Post>,
    deserializeError: Deserializers.Error
);
```

## 添加请求头

在 HttpClient 上设置请求头：

```csharp
httpClient.DefaultRequestHeaders.Authorization =
    new AuthenticationHeaderValue("Bearer", "your-token");

httpClient.DefaultRequestHeaders.Add("X-Custom-Header", "value");

var result = await httpClient.GetAsync(...);
```

## 查询参数

使用查询参数构建 URL：

```csharp
// 使用字符串插值
var url = $"https://api.example.com/posts?userId={userId}&page={page}".ToAbsoluteUrl();

// 或使用 URL 构建库
var result = await httpClient.GetAsync(
    url: url,
    deserializeSuccess: Deserializers.Json<List<Post>>,
    deserializeError: Deserializers.Error
);
```

## 取消操作

传递 `CancellationToken` 以支持可取消的请求：

```csharp
var cts = new CancellationTokenSource(TimeSpan.FromSeconds(10));

var result = await httpClient.GetAsync(
    url: "https://api.example.com/posts/1".ToAbsoluteUrl(),
    deserializeSuccess: Deserializers.Json<Post>,
    deserializeError: Deserializers.Error,
    cancellationToken: cts.Token
);
```

## 最佳实践

1. **在生产环境中使用 IHttpClientFactory** 以实现正确的连接池管理
2. **在 GlobalUsings.cs 中定义类型别名** 以保持代码简洁
3. **创建可复用的反序列化器** 以避免重复
4. **始终处理所有情况** 在 switch 表达式中
5. **使用取消令牌** 用于面向用户的请求

## 下一步

- [错误处理](/zh/docs/error-handling/) - 深入了解 Result 类型
- [高级用法](/zh/docs/advanced-usage/) - 重试策略、中间件等
- [API 参考](/zh/api/) - 完整的方法文档
