---
layout: layouts/docs.njk
title: IHttpClientFactory 扩展
lang: zh
permalink: /zh/docs/api/httpclientfactory-extensions/
eleventyNavigation:
  key: IHttpClientFactory 扩展
  parent: API 参考
  order: 2
---

# IHttpClientFactory 扩展

`System.Net.Http.IHttpClientFactory` 的扩展方法，提供与 HttpClient 扩展相同的类型安全 HTTP 操作，并内置客户端创建和连接池管理。

**命名空间：** `RestClient.Net`

## 概述

这些扩展方法与 Microsoft.Extensions.Http 中的 `IHttpClientFactory` 配合使用，在 ASP.NET Core 应用程序中实现正确的 HTTP 连接管理。

## SendAsync

发送 HTTP 请求的核心方法，可完全控制所有参数。

```csharp
public static async Task<Result<TSuccess, HttpError<TError>>> SendAsync<TSuccess, TError>(
    this IHttpClientFactory httpClientFactory,
    string clientName,
    AbsoluteUrl url,
    HttpMethod httpMethod,
    Deserialize<TSuccess> deserializeSuccess,
    Deserialize<TError> deserializeError,
    HttpContent? requestBody = null,
    IReadOnlyDictionary<string, string>? headers = null,
    HttpAction? httpOperation = null,
    CancellationToken cancellationToken = default
)
```

### 参数

| 参数 | 类型 | 描述 |
|-----------|------|-------------|
| `httpClientFactory` | `IHttpClientFactory` | HTTP 客户端工厂实例 |
| `clientName` | `string` | 已配置的 HTTP 客户端名称 |
| `url` | `AbsoluteUrl` | 请求的绝对 URL |
| `httpMethod` | `HttpMethod` | HTTP 方法（GET、POST、PUT 等） |
| `deserializeSuccess` | `Deserialize<TSuccess>` | 反序列化成功响应的异步函数 |
| `deserializeError` | `Deserialize<TError>` | 反序列化错误响应的异步函数 |
| `requestBody` | `HttpContent?` | 可选的请求体内容 |
| `headers` | `IReadOnlyDictionary<string, string>?` | 可选的请求头 |
| `httpOperation` | `HttpAction?` | 可选的自定义 HTTP 操作处理器 |
| `cancellationToken` | `CancellationToken` | 取消令牌 |

---

## GetAsync

使用命名 HTTP 客户端执行 GET 请求。

```csharp
public static Task<Result<TSuccess, HttpError<TError>>> GetAsync<TSuccess, TError>(
    this IHttpClientFactory httpClientFactory,
    string clientName,
    AbsoluteUrl url,
    Deserialize<TSuccess> deserializeSuccess,
    Deserialize<TError> deserializeError,
    IReadOnlyDictionary<string, string>? headers = null,
    CancellationToken cancellationToken = default
)
```

### 示例

```csharp
// 在 Program.cs 中配置命名客户端
builder.Services.AddHttpClient("api", client =>
{
    client.BaseAddress = new Uri("https://api.example.com");
    client.DefaultRequestHeaders.Add("Accept", "application/json");
});

// 在服务中使用工厂
public class UserService(IHttpClientFactory factory)
{
    public async Task<Result<User, HttpError<ApiError>>> GetUserAsync(int id)
    {
        return await factory.GetAsync(
            clientName: "api",
            url: $"https://api.example.com/users/{id}".ToAbsoluteUrl(),
            deserializeSuccess: DeserializeJson<User>,
            deserializeError: DeserializeJson<ApiError>
        );
    }
}
```

---

## PostAsync

执行带请求体的 POST 请求。

```csharp
public static Task<Result<TSuccess, HttpError<TError>>> PostAsync<TSuccess, TError>(
    this IHttpClientFactory httpClientFactory,
    string clientName,
    AbsoluteUrl url,
    HttpContent? requestBody,
    Deserialize<TSuccess> deserializeSuccess,
    Deserialize<TError> deserializeError,
    IReadOnlyDictionary<string, string>? headers = null,
    CancellationToken cancellationToken = default
)
```

---

## PutAsync

执行带请求体的 PUT 请求。

```csharp
public static Task<Result<TSuccess, HttpError<TError>>> PutAsync<TSuccess, TError>(
    this IHttpClientFactory httpClientFactory,
    string clientName,
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
    this IHttpClientFactory httpClientFactory,
    string clientName,
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
    this IHttpClientFactory httpClientFactory,
    string clientName,
    AbsoluteUrl url,
    HttpContent requestBody,
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
    this IHttpClientFactory httpClientFactory,
    string clientName,
    AbsoluteUrl url,
    Stream destinationStream,
    Deserialize<TError> deserializeError,
    IReadOnlyDictionary<string, string>? headers = null,
    CancellationToken cancellationToken = default
)
```

---

## UploadFileAsync

上传带进度报告的文件。

```csharp
public static Task<Result<TSuccess, HttpError<TError>>> UploadFileAsync<TSuccess, TError>(
    this IHttpClientFactory httpClientFactory,
    string clientName,
    AbsoluteUrl url,
    ProgressReportingHttpContent fileStream,
    Deserialize<TSuccess> deserializeSuccess,
    Deserialize<TError> deserializeError,
    IReadOnlyDictionary<string, string>? headers = null,
    CancellationToken cancellationToken = default
)
```

---

## 请求工厂方法

这些静态方法创建可复用的请求委托。

### CreateGet

```csharp
public static GetAsync<TSuccess, TError, TParam> CreateGet<TSuccess, TError, TParam>(
    AbsoluteUrl url,
    BuildRequest<TParam> buildRequest,
    Deserialize<TSuccess> deserializeSuccess,
    Deserialize<TError> deserializeError
)
```

### CreatePost

```csharp
public static PostAsync<TSuccess, TError, TParam> CreatePost<TSuccess, TError, TParam>(
    AbsoluteUrl url,
    BuildRequest<TParam> buildRequest,
    Deserialize<TSuccess> deserializeSuccess,
    Deserialize<TError> deserializeError
)
```

### CreatePut / CreateDelete / CreatePatch / CreateHead / CreateOptions

类似的工厂方法用于其他 HTTP 方法。

---

## 配置示例

```csharp
// Program.cs
var builder = WebApplication.CreateBuilder(args);

// 配置命名 HTTP 客户端
builder.Services.AddHttpClient("github", client =>
{
    client.BaseAddress = new Uri("https://api.github.com");
    client.DefaultRequestHeaders.Add("Accept", "application/vnd.github.v3+json");
    client.DefaultRequestHeaders.Add("User-Agent", "MyApp/1.0");
});

builder.Services.AddHttpClient("internal-api", client =>
{
    client.BaseAddress = new Uri("https://internal.example.com");
})
.AddPolicyHandler(GetRetryPolicy()); // 添加 Polly 策略

// 注册使用工厂的服务
builder.Services.AddScoped<IGitHubService, GitHubService>();
```

## 最佳实践

1. **使用命名客户端** - 为不同的 API 配置不同的客户端
2. **配置基础地址** - 在客户端配置中设置 `BaseAddress`
3. **添加默认请求头** - 配置常用请求头如 Accept、Authorization
4. **使用 Polly** - 添加重试和断路器策略以提高弹性
5. **避免捕获 HttpClient** - 让工厂根据需要创建新客户端
