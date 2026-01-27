---
layout: layouts/docs.njk
title: 入门指南
lang: zh
permalink: /zh/docs/
eleventyNavigation:
  key: 入门指南
  order: 1
---

# RestClient.Net 入门指南

RestClient.Net 是一个基于函数式编程模式构建的 C# 类型安全 REST 客户端。

## 安装

```bash
dotnet add package RestClient.Net
```

## 基本 GET 请求

```csharp
using System.Text.Json;
using RestClient.Net;
using Urls;

// 定义模型
internal sealed record Post(int UserId, int Id, string Title, string Body);
internal sealed record ErrorResponse(string Message);

// 创建 HttpClient
using var httpClient = new HttpClient();

// 发起 GET 调用
var result = await httpClient
    .GetAsync(
        url: "https://jsonplaceholder.typicode.com/posts/1".ToAbsoluteUrl(),
        deserializeSuccess: DeserializePost,
        deserializeError: DeserializeError
    );

// 模式匹配 - 必须处理所有情况
var output = result switch
{
    OkPost(var post) => $"成功: {post.Title}",
    ErrorPost(ResponseErrorPost(var err, var status, _)) => $"错误 {status}",
    ErrorPost(ExceptionErrorPost(var ex)) => $"异常: {ex.Message}",
};
```

## 类型别名

添加到 `GlobalUsings.cs`:

```csharp
global using OkPost = Outcome.Result<Post, Outcome.HttpError<ErrorResponse>>.Ok<Post, Outcome.HttpError<ErrorResponse>>;
global using ErrorPost = Outcome.Result<Post, Outcome.HttpError<ErrorResponse>>.Error<Post, Outcome.HttpError<ErrorResponse>>;
global using ResponseErrorPost = Outcome.HttpError<ErrorResponse>.ErrorResponseError;
global using ExceptionErrorPost = Outcome.HttpError<ErrorResponse>.ExceptionError;
```
