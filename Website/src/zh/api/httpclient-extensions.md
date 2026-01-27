---
layout: layouts/docs.njk
title: HttpClient 扩展方法
lang: zh
eleventyNavigation:
  key: HttpClient 扩展方法
  parent: API 参考
  order: 1
permalink: /zh/api/httpclient-extensions/
---

# HttpClient 扩展方法

为 `HttpClient` 提供的扩展方法，返回 `Result<TSuccess, HttpError<TError>>` 而不是抛出异常。

## GetAsync

发起类型安全的 GET 请求。

```csharp
public static async Task<Result<TSuccess, HttpError<TError>>> GetAsync<TSuccess, TError>(
    this HttpClient httpClient,
    AbsoluteUrl url,
    Func<HttpContent, CancellationToken, Task<TSuccess>> deserializeSuccess,
    Func<HttpContent, CancellationToken, Task<TError>> deserializeError,
    CancellationToken cancellationToken = default
)
```

### 参数

| 参数 | 类型 | 描述 |
|------|------|------|
| `url` | `AbsoluteUrl` | 请求的 URL（使用 `.ToAbsoluteUrl()` 扩展方法） |
| `deserializeSuccess` | `Func<HttpContent, CancellationToken, Task<TSuccess>>` | 反序列化成功响应的函数 |
| `deserializeError` | `Func<HttpContent, CancellationToken, Task<TError>>` | 反序列化错误响应的函数 |
| `cancellationToken` | `CancellationToken` | 可选的取消令牌 |

### 返回值

`Task<Result<TSuccess, HttpError<TError>>>` - 一个可辨识联合，可能是：
- `Ok<TSuccess>` - 成功，包含反序列化的数据
- `Error<HttpError<TError>>` - 错误，包含 `ResponseError` 或 `ExceptionError`

### 示例

```csharp
var result = await httpClient.GetAsync(
    url: "https://api.example.com/users/1".ToAbsoluteUrl(),
    deserializeSuccess: DeserializeUser,
    deserializeError: DeserializeApiError
);

var output = result switch
{
    OkUser(var user) => $"找到: {user.Name}",
    ErrorUser(ResponseErrorUser(var err, var status, _)) => $"API 错误 {status}: {err.Message}",
    ErrorUser(ExceptionErrorUser(var ex)) => $"异常: {ex.Message}",
};
```

---

## PostAsync

发起带请求体的类型安全 POST 请求。

```csharp
public static async Task<Result<TSuccess, HttpError<TError>>> PostAsync<TRequest, TSuccess, TError>(
    this HttpClient httpClient,
    AbsoluteUrl url,
    TRequest body,
    Func<TRequest, HttpContent> serializeRequest,
    Func<HttpContent, CancellationToken, Task<TSuccess>> deserializeSuccess,
    Func<HttpContent, CancellationToken, Task<TError>> deserializeError,
    CancellationToken cancellationToken = default
)
```

### 参数

| 参数 | 类型 | 描述 |
|------|------|------|
| `url` | `AbsoluteUrl` | 请求的 URL |
| `body` | `TRequest` | 请求体对象 |
| `serializeRequest` | `Func<TRequest, HttpContent>` | 序列化请求体的函数 |
| `deserializeSuccess` | `Func<HttpContent, CancellationToken, Task<TSuccess>>` | 反序列化成功响应的函数 |
| `deserializeError` | `Func<HttpContent, CancellationToken, Task<TError>>` | 反序列化错误响应的函数 |
| `cancellationToken` | `CancellationToken` | 可选的取消令牌 |

### 示例

```csharp
var newUser = new CreateUserRequest("张三", "zhangsan@example.com");

var result = await httpClient.PostAsync(
    url: "https://api.example.com/users".ToAbsoluteUrl(),
    body: newUser,
    serializeRequest: SerializeJson,
    deserializeSuccess: DeserializeUser,
    deserializeError: DeserializeApiError
);
```

---

## PutAsync

发起类型安全的 PUT 请求。签名与 `PostAsync` 相同，用于更新现有资源。

---

## DeleteAsync

发起类型安全的 DELETE 请求。签名与 `GetAsync` 相同，用于删除资源。

---

## PatchAsync

发起类型安全的 PATCH 请求。签名与 `PostAsync` 相同，用于部分更新。

---

## 与 IHttpClientFactory 配合使用

RestClient.Net 与 `IHttpClientFactory` 无缝集成，实现正确的连接池管理：

```csharp
// 在 Program.cs 中
builder.Services.AddHttpClient("api", client =>
{
    client.BaseAddress = new Uri("https://api.example.com");
    client.DefaultRequestHeaders.Add("Accept", "application/json");
});

// 在您的服务中
public class UserService(IHttpClientFactory factory)
{
    public async Task<Result<User, HttpError<ApiError>>> GetUserAsync(int id)
    {
        var client = factory.CreateClient("api");
        return await client.GetAsync(
            url: $"/users/{id}".ToAbsoluteUrl(),
            deserializeSuccess: DeserializeUser,
            deserializeError: DeserializeApiError
        );
    }
}
```
