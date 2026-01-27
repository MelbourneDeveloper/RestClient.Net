---
layout: layouts/docs.njk
title: 错误处理
description: 掌握 RestClient.Net Result 类型的函数式错误处理。学习 C# REST 调用的模式匹配、可辨识联合和穷尽性检查。
keywords: C# 错误处理, Result 类型, 可辨识联合, 模式匹配, 穷尽性检查
lang: zh
permalink: /zh/docs/error-handling/
eleventyNavigation:
  key: 错误处理
  order: 4
faq:
  - question: RestClient.Net 中的 Result 类型是什么？
    answer: Result 类型是一个可辨识联合，表示成功（Ok）或失败（Error）。它强制您在编译时处理所有可能的结果。
  - question: 什么是穷尽性检查？
    answer: Exhaustion 分析器验证您的 switch 表达式处理了所有可能的情况。遗漏的情况会导致编译错误，而不是运行时崩溃。
  - question: ResponseError 和 ExceptionError 有什么区别？
    answer: ResponseError 在服务器返回错误状态码（4xx、5xx）及响应体时发生。ExceptionError 在抛出异常时发生（网络错误、超时等）。
---

# 错误处理

RestClient.Net 使用可辨识联合和穷尽性检查来使错误处理安全且明确。本指南解释相关模式和最佳实践。

## 传统错误处理的问题

传统 HTTP 客户端会抛出异常：

```csharp
// 传统方式 - 危险！
try
{
    var response = await httpClient.GetAsync("https://api.example.com/user/1");
    response.EnsureSuccessStatusCode(); // 出错时抛出异常！
    var user = await response.Content.ReadFromJsonAsync<User>();
}
catch (HttpRequestException ex)
{
    // 网络错误
}
catch (JsonException ex)
{
    // 反序列化错误
}
catch (Exception ex)
{
    // 还可能发生什么？谁知道！
}
```

问题：
- 类型签名不会告诉你可能抛出什么
- 容易忘记 catch 块
- 正常路径和错误路径结构不同
- 遗漏异常类型会导致运行时崩溃

## RestClient.Net 方式

每个操作都返回 `Result<TSuccess, HttpError<TError>>`：

```csharp
// 类型告诉你所有可能发生的事情
Result<User, HttpError<ApiError>> result = await httpClient.GetAsync(...);

// 模式匹配确保处理所有情况
var output = result switch
{
    OkUser(var user) => user.Name,
    ErrorUser(ResponseErrorUser(var err, var status, _)) => $"API 错误 {status}",
    ErrorUser(ExceptionErrorUser(var ex)) => $"异常: {ex.Message}",
};
```

## 理解 Result 类型

### Result<TSuccess, TError>

基础可辨识联合：

```csharp
public abstract record Result<TSuccess, TError>
{
    public sealed record Ok<TSuccess, TError>(TSuccess Value) : Result<TSuccess, TError>;
    public sealed record Error<TSuccess, TError>(TError Value) : Result<TSuccess, TError>;
}
```

### HttpError<TError>

HTTP 操作的错误类型：

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

## 穷尽性检查

Exhaustion 分析器确保您处理所有情况：

```csharp
// 这段代码无法编译！
var message = result switch
{
    OkUser(var user) => "成功",
    ErrorUser(ResponseErrorUser(...)) => "响应错误",
    // 编译错误：缺少 ExceptionErrorUser
};
```

错误消息会精确告诉你缺少什么：

```
error EXHAUSTION001: Switch on Result is not exhaustive;
Matched: Ok<User, HttpError<ApiError>>, Error<User, HttpError<ApiError>> with ErrorResponseError
Missing: Error<User, HttpError<ApiError>> with ExceptionError
```

## 模式匹配模式

### 基本模式

明确处理所有三种情况：

```csharp
var message = result switch
{
    OkUser(var user) => $"欢迎，{user.Name}",
    ErrorUser(ResponseErrorUser(var err, var status, _)) =>
        $"服务器错误 {(int)status}: {err.Message}",
    ErrorUser(ExceptionErrorUser(var ex)) =>
        $"网络错误: {ex.Message}",
};
```

### 状态码匹配

对不同状态码进行不同处理：

```csharp
var message = result switch
{
    OkUser(var user) => $"找到: {user.Name}",

    ErrorUser(ResponseErrorUser(_, HttpStatusCode.NotFound, _)) =>
        "用户未找到",

    ErrorUser(ResponseErrorUser(_, HttpStatusCode.Unauthorized, _)) =>
        "请登录",

    ErrorUser(ResponseErrorUser(var err, var status, _)) =>
        $"服务器错误 {(int)status}: {err.Message}",

    ErrorUser(ExceptionErrorUser(var ex)) =>
        $"网络错误: {ex.Message}",
};
```

### 访问完整响应

`ResponseError` 包含完整的 `HttpResponseMessage`：

```csharp
var message = result switch
{
    OkUser(var user) => user.Name,

    ErrorUser(ResponseErrorUser(var err, var status, var response)) =>
    {
        // 访问响应头
        if (response.Headers.TryGetValues("X-Rate-Limit-Remaining", out var values))
        {
            Console.WriteLine($"剩余请求限制: {values.First()}");
        }
        return $"错误: {err.Message}";
    },

    ErrorUser(ExceptionErrorUser(var ex)) => $"异常: {ex.Message}",
};
```

### 异常类型匹配

处理特定异常类型：

```csharp
var message = result switch
{
    OkUser(var user) => user.Name,

    ErrorUser(ResponseErrorUser(var err, _, _)) => err.Message,

    ErrorUser(ExceptionErrorUser(TaskCanceledException ex)) when ex.CancellationToken.IsCancellationRequested =>
        "请求已取消",

    ErrorUser(ExceptionErrorUser(TaskCanceledException)) =>
        "请求超时",

    ErrorUser(ExceptionErrorUser(HttpRequestException)) =>
        "网络连接问题",

    ErrorUser(ExceptionErrorUser(var ex)) =>
        $"意外错误: {ex.Message}",
};
```

## 常见错误处理模式

### 出错时返回默认值

发生任何错误时返回默认值：

```csharp
User user = result switch
{
    OkUser(var u) => u,
    _ => User.Guest,
};
```

### 出错时抛出异常（逃生舱口）

当确实无法处理错误时：

```csharp
User user = result switch
{
    OkUser(var u) => u,
    ErrorUser(var error) => throw new InvalidOperationException($"获取用户失败: {error}"),
};
```

### 转换为可空类型

用于可选数据：

```csharp
User? user = result switch
{
    OkUser(var u) => u,
    _ => null,
};

if (user is not null)
{
    // 使用 user
}
```

### 记录错误日志

记录错误同时仍然处理它们：

```csharp
var message = result switch
{
    OkUser(var user) => user.Name,

    ErrorUser(ResponseErrorUser(var err, var status, _)) =>
    {
        logger.LogWarning("API 返回 {Status}: {Error}", status, err.Message);
        return "服务暂时不可用";
    },

    ErrorUser(ExceptionErrorUser(var ex)) =>
    {
        logger.LogError(ex, "发生网络错误");
        return "连接失败";
    },
};
```

## 链式操作

### Map - 转换成功值

在不触及错误的情况下转换成功值：

```csharp
// 将 Result<User, HttpError<ApiError>> 转换为 Result<string, HttpError<ApiError>>
var nameResult = userResult.Map(user => user.Name);
```

### FlatMap / Bind - 链接异步操作

链接每个都返回 Result 的操作：

```csharp
// 获取用户，然后获取其订单
var ordersResult = await userResult
    .FlatMapAsync(user => GetOrdersAsync(user.Id));
```

### 聚合多个结果

组合多个 Result：

```csharp
var userResult = await GetUserAsync(userId);
var ordersResult = await GetOrdersAsync(userId);
var settingsResult = await GetSettingsAsync(userId);

var combined = (userResult, ordersResult, settingsResult) switch
{
    (OkUser(var user), OkOrders(var orders), OkSettings(var settings)) =>
        new UserDashboard(user, orders, settings),

    (ErrorUser(var e), _, _) => throw new Exception($"用户错误: {e}"),
    (_, ErrorOrders(var e), _) => throw new Exception($"订单错误: {e}"),
    (_, _, ErrorSettings(var e)) => throw new Exception($"设置错误: {e}"),
};
```

## 错误响应模型

为您的 API 定义清晰的错误模型：

```csharp
// 简单错误
record ApiError(string Message, string Code);

// 带验证的详细错误
record ValidationError(
    string Message,
    Dictionary<string, string[]> Errors
);

// 标准问题详情（RFC 7807）
record ProblemDetails(
    string Type,
    string Title,
    int Status,
    string Detail,
    string Instance
);
```

## 最佳实践

1. **定义类型别名** 以获得更简洁的模式匹配
2. **始终处理所有情况** - 编译器会强制执行
3. **具体处理错误** - 不要只是捕获所有内容
4. **记录错误日志** 然后再返回用户友好的消息
5. **使用响应对象** 获取请求头和高级场景
6. **在适当时链接操作** 使用 Map 和 FlatMap

## 下一步

- [高级用法](/zh/docs/advanced-usage/) - 重试策略和中间件
- [Exhaustion 分析器](/zh/docs/exhaustion/) - 深入了解穷尽性检查
- [API 参考](/zh/api/result-types/) - 完整的 Result 类型文档
