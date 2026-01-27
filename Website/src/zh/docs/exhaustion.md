---
layout: layouts/docs.njk
title: Exhaustion 分析器
lang: zh
permalink: /zh/docs/exhaustion/
eleventyNavigation:
  key: Exhaustion
  order: 4
---

# 穷尽性检查

Exhaustion 分析器确保你在模式匹配时处理每种可能的情况。

## 不使用 Exhaustion（危险）

```csharp
// 编译通过但可能在运行时崩溃！
var output = result switch
{
    OkPost(var post) => $"成功: {post.Title}",
    ErrorPost(ResponseErrorPost(var err, var status, _)) => $"错误 {status}",
    // 遗漏了 ExceptionErrorPost 情况！
};
```

## 使用 Exhaustion（安全）

```
error EXHAUSTION001: Result 上的 Switch 不完整;
已匹配: Ok<Post, HttpError<ErrorResponse>>, Error<Post, HttpError<ErrorResponse>> with ErrorResponseError<ErrorResponse>
遗漏: Error<Post, HttpError<ErrorResponse>> with ExceptionError<ErrorResponse>
```

你的构建将失败，直到处理所有情况。**运行时崩溃变成编译时错误。**

## 安装

Exhaustion 会随 RestClient.Net 自动安装，或单独安装:

```bash
dotnet add package Exhaustion
```
