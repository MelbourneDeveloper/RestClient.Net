---
layout: layouts/blog.njk
title: RestClient.Net 介绍 - C# 中的类型安全 REST 调用
date: 2024-01-15
author: Christian Findlay
excerpt: 了解 RestClient.Net 如何通过可辨识联合和穷尽性检查为 HTTP 调用带来函数式编程模式。
lang: zh
permalink: /zh/blog/introducing-restclient/
tags:
  - zhposts
  - announcement
---

# RestClient.Net 介绍

RestClient.Net 是一个现代的、类型安全的 C# REST 客户端，为 HTTP 通信带来函数式编程模式。

## 为什么需要另一个 REST 客户端？

.NET 中的传统 HTTP 客户端有一个问题：它们会抛出异常。这迫使开发者使用 try/catch 块，使错误处理变得不一致且容易出错。

RestClient.Net 采用了不同的方法。每个 HTTP 调用返回一个必须显式处理的 `Result` 类型，而不是抛出异常。

## 可辨识联合的力量

```csharp
// 发起 GET 请求
var result = await httpClient.GetAsync(
    url: "https://api.example.com/posts/1".ToAbsoluteUrl(),
    deserializeSuccess: DeserializePost,
    deserializeError: DeserializeError
);

// 模式匹配 - 编译器强制你处理所有情况
var output = result switch
{
    OkPost(var post) => $"成功: {post.Title}",
    ErrorPost(ResponseErrorPost(var err, var status, _)) => $"API 错误: {status}",
    ErrorPost(ExceptionErrorPost(var ex)) => $"异常: {ex.Message}",
};
```

## 穷尽性检查

使用 [Exhaustion 分析器](https://www.nuget.org/packages/Exhaustion)，如果你遗漏了某个情况，代码将无法编译。不再有运行时惊喜。

## 开始使用

```bash
dotnet add package RestClient.Net
```

查看[文档](/zh/docs/)了解更多！
