---
layout: layouts/docs.njk
title: 配置
lang: zh
permalink: /zh/api/configuration/
eleventyNavigation:
  key: 配置
  parent: API 参考
  order: 4
---

# 配置

RestClient.Net 与标准的 \`HttpClient\` 和 \`IHttpClientFactory\` 模式配合使用。无需特殊配置。

## 使用 IHttpClientFactory（推荐）

```csharp
// Program.cs
builder.Services.AddHttpClient("MyApi", client =>
{
    client.BaseAddress = new Uri("https://api.example.com/");
    client.DefaultRequestHeaders.Add("Accept", "application/json");
});

// 在您的服务中
public class UserService
{
    private readonly HttpClient _httpClient;

    public UserService(IHttpClientFactory factory)
    {
        _httpClient = factory.CreateClient("MyApi");
    }

    public async Task<Result<User, HttpError<ErrorResponse>>> GetUser(int id) =>
        await _httpClient.GetAsync(
            url: \$"users/{id}".ToAbsoluteUrl(),
            deserializeSuccess: Deserializers.Json<User>,
            deserializeError: Deserializers.Error
        );
}
```

## 身份验证

### Bearer Token

```csharp
builder.Services.AddHttpClient("MyApi", client =>
{
    client.BaseAddress = new Uri("https://api.example.com/");
})
.AddHttpMessageHandler<AuthHeaderHandler>();

public class AuthHeaderHandler : DelegatingHandler
{
    private readonly ITokenProvider _tokens;

    public AuthHeaderHandler(ITokenProvider tokens) => _tokens = tokens;

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken ct)
    {
        var token = await _tokens.GetTokenAsync(ct);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        return await base.SendAsync(request, ct);
    }
}
```

### API Key

```csharp
builder.Services.AddHttpClient("MyApi", client =>
{
    client.DefaultRequestHeaders.Add("X-API-Key", configuration["ApiKey"]);
});
```

## 使用 Polly 的重试策略

```csharp
builder.Services.AddHttpClient("MyApi")
    .AddTransientHttpErrorPolicy(p =>
        p.WaitAndRetryAsync(3, retryAttempt =>
            TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))));
```

## 超时设置

```csharp
builder.Services.AddHttpClient("MyApi", client =>
{
    client.Timeout = TimeSpan.FromSeconds(30);
});
```

## 基础地址与相对 URL

使用 \`BaseAddress\` 时，您可以使用相对 URL：

```csharp
// 当 BaseAddress = "https://api.example.com/"
var result = await httpClient.GetAsync(
    url: "users/123".ToAbsoluteUrl(),  // 解析为完整 URL
    deserializeSuccess: Deserializers.Json<User>,
    deserializeError: Deserializers.Error
);
```

## 每个请求的自定义头

可以通过 \`HttpRequestMessage\` 设置头信息以获得更多控制：

```csharp
var request = new HttpRequestMessage(HttpMethod.Get, "users/123");
request.Headers.Add("X-Request-ID", Guid.NewGuid().ToString());

var response = await httpClient.SendAsync(request);
```

## 依赖注入

注册您的类型化客户端：

```csharp
// Program.cs
builder.Services.AddScoped<IUserService, UserService>();

// UserService.cs
public interface IUserService
{
    Task<Result<User, HttpError<ErrorResponse>>> GetUser(int id);
    Task<Result<User, HttpError<ErrorResponse>>> CreateUser(CreateUserRequest request);
}
```
