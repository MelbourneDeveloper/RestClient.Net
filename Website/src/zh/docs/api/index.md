---
layout: layouts/docs.njk
title: API 参考
lang: zh
permalink: /zh/docs/api/
eleventyNavigation:
  key: API 参考
  parent: 文档
  order: 1
---

# API 参考

RestClient.Net 和 Outcome 库的完整参考文档。

## 核心包

### RestClient.Net

主库，提供带有可辨识联合返回类型的类型安全 HTTP 操作。

- [HttpClient 扩展](/zh/docs/api/httpclient-extensions/) - `HttpClient` 的扩展方法
- [IHttpClientFactory 扩展](/zh/docs/api/httpclientfactory-extensions/) - `IHttpClientFactory` 的扩展方法
- [委托](/zh/docs/api/delegates/) - 用于序列化/反序列化的函数委托
- [工具类](/zh/docs/api/utilities/) - 辅助类如 `ProgressReportingHttpContent`

### Outcome

提供用于错误处理的可辨识联合的函数式编程库。

- [Result 类型](/zh/docs/api/result/) - 核心 `Result<TSuccess, TFailure>` 类型
- [HttpError 类型](/zh/docs/api/httperror/) - HTTP 特定的错误表示
- [Result 扩展](/zh/docs/api/result-extensions/) - `Result` 的扩展方法
- [Unit 类型](/zh/docs/api/unit/) - 函数式编程中的 void 等价物

### 代码生成器

从 API 规范生成类型安全客户端的工具。

- [OpenAPI 生成器](/zh/docs/api/openapi-generator/) - 从 OpenAPI/Swagger 规范生成 C# 客户端
- [MCP 生成器](/zh/docs/api/mcp-generator/) - 从 OpenAPI 规范生成模型上下文协议服务器

## 快速入门

```csharp
using RestClient.Net;
using Outcome;

// 发起类型安全的 GET 请求
var result = await httpClient.GetAsync<User, ApiError>(
    url: "https://api.example.com/users/1".ToAbsoluteUrl(),
    deserializeSuccess: DeserializeJson<User>,
    deserializeError: DeserializeJson<ApiError>
);

// 使用模式匹配处理结果
var message = result switch
{
    Result<User, HttpError<ApiError>>.Ok<User, HttpError<ApiError>>(var user)
        => $"找到用户: {user.Name}",
    Result<User, HttpError<ApiError>>.Error<User, HttpError<ApiError>>(var error)
        => error switch
        {
            HttpError<ApiError>.ErrorResponseError(var body, var status, _)
                => $"API 错误 {status}: {body.Message}",
            HttpError<ApiError>.ExceptionError(var ex)
                => $"异常: {ex.Message}",
            _ => "未知错误"
        },
    _ => "意外情况"
};
```

## 设计理念

RestClient.Net 遵循铁路导向编程原则：

1. **预期错误不使用异常** - HTTP 错误作为值返回，而不是抛出
2. **完整的错误处理** - 模式匹配确保处理所有情况
3. **类型安全** - 泛型参数提供编译时类型检查
4. **可组合性** - Result 可以函数式地进行映射、绑定和组合
