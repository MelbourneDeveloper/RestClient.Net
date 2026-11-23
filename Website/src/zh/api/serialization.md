---
layout: layouts/docs.njk
title: 序列化
lang: zh
permalink: /zh/api/serialization/
eleventyNavigation:
  key: 序列化
  parent: API 参考
  order: 3
---

# 序列化

RestClient.Net 使用基于函数的序列化来实现最大灵活性。您可以完全控制请求和响应体的序列化方式。

## 请求序列化

\`serializeRequest\` 参数将请求对象转换为 \`HttpContent\`：

```csharp
// 使用 System.Text.Json（推荐）
serializeRequest: body => JsonContent.Create(body)

// 使用自定义选项
serializeRequest: body => JsonContent.Create(body, options: new JsonSerializerOptions
{
    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
})

// 自定义媒体类型
serializeRequest: body => new StringContent(
    JsonSerializer.Serialize(body),
    Encoding.UTF8,
    "application/vnd.api+json"
)
```

## 响应反序列化

\`deserializeSuccess\` 和 \`deserializeError\` 参数转换响应：

```csharp
// 使用 System.Text.Json
deserializeSuccess: async (response, ct) =>
    await response.Content.ReadFromJsonAsync<User>(ct)

// 使用自定义选项
deserializeSuccess: async (response, ct) =>
    await response.Content.ReadFromJsonAsync<User>(
        new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
        ct
    )

// 自定义反序列化
deserializeSuccess: async (response, ct) =>
{
    var json = await response.Content.ReadAsStringAsync(ct);
    return MyCustomParser.Parse<User>(json);
}
```

## 常见模式

### 可复用的反序列化器

定义可复用的反序列化器以避免重复：

```csharp
public static class Deserializers
{
    public static async Task<T> Json<T>(
        HttpResponseMessage response,
        CancellationToken ct) =>
        await response.Content.ReadFromJsonAsync<T>(ct)
            ?? throw new InvalidOperationException("空响应");

    public static async Task<ErrorResponse> Error(
        HttpResponseMessage response,
        CancellationToken ct) =>
        await response.Content.ReadFromJsonAsync<ErrorResponse>(ct)
            ?? new ErrorResponse("未知错误");
}

// 使用
var result = await httpClient.GetAsync(
    url: "https://api.example.com/users/123".ToAbsoluteUrl(),
    deserializeSuccess: Deserializers.Json<User>,
    deserializeError: Deserializers.Error
);
```

### 处理空响应

对于不返回响应体的端点（如 DELETE）：

```csharp
// 返回一个标记值表示成功
deserializeSuccess: async (response, ct) => Unit.Value

// 或返回状态码
deserializeSuccess: async (response, ct) => response.StatusCode
```

### 二进制数据

对于文件下载或二进制响应：

```csharp
deserializeSuccess: async (response, ct) =>
    await response.Content.ReadAsByteArrayAsync(ct)

// 或作为流
deserializeSuccess: async (response, ct) =>
    await response.Content.ReadAsStreamAsync(ct)
```

## 状态码处理

反序列化器接收完整的 \`HttpResponseMessage\`，因此您可以检查状态码：

```csharp
deserializeSuccess: async (response, ct) =>
{
    // 204 无内容
    if (response.StatusCode == HttpStatusCode.NoContent)
        return default(User);

    return await response.Content.ReadFromJsonAsync<User>(ct);
}
```

## 错误响应类型

您的错误类型可以是任何类型：

```csharp
// 简单字符串
deserializeError: async (response, ct) =>
    await response.Content.ReadAsStringAsync(ct)

// 结构化错误
public record ApiError(string Code, string Message, string[] Details);
deserializeError: async (response, ct) =>
    await response.Content.ReadFromJsonAsync<ApiError>(ct)

// Problem Details (RFC 7807)
deserializeError: async (response, ct) =>
    await response.Content.ReadFromJsonAsync<ProblemDetails>(ct)
```
