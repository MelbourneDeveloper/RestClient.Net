---
layout: layouts/docs.njk
title: Result 类型
lang: zh
permalink: /zh/api/result-types/
eleventyNavigation:
  key: Result 类型
  parent: API 参考
  order: 2
---

# Result 类型

RestClient.Net 使用可辨识联合类型来表示 HTTP 请求的所有可能结果。这使得配合 Exhaustion 分析器可以实现编译时穷尽性检查。

## Result<TSuccess, TError>

表示成功或错误的核心结果类型。

```csharp
public abstract record Result<TSuccess, TError>
{
    public sealed record Ok<TSuccess, TError>(TSuccess Value) : Result<TSuccess, TError>;
    public sealed record Error<TSuccess, TError>(TError Value) : Result<TSuccess, TError>;
}
```

### 模式匹配

```csharp
Result<User, HttpError<ApiError>> result = await GetUserAsync();

var output = result switch
{
    Result<User, HttpError<ApiError>>.Ok(var user) => user.Name,
    Result<User, HttpError<ApiError>>.Error(var error) => "错误",
};
```

---

## HttpError<TError>

表示 HTTP 级别的错误，区分响应错误和异常。

```csharp
public abstract record HttpError<TError>
{
    // 服务器返回错误响应（4xx、5xx）
    public sealed record ResponseError(
        TError Error,
        HttpStatusCode StatusCode,
        HttpResponseMessage Response
    ) : HttpError<TError>;

    // 发生异常（网络错误、超时等）
    public sealed record ExceptionError(
        Exception Exception
    ) : HttpError<TError>;
}
```

### ResponseError

当服务器返回非成功状态码时发生：

| 属性 | 类型 | 描述 |
|------|------|------|
| `Error` | `TError` | 反序列化的错误响应体 |
| `StatusCode` | `HttpStatusCode` | HTTP 状态码（如 404、500） |
| `Response` | `HttpResponseMessage` | 完整的 HTTP 响应，用于获取头信息等 |

### ExceptionError

在请求过程中抛出异常时发生：

| 属性 | 类型 | 描述 |
|------|------|------|
| `Exception` | `Exception` | 捕获的异常 |

常见异常：
- `HttpRequestException` - 网络连接问题
- `TaskCanceledException` - 请求超时或取消
- `JsonException` - 反序列化失败

---

## 类型别名

为了更简洁的模式匹配，在 `GlobalUsings.cs` 中定义类型别名：

```csharp
// 对于带有 ApiError 的 User 类型
global using OkUser = Outcome.Result<User, Outcome.HttpError<ApiError>>
    .Ok<User, Outcome.HttpError<ApiError>>;

global using ErrorUser = Outcome.Result<User, Outcome.HttpError<ApiError>>
    .Error<User, Outcome.HttpError<ApiError>>;

global using ResponseErrorUser = Outcome.HttpError<ApiError>.ErrorResponseError;

global using ExceptionErrorUser = Outcome.HttpError<ApiError>.ExceptionError;
```

### 使用类型别名

```csharp
var output = result switch
{
    OkUser(var user) => $"找到: {user.Name}",
    ErrorUser(ResponseErrorUser(var err, var status, _)) => $"错误 {status}",
    ErrorUser(ExceptionErrorUser(var ex)) => $"异常: {ex.Message}",
};
```

---

## 穷尽性检查

配合 [Exhaustion 分析器](/zh/docs/exhaustion/)，遗漏情况会导致编译错误：

```csharp
// 编译错误：EXHAUSTION001
// 遗漏情况：ExceptionErrorUser
var output = result switch
{
    OkUser(var user) => "成功",
    ErrorUser(ResponseErrorUser(...)) => "响应错误",
    // ExceptionErrorUser 未处理！
};
```

```csharp
// 编译通过：所有情况已处理
var output = result switch
{
    OkUser(var user) => "成功",
    ErrorUser(ResponseErrorUser(...)) => "响应错误",
    ErrorUser(ExceptionErrorUser(...)) => "异常",
};
```

---

## 映射结果

### Map Success

转换成功值同时保留错误：

```csharp
public static Result<TNew, TError> Map<TSuccess, TNew, TError>(
    this Result<TSuccess, TError> result,
    Func<TSuccess, TNew> mapper
) => result switch
{
    Result<TSuccess, TError>.Ok(var value) =>
        new Result<TNew, TError>.Ok(mapper(value)),
    Result<TSuccess, TError>.Error(var error) =>
        new Result<TNew, TError>.Error(error),
};

// 用法
var nameResult = userResult.Map(user => user.Name);
```

### FlatMap / Bind

链接返回 Result 的操作：

```csharp
public static async Task<Result<TNew, TError>> FlatMapAsync<TSuccess, TNew, TError>(
    this Result<TSuccess, TError> result,
    Func<TSuccess, Task<Result<TNew, TError>>> mapper
) => result switch
{
    Result<TSuccess, TError>.Ok(var value) => await mapper(value),
    Result<TSuccess, TError>.Error(var error) =>
        new Result<TNew, TError>.Error(error),
};

// 用法：获取用户，然后获取其订单
var ordersResult = await userResult.FlatMapAsync(
    user => GetOrdersAsync(user.Id)
);
```

---

## 常见模式

### 错误时使用默认值

```csharp
var user = result switch
{
    OkUser(var u) => u,
    _ => User.Empty,
};
```

### 错误时抛出异常（逃生出口）

```csharp
var user = result switch
{
    OkUser(var u) => u,
    ErrorUser(var e) => throw new InvalidOperationException($"失败: {e}"),
};
```

### 转换为可空类型

```csharp
User? user = result switch
{
    OkUser(var u) => u,
    _ => null,
};
```
