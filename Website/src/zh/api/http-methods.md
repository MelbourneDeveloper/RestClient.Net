---
layout: layouts/docs.njk
title: HTTP 方法
lang: zh
permalink: /zh/api/http-methods/
eleventyNavigation:
  key: HTTP 方法
  parent: API 参考
  order: 1
---

# HTTP 方法

RestClient.Net 为 `HttpClient` 的所有标准 HTTP 方法提供类型安全的扩展方法。

## GET 请求

从服务器获取数据。

```csharp
var result = await httpClient.GetAsync(
    url: "https://api.example.com/posts/1".ToAbsoluteUrl(),
    deserializeSuccess: DeserializePost,
    deserializeError: DeserializeError
);
```

### 参数

| 参数 | 类型 | 描述 |
|------|------|------|
| `url` | `AbsoluteUrl` | 端点 URL（使用 `.ToAbsoluteUrl()` 扩展） |
| `deserializeSuccess` | `Func<HttpResponseMessage, Task<T>>` | 成功响应的反序列化器 |
| `deserializeError` | `Func<HttpResponseMessage, Task<TError>>` | 错误响应的反序列化器 |
| `cancellationToken` | `CancellationToken` | 可选的取消令牌 |

## POST 请求

发送数据以创建新资源。

```csharp
var result = await httpClient.PostAsync(
    url: "https://api.example.com/posts".ToAbsoluteUrl(),
    body: new CreatePostRequest("Title", "Body"),
    serializeBody: SerializeRequest,
    deserializeSuccess: DeserializePost,
    deserializeError: DeserializeError
);
```

### 参数

| 参数 | 类型 | 描述 |
|------|------|------|
| `url` | `AbsoluteUrl` | 端点 URL |
| `body` | `TBody` | 请求体对象 |
| `serializeBody` | `Func<TBody, HttpContent>` | 请求体序列化器 |
| `deserializeSuccess` | `Func<HttpResponseMessage, Task<T>>` | 成功响应的反序列化器 |
| `deserializeError` | `Func<HttpResponseMessage, Task<TError>>` | 错误响应的反序列化器 |

## PUT 请求

完全替换现有资源。

```csharp
var result = await httpClient.PutAsync(
    url: "https://api.example.com/posts/1".ToAbsoluteUrl(),
    body: new UpdatePostRequest(1, "New Title", "New Body"),
    serializeBody: SerializeRequest,
    deserializeSuccess: DeserializePost,
    deserializeError: DeserializeError
);
```

## PATCH 请求

部分更新现有资源。

```csharp
var result = await httpClient.PatchAsync(
    url: "https://api.example.com/posts/1".ToAbsoluteUrl(),
    body: new PatchPostRequest { Title = "Updated Title" },
    serializeBody: SerializeRequest,
    deserializeSuccess: DeserializePost,
    deserializeError: DeserializeError
);
```

## DELETE 请求

删除资源。

```csharp
var result = await httpClient.DeleteAsync(
    url: "https://api.example.com/posts/1".ToAbsoluteUrl(),
    deserializeSuccess: DeserializeDeleteResponse,
    deserializeError: DeserializeError
);
```

## 常见模式

### 添加请求头

```csharp
httpClient.DefaultRequestHeaders.Authorization =
    new AuthenticationHeaderValue("Bearer", token);

var result = await httpClient.GetAsync(...);
```

### 使用 IHttpClientFactory

```csharp
// 在 Startup.cs / Program.cs 中
services.AddHttpClient("api", client =>
{
    client.BaseAddress = new Uri("https://api.example.com");
    client.DefaultRequestHeaders.Add("Accept", "application/json");
});

// 在你的服务中
var httpClient = httpClientFactory.CreateClient("api");
var result = await httpClient.GetAsync(...);
```

## 返回类型

所有方法都返回 `Result<TSuccess, HttpError<TError>>`。有关处理响应的详细信息，请参阅 [Result 类型](/zh/api/result-types/)。
