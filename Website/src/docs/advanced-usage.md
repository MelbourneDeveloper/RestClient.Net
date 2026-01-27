---
layout: layouts/docs.njk
title: Advanced Usage
description: Advanced RestClient.Net patterns including retry policies, custom middleware, authentication handlers, and integration with dependency injection.
keywords: RestClient.Net advanced, retry policy, IHttpClientFactory, authentication handler, middleware
eleventyNavigation:
  key: Advanced Usage
  order: 5
faq:
  - question: How do I add retry logic to RestClient.Net?
    answer: Use Polly with IHttpClientFactory to add retry policies. Configure retry strategies for transient errors like network timeouts or 5xx responses.
  - question: How do I use RestClient.Net with dependency injection?
    answer: Register your services with IHttpClientFactory, configure named or typed clients, and inject them into your services.
  - question: How do I handle authentication with RestClient.Net?
    answer: Use DelegatingHandler to create an authentication handler that adds tokens to requests. Combine with IHttpClientFactory for token refresh.
---

# Advanced Usage

This guide covers advanced patterns for production use of RestClient.Net.

## Using IHttpClientFactory

Always use `IHttpClientFactory` in production to avoid socket exhaustion:

```csharp
// Program.cs
builder.Services.AddHttpClient("api", client =>
{
    client.BaseAddress = new Uri("https://api.example.com");
    client.DefaultRequestHeaders.Add("Accept", "application/json");
    client.Timeout = TimeSpan.FromSeconds(30);
});

// In your service
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

## Typed Clients

Create strongly-typed clients for better organization:

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

## Retry Policies with Polly

Add retry logic for transient failures:

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

### Custom Retry Strategy

For more control:

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
                    // Check for Retry-After header
                    if (response.Result?.Headers.RetryAfter?.Delta is { } delta)
                    {
                        return delta;
                    }
                    // Exponential backoff
                    return TimeSpan.FromSeconds(Math.Pow(2, retryAttempt));
                },
                onRetryAsync: (outcome, timespan, retryAttempt, context) =>
                {
                    Console.WriteLine($"Retry {retryAttempt} after {timespan}");
                    return Task.CompletedTask;
                }
            )
    );
```

## Authentication Handler

Create a delegating handler for authentication:

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

### Token Refresh Handler

Automatically refresh expired tokens:

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
            logger.LogInformation("Token expired, refreshing...");

            token = await tokenService.RefreshTokenAsync(cancellationToken);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            response = await base.SendAsync(request, cancellationToken);
        }

        return response;
    }
}
```

## Request/Response Logging

Log all HTTP traffic:

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
            "[{RequestId}] {StatusCode} in {ElapsedMs}ms",
            requestId,
            (int)response.StatusCode,
            stopwatch.ElapsedMilliseconds
        );

        return response;
    }
}
```

## Circuit Breaker

Prevent cascading failures:

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
                    Console.WriteLine($"Circuit broken for {duration}");
                },
                onReset: () =>
                {
                    Console.WriteLine("Circuit reset");
                }
            )
    );
```

## Custom Serialization

Use different serializers:

### System.Text.Json with Options

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
        ?? throw new InvalidOperationException($"Failed to deserialize {typeof(T).Name}");
}
```

### Newtonsoft.Json

```csharp
public static async Task<T> NewtonsoftJson<T>(HttpContent content, CancellationToken ct)
{
    var json = await content.ReadAsStringAsync(ct);
    return JsonConvert.DeserializeObject<T>(json)
        ?? throw new InvalidOperationException($"Failed to deserialize {typeof(T).Name}");
}
```

## Caching Responses

Cache successful responses:

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

## Request Correlation

Add correlation IDs for distributed tracing:

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

## Testing

### Mock HttpClient

```csharp
public class UserServiceTests
{
    [Fact]
    public async Task GetUser_ReturnsUser_WhenFound()
    {
        // Arrange
        var handler = new MockHttpMessageHandler(request =>
            new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = JsonContent.Create(new User("1", "John"))
            });

        var client = new HttpClient(handler)
        {
            BaseAddress = new Uri("https://api.example.com")
        };

        var service = new UserService(client);

        // Act
        var result = await service.GetUserAsync("1");

        // Assert
        Assert.IsType<OkUser>(result);
    }
}
```

## Best Practices Summary

1. **Always use IHttpClientFactory** in production
2. **Configure timeouts** explicitly
3. **Add retry policies** for transient errors
4. **Use circuit breakers** to prevent cascading failures
5. **Implement authentication handlers** for token management
6. **Log requests and responses** for debugging
7. **Add correlation IDs** for distributed tracing
8. **Cache expensive operations** when appropriate
9. **Test with mock handlers** for reliability

## Next Steps

- [OpenAPI Generator](/docs/openapi/) - Generate clients automatically
- [MCP Server](/docs/mcp/) - Claude Code integration
- [API Reference](/api/) - Complete documentation
