---
layout: layouts/docs.njk
title: 高级用法
description: RestClient.Net 高级模式，包括重试策略、自定义中间件、身份验证处理器和依赖注入集成。
keywords: RestClient.Net 高级, 重试策略, IHttpClientFactory, 身份验证处理器, 中间件
lang: zh
permalink: /zh/docs/advanced-usage/
eleventyNavigation:
  key: 高级用法
  order: 5
faq:
  - question: 如何为 RestClient.Net 添加重试逻辑？
    answer: 结合 IHttpClientFactory 使用 Polly 添加重试策略。为网络超时或 5xx 响应等瞬态错误配置重试策略。
  - question: 如何在依赖注入中使用 RestClient.Net？
    answer: 使用 IHttpClientFactory 注册服务，配置命名或类型化客户端，然后将它们注入到服务中。
  - question: 如何使用 RestClient.Net 处理身份验证？
    answer: 使用 DelegatingHandler 创建身份验证处理器，为请求添加令牌。结合 IHttpClientFactory 实现令牌刷新。
---

# 高级用法

本指南涵盖 RestClient.Net 生产环境使用的高级模式。

## 使用 IHttpClientFactory

在生产环境中始终使用 `IHttpClientFactory` 以避免套接字耗尽：

```csharp
// Program.cs
builder.Services.AddHttpClient("api", client =>
{
    client.BaseAddress = new Uri("https://api.example.com");
    client.DefaultRequestHeaders.Add("Accept", "application/json");
    client.Timeout = TimeSpan.FromSeconds(30);
});

// 在服务中
public class UserService(IHttpClientFactory httpClientFactory)
{
    public async Task<Result<User, HttpError<ApiError>>> GetUserAsync(
        string userId,
        CancellationToken ct = default)
    {
        var client = httpClientFactory.CreateClient("api");

        return await client.GetAsync(
            url: $"/users/{userId}".ToAbsoluteUrl(),
            deserializeSuccess: Deserializers.Json<User>,
            deserializeError: Deserializers.Error,
            cancellationToken: ct
        );
    }
}
```

## 类型化客户端

创建强类型客户端以便更好地组织代码：

```csharp
// UserApiClient.cs
public class UserApiClient(HttpClient httpClient)
{
    public Task<Result<User, HttpError<ApiError>>> GetUserAsync(
        string userId,
        CancellationToken ct = default) =>
        httpClient.GetAsync(
            url: $"/users/{userId}".ToAbsoluteUrl(),
            deserializeSuccess: Deserializers.Json<User>,
            deserializeError: Deserializers.Error,
            cancellationToken: ct
        );

    public Task<Result<User, HttpError<ApiError>>> CreateUserAsync(
        CreateUserRequest request,
        CancellationToken ct = default) =>
        httpClient.PostAsync(
            url: "/users".ToAbsoluteUrl(),
            body: request,
            serializeRequest: body => JsonContent.Create(body),
            deserializeSuccess: Deserializers.Json<User>,
            deserializeError: Deserializers.Error,
            cancellationToken: ct
        );
}

// Program.cs
builder.Services.AddHttpClient<UserApiClient>(client =>
{
    client.BaseAddress = new Uri("https://api.example.com");
});
```

## 使用 Polly 的重试策略

为瞬态故障添加重试逻辑：

```csharp
using Microsoft.Extensions.Http.Resilience;

// Program.cs
builder.Services.AddHttpClient("api")
    .AddStandardResilienceHandler(options =>
    {
        options.Retry.MaxRetryAttempts = 3;
        options.Retry.Delay = TimeSpan.FromMilliseconds(500);
        options.Retry.UseJitter = true;
        options.Retry.ShouldHandle = args => ValueTask.FromResult(
            args.Outcome.Exception is not null ||
            args.Outcome.Result?.StatusCode >= HttpStatusCode.InternalServerError
        );
    });
```

### 自定义重试策略

需要更多控制时：

```csharp
builder.Services.AddHttpClient("api")
    .AddPolicyHandler(
        HttpPolicyExtensions
            .HandleTransientHttpError()
            .OrResult(msg => msg.StatusCode == HttpStatusCode.TooManyRequests)
            .WaitAndRetryAsync(
                retryCount: 3,
                sleepDurationProvider: (retryAttempt, response, context) =>
                {
                    // 检查 Retry-After 头
                    if (response.Result?.Headers.RetryAfter?.Delta is { } delta)
                    {
                        return delta;
                    }
                    // 指数退避
                    return TimeSpan.FromSeconds(Math.Pow(2, retryAttempt));
                },
                onRetryAsync: (outcome, timespan, retryAttempt, context) =>
                {
                    Console.WriteLine($"第 {retryAttempt} 次重试，等待 {timespan}");
                    return Task.CompletedTask;
                }
            )
    );
```

## 身份验证处理器

创建用于身份验证的委托处理器：

```csharp
public class AuthenticationHandler(ITokenService tokenService) : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        var token = await tokenService.GetAccessTokenAsync(cancellationToken);

        request.Headers.Authorization =
            new AuthenticationHeaderValue("Bearer", token);

        return await base.SendAsync(request, cancellationToken);
    }
}

// Program.cs
builder.Services.AddTransient<AuthenticationHandler>();
builder.Services.AddHttpClient("api")
    .AddHttpMessageHandler<AuthenticationHandler>();
```

### 令牌刷新处理器

自动刷新过期令牌：

```csharp
public class TokenRefreshHandler(
    ITokenService tokenService,
    ILogger<TokenRefreshHandler> logger) : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        var token = await tokenService.GetAccessTokenAsync(cancellationToken);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await base.SendAsync(request, cancellationToken);

        if (response.StatusCode == HttpStatusCode.Unauthorized)
        {
            logger.LogInformation("令牌已过期，正在刷新...");

            token = await tokenService.RefreshTokenAsync(cancellationToken);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            response = await base.SendAsync(request, cancellationToken);
        }

        return response;
    }
}
```

## 请求/响应日志

记录所有 HTTP 流量：

```csharp
public class LoggingHandler(ILogger<LoggingHandler> logger) : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        var requestId = Guid.NewGuid().ToString("N")[..8];

        logger.LogInformation(
            "[{RequestId}] {Method} {Uri}",
            requestId,
            request.Method,
            request.RequestUri
        );

        var stopwatch = Stopwatch.StartNew();
        var response = await base.SendAsync(request, cancellationToken);
        stopwatch.Stop();

        logger.LogInformation(
            "[{RequestId}] {StatusCode} 耗时 {ElapsedMs}ms",
            requestId,
            (int)response.StatusCode,
            stopwatch.ElapsedMilliseconds
        );

        return response;
    }
}
```

## 断路器

防止级联故障：

```csharp
builder.Services.AddHttpClient("api")
    .AddPolicyHandler(
        HttpPolicyExtensions
            .HandleTransientHttpError()
            .CircuitBreakerAsync(
                handledEventsAllowedBeforeBreaking: 5,
                durationOfBreak: TimeSpan.FromSeconds(30),
                onBreak: (result, duration) =>
                {
                    Console.WriteLine($"断路器打开，持续 {duration}");
                },
                onReset: () =>
                {
                    Console.WriteLine("断路器重置");
                }
            )
    );
```

## 自定义序列化

使用不同的序列化器：

### System.Text.Json 带选项

```csharp
public static class Deserializers
{
    private static readonly JsonSerializerOptions Options = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
    };

    public static async Task<T> Json<T>(HttpContent content, CancellationToken ct)
        where T : class =>
        await content.ReadFromJsonAsync<T>(Options, ct)
        ?? throw new InvalidOperationException($"反序列化 {typeof(T).Name} 失败");
}
```

### Newtonsoft.Json

```csharp
public static async Task<T> NewtonsoftJson<T>(HttpContent content, CancellationToken ct)
{
    var json = await content.ReadAsStringAsync(ct);
    return JsonConvert.DeserializeObject<T>(json)
        ?? throw new InvalidOperationException($"反序列化 {typeof(T).Name} 失败");
}
```

## 缓存响应

缓存成功的响应：

```csharp
public class CachingService(
    IMemoryCache cache,
    IHttpClientFactory httpClientFactory)
{
    public async Task<Result<User, HttpError<ApiError>>> GetUserAsync(
        string userId,
        CancellationToken ct = default)
    {
        var cacheKey = $"user:{userId}";

        if (cache.TryGetValue<User>(cacheKey, out var cached))
        {
            return new Result<User, HttpError<ApiError>>.Ok(cached);
        }

        var client = httpClientFactory.CreateClient("api");

        var result = await client.GetAsync(
            url: $"/users/{userId}".ToAbsoluteUrl(),
            deserializeSuccess: Deserializers.Json<User>,
            deserializeError: Deserializers.Error,
            cancellationToken: ct
        );

        if (result is Result<User, HttpError<ApiError>>.Ok(var user))
        {
            cache.Set(cacheKey, user, TimeSpan.FromMinutes(5));
        }

        return result;
    }
}
```

## 请求关联

为分布式追踪添加关联 ID：

```csharp
public class CorrelationHandler(IHttpContextAccessor httpContextAccessor) : DelegatingHandler
{
    protected override Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        var correlationId = httpContextAccessor.HttpContext?
            .Request.Headers["X-Correlation-ID"].FirstOrDefault()
            ?? Guid.NewGuid().ToString();

        request.Headers.Add("X-Correlation-ID", correlationId);

        return base.SendAsync(request, cancellationToken);
    }
}
```

## 测试

### 模拟 HttpClient

```csharp
public class UserServiceTests
{
    [Fact]
    public async Task GetUser_ReturnsUser_WhenFound()
    {
        // 准备
        var handler = new MockHttpMessageHandler(request =>
            new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = JsonContent.Create(new User("1", "张三"))
            });

        var client = new HttpClient(handler)
        {
            BaseAddress = new Uri("https://api.example.com")
        };

        var service = new UserService(client);

        // 执行
        var result = await service.GetUserAsync("1");

        // 断言
        Assert.IsType<OkUser>(result);
    }
}
```

## 最佳实践总结

1. **在生产环境中始终使用 IHttpClientFactory**
2. **明确配置超时**
3. **为瞬态错误添加重试策略**
4. **使用断路器防止级联故障**
5. **实现身份验证处理器** 进行令牌管理
6. **记录请求和响应日志** 用于调试
7. **添加关联 ID** 用于分布式追踪
8. **在适当时缓存昂贵的操作**
9. **使用模拟处理器测试** 以确保可靠性

## 下一步

- [OpenAPI 生成器](/zh/docs/openapi/) - 自动生成客户端
- [MCP 服务器](/zh/docs/mcp/) - Claude Code 集成
- [API 参考](/zh/api/) - 完整文档
