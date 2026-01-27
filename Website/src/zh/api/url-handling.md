---
layout: layouts/docs.njk
title: URL 处理
lang: zh
permalink: /zh/api/url-handling/
eleventyNavigation:
  key: URL 处理
  parent: API 参考
  order: 4
---

# URL 处理

RestClient.Net 使用 \`AbsoluteUrl\` 类型进行类型安全的 URL 处理。本页介绍 URL 模式和工具。

## AbsoluteUrl 类型

\`AbsoluteUrl\` 类型确保 URL 在编译时有效：

```csharp
using Urls;

// 将字符串转换为 AbsoluteUrl
AbsoluteUrl url = "https://api.example.com/users/1".ToAbsoluteUrl();

// 在请求中使用
var result = await httpClient.GetAsync(
    url: url,
    deserializeSuccess: Deserializers.User,
    deserializeError: Deserializers.Error
);
```

## 字符串扩展

\`ToAbsoluteUrl()\` 扩展方法验证并转换字符串：

```csharp
// 有效的 URL
var url1 = "https://api.example.com".ToAbsoluteUrl();
var url2 = "http://localhost:5000/api".ToAbsoluteUrl();

// 无效的 URL 在运行时抛出异常
var url3 = "not-a-url".ToAbsoluteUrl(); // 抛出异常！
var url4 = "/relative/path".ToAbsoluteUrl(); // 抛出异常！
```

## 路径参数

使用字符串插值处理路径参数：

```csharp
var userId = "123";
var url = \$"https://api.example.com/users/{userId}".ToAbsoluteUrl();

// 多个参数
var orderId = "456";
var url = \$"https://api.example.com/users/{userId}/orders/{orderId}".ToAbsoluteUrl();
```

### URL 编码路径参数

对用户输入进行编码以防止注入：

```csharp
var searchTerm = "foo/bar"; // 包含特殊字符

// 错误 - 可能破坏 URL
var badUrl = \$"https://api.example.com/search/{searchTerm}".ToAbsoluteUrl();

// 正确 - 正确编码
var goodUrl = \$"https://api.example.com/search/{Uri.EscapeDataString(searchTerm)}"
    .ToAbsoluteUrl();
```

## 查询参数

### 简单查询字符串

```csharp
var page = 1;
var limit = 20;
var url = \$"https://api.example.com/users?page={page}&limit={limit}".ToAbsoluteUrl();
```

### 带 URL 编码

```csharp
var query = "hello world";
var url = \$"https://api.example.com/search?q={Uri.EscapeDataString(query)}"
    .ToAbsoluteUrl();
// 结果：https://api.example.com/search?q=hello%20world
```

### 查询字符串构建器

对于复杂的查询字符串，使用辅助方法：

```csharp
public static class QueryString
{
    public static string Build(params (string key, string? value)[] parameters)
    {
        var pairs = parameters
            .Where(p => p.value is not null)
            .Select(p => \$"{Uri.EscapeDataString(p.key)}={Uri.EscapeDataString(p.value!)}");

        return string.Join("&", pairs);
    }
}

// 使用
var query = QueryString.Build(
    ("q", searchTerm),
    ("page", page.ToString()),
    ("limit", limit.ToString()),
    ("sort", sortBy) // 如果为 null 则省略
);

var url = \$"https://api.example.com/search?{query}".ToAbsoluteUrl();
```

### 使用 QueryHelpers

配合 Microsoft.AspNetCore.WebUtilities 使用：

```csharp
using Microsoft.AspNetCore.WebUtilities;

var parameters = new Dictionary<string, string?>
{
    ["q"] = searchTerm,
    ["page"] = page.ToString(),
    ["limit"] = limit.ToString(),
};

var url = QueryHelpers.AddQueryString("https://api.example.com/search", parameters)
    .ToAbsoluteUrl();
```

## 基础 URL + 相对路径

使用 \`IHttpClientFactory\` 时，将基础 URL 与相对路径组合：

```csharp
// Program.cs
builder.Services.AddHttpClient("api", client =>
{
    client.BaseAddress = new Uri("https://api.example.com/v1/");
});

// 在您的服务中
var client = httpClientFactory.CreateClient("api");

// 使用相对路径
var result = await client.GetAsync(
    url: "users/123".ToAbsoluteUrl(), // 解析为 https://api.example.com/v1/users/123
    deserializeSuccess: Deserializers.User,
    deserializeError: Deserializers.Error
);
```

### URL 解析规则

```csharp
// 基础：https://api.example.com/v1/

"users"          // -> https://api.example.com/v1/users
"users/123"      // -> https://api.example.com/v1/users/123
"/users"         // -> https://api.example.com/users（覆盖路径！）
"../users"       // -> https://api.example.com/users
```

## URL 构建器模式

为复杂场景创建流畅的 URL 构建器：

```csharp
public class UrlBuilder
{
    private readonly string _baseUrl;
    private readonly List<string> _segments = [];
    private readonly Dictionary<string, string> _query = [];

    public UrlBuilder(string baseUrl) => _baseUrl = baseUrl.TrimEnd('/');

    public UrlBuilder Segment(string segment)
    {
        _segments.Add(Uri.EscapeDataString(segment));
        return this;
    }

    public UrlBuilder Query(string key, string? value)
    {
        if (value is not null)
            _query[key] = value;
        return this;
    }

    public AbsoluteUrl Build()
    {
        var path = string.Join("/", _segments);
        var query = _query.Count > 0
            ? "?" + string.Join("&", _query.Select(kv =>
                \$"{Uri.EscapeDataString(kv.Key)}={Uri.EscapeDataString(kv.Value)}"))
            : "";

        return \$"{_baseUrl}/{path}{query}".ToAbsoluteUrl();
    }
}

// 使用
var url = new UrlBuilder("https://api.example.com")
    .Segment("users")
    .Segment(userId)
    .Segment("orders")
    .Query("status", "pending")
    .Query("page", "1")
    .Build();
```

## 基于环境的 URL

按环境配置 URL：

```csharp
// appsettings.json
{
  "ApiSettings": {
    "BaseUrl": "https://api.example.com"
  }
}

// appsettings.Development.json
{
  "ApiSettings": {
    "BaseUrl": "https://localhost:5001"
  }
}

// Program.cs
var apiSettings = builder.Configuration.GetSection("ApiSettings").Get<ApiSettings>()!;

builder.Services.AddHttpClient("api", client =>
{
    client.BaseAddress = new Uri(apiSettings.BaseUrl);
});
```

## URL 验证

### 编译时

\`AbsoluteUrl\` 类型提供类型安全：

```csharp
public Task<Result<User, HttpError<ApiError>>> GetUserAsync(
    AbsoluteUrl url, // 只接受有效的 AbsoluteUrl
    CancellationToken ct = default);

// 调用者必须将字符串转换为 AbsoluteUrl
var result = await GetUserAsync(
    "https://api.example.com/users/1".ToAbsoluteUrl(),
    ct
);
```

### 运行时验证

```csharp
public static bool TryCreateAbsoluteUrl(string input, out AbsoluteUrl? url)
{
    try
    {
        url = input.ToAbsoluteUrl();
        return true;
    }
    catch
    {
        url = null;
        return false;
    }
}
```

## 最佳实践

1. **始终编码用户输入** - 对路径和查询参数使用 \`Uri.EscapeDataString()\`
2. **使用基础 URL** - 在 \`IHttpClientFactory\` 中配置基础 URL
3. **安全构建复杂 URL** - 对多个参数使用 URL 构建器
4. **验证 URL** - \`ToAbsoluteUrl()\` 对无效 URL 抛出异常
5. **按环境配置** - 使用 appsettings 配置不同环境
