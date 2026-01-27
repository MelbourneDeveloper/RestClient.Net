---
layout: layouts/docs.njk
title: HttpClient 扩展
lang: zh
permalink: /zh/docs/api/httpclient-extensions/
eleventyNavigation:
  key: HttpClient 扩展
  parent: API 参考
  order: 1
---

# HttpClient 扩展

`System.Net.Http.HttpClient` 的扩展方法，返回 `Result<TSuccess, HttpError<TError>>` 而不是抛出异常。

**命名空间：** `RestClient.Net`

## SendAsync

发送 HTTP 请求的核心方法，可完全控制所有参数。

```csharp
public static async Task<Result<TSuccess, HttpError<TError>>> SendAsync<TSuccess, TError>(
    this HttpClient httpClient,
    AbsoluteUrl url,
    HttpMethod httpMethod,
    Deserialize<TSuccess> deserializeSuccess,
    Deserialize<TError> deserializeError,
    IReadOnlyDictionary<string, string>? headers = null,
    HttpContent? requestBody = null,
    HttpAction? httpOperation = null,
    CancellationToken cancellationToken = default
)
```

### 类型参数

| 参数 | 描述 |
|-----------|-------------|
| `TSuccess` | 成功响应（2xx）反序列化的目标类型 |
| `TError` | 错误响应（非 2xx）反序列化的目标类型 |

### 参数

| 参数 | 类型 | 描述 |
|-----------|------|-------------|
| `httpClient` | `HttpClient` | HTTP 客户端实例 |
| `url` | `AbsoluteUrl` | 请求的绝对 URL |
| `httpMethod` | `HttpMethod` | HTTP 方法（GET、POST、PUT 等） |
| `deserializeSuccess` | `Deserialize<TSuccess>` | 反序列化成功响应的异步函数 |
| `deserializeError` | `Deserialize<TError>` | 反序列化错误响应的异步函数 |
| `headers` | `IReadOnlyDictionary<string, string>?` | 可选的请求头 |
| `requestBody` | `HttpContent?` | 可选的请求体内容 |
| `httpOperation` | `HttpAction?` | 可选的自定义 HTTP 操作处理器 |
| `cancellationToken` | `CancellationToken` | 取消令牌 |

### 返回值

`Task<Result<TSuccess, HttpError<TError>>>` - 结果为以下之一：
- `Ok<TSuccess>` 包含 2xx 状态码的反序列化响应
- `Error<HttpError<TError>>` 包含 `ErrorResponseError` 或 `ExceptionError`

---

## GetAsync

执行 GET 请求。

```csharp
public static Task<Result<TSuccess, HttpError<TError>>> GetAsync<TSuccess, TError>(
    this HttpClient httpClient,
    AbsoluteUrl url,
    Deserialize<TSuccess> deserializeSuccess,
    Deserialize<TError> deserializeError,
    IReadOnlyDictionary<string, string>? headers = null,
    CancellationToken cancellationToken = default
)
```

### 示例

```csharp
var result = await httpClient.GetAsync(
    url: "https://api.example.com/users/1".ToAbsoluteUrl(),
    deserializeSuccess: DeserializeJson<User>,
    deserializeError: DeserializeJson<ApiError>
);

var output = result switch
{
    Result<User, HttpError<ApiError>>.Ok<User, HttpError<ApiError>>(var user)
        => $"找到: {user.Name}",
    Result<User, HttpError<ApiError>>.Error<User, HttpError<ApiError>>(var error)
        => $"发生错误",
};
```

---

## PostAsync

执行带请求体的 POST 请求。

```csharp
public static Task<Result<TSuccess, HttpError<TError>>> PostAsync<TSuccess, TError>(
    this HttpClient httpClient,
    AbsoluteUrl url,
    HttpContent? requestBody,
    Deserialize<TSuccess> deserializeSuccess,
    Deserialize<TError> deserializeError,
    IReadOnlyDictionary<string, string>? headers = null,
    CancellationToken cancellationToken = default
)
```

### 示例

```csharp
var content = new StringContent(
    JsonSerializer.Serialize(new { Name = "张三", Email = "zhangsan@example.com" }),
    Encoding.UTF8,
    "application/json"
);

var result = await httpClient.PostAsync(
    url: "https://api.example.com/users".ToAbsoluteUrl(),
    requestBody: content,
    deserializeSuccess: DeserializeJson<User>,
    deserializeError: DeserializeJson<ApiError>
);
```

---

## PutAsync

执行带请求体的 PUT 请求。

```csharp
public static Task<Result<TSuccess, HttpError<TError>>> PutAsync<TSuccess, TError>(
    this HttpClient httpClient,
    AbsoluteUrl url,
    HttpContent? requestBody,
    Deserialize<TSuccess> deserializeSuccess,
    Deserialize<TError> deserializeError,
    IReadOnlyDictionary<string, string>? headers = null,
    CancellationToken cancellationToken = default
)
```

---

## DeleteAsync

执行 DELETE 请求。

```csharp
public static Task<Result<TSuccess, HttpError<TError>>> DeleteAsync<TSuccess, TError>(
    this HttpClient httpClient,
    AbsoluteUrl url,
    Deserialize<TSuccess> deserializeSuccess,
    Deserialize<TError> deserializeError,
    IReadOnlyDictionary<string, string>? headers = null,
    CancellationToken cancellationToken = default
)
```

---

## PatchAsync

执行带请求体的 PATCH 请求。

```csharp
public static Task<Result<TSuccess, HttpError<TError>>> PatchAsync<TSuccess, TError>(
    this HttpClient httpClient,
    AbsoluteUrl url,
    HttpContent? requestBody,
    Deserialize<TSuccess> deserializeSuccess,
    Deserialize<TError> deserializeError,
    IReadOnlyDictionary<string, string>? headers = null,
    CancellationToken cancellationToken = default
)
```

---

## HeadAsync

执行 HEAD 请求（仅获取响应头）。

```csharp
public static Task<Result<TSuccess, HttpError<TError>>> HeadAsync<TSuccess, TError>(
    this HttpClient httpClient,
    AbsoluteUrl url,
    Deserialize<TSuccess> deserializeSuccess,
    Deserialize<TError> deserializeError,
    IReadOnlyDictionary<string, string>? headers = null,
    CancellationToken cancellationToken = default
)
```

---

## OptionsAsync

执行 OPTIONS 请求（获取允许的方法）。

```csharp
public static Task<Result<TSuccess, HttpError<TError>>> OptionsAsync<TSuccess, TError>(
    this HttpClient httpClient,
    AbsoluteUrl url,
    Deserialize<TSuccess> deserializeSuccess,
    Deserialize<TError> deserializeError,
    IReadOnlyDictionary<string, string>? headers = null,
    CancellationToken cancellationToken = default
)
```

---

## DownloadFileAsync

从指定 URL 下载文件到流。

```csharp
public static Task<Result<Unit, HttpError<TError>>> DownloadFileAsync<TError>(
    this HttpClient httpClient,
    AbsoluteUrl url,
    Stream destinationStream,
    Deserialize<TError> deserializeError,
    IReadOnlyDictionary<string, string>? headers = null,
    CancellationToken cancellationToken = default
)
```

### 示例

```csharp
using var fileStream = File.Create("downloaded-file.pdf");
var result = await httpClient.DownloadFileAsync(
    url: "https://api.example.com/files/report.pdf".ToAbsoluteUrl(),
    destinationStream: fileStream,
    deserializeError: DeserializeJson<ApiError>
);

if (result.IsOk)
{
    Console.WriteLine("文件下载成功");
}
```

---

## UploadFileAsync

上传文件到指定 URL。

```csharp
public static Task<Result<TSuccess, HttpError<TError>>> UploadFileAsync<TSuccess, TError>(
    this HttpClient httpClient,
    AbsoluteUrl url,
    HttpContent requestBody,
    Deserialize<TSuccess> deserializeSuccess,
    Deserialize<TError> deserializeError,
    IReadOnlyDictionary<string, string>? headers = null,
    CancellationToken cancellationToken = default
)
```

### 示例

```csharp
using var fileStream = File.OpenRead("document.pdf");
var content = new ProgressReportingHttpContent(
    content: fileStream,
    progress: (current, total) => Console.WriteLine($"进度: {current}/{total}")
);

var result = await httpClient.UploadFileAsync(
    url: "https://api.example.com/upload".ToAbsoluteUrl(),
    requestBody: content,
    deserializeSuccess: DeserializeJson<UploadResponse>,
    deserializeError: DeserializeJson<ApiError>
);
```

---

## 请求工厂方法

这些方法创建可多次调用的可复用请求委托。

### CreateGet

创建 GET 请求委托。

```csharp
public static GetAsync<TSuccess, TError, TParam> CreateGet<TSuccess, TError, TParam>(
    AbsoluteUrl url,
    BuildRequest<TParam> buildRequest,
    Deserialize<TSuccess> deserializeSuccess,
    Deserialize<TError> deserializeError
)
```

### CreatePost

创建 POST 请求委托。

### CreatePut

创建 PUT 请求委托。

### CreateDelete

创建 DELETE 请求委托。

### CreatePatch

创建 PATCH 请求委托。

### 工厂示例

```csharp
// 定义一次请求工厂
var getUser = HttpClientExtensions.CreateGet<User, ApiError, int>(
    url: "https://api.example.com".ToAbsoluteUrl(),
    buildRequest: userId => new HttpRequestParts(
        RelativeUrl: $"/users/{userId}".ToRelativeUrl(),
        Body: null,
        Headers: null
    ),
    deserializeSuccess: DeserializeJson<User>,
    deserializeError: DeserializeJson<ApiError>
);

// 多次使用
var user1 = await getUser(httpClient, 1, CancellationToken.None);
var user2 = await getUser(httpClient, 2, CancellationToken.None);
```
