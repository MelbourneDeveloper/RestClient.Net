---
layout: layouts/docs.njk
title: IHttpClientFactory Extensions
eleventyNavigation:
  key: IHttpClientFactory Extensions
  parent: API Reference
  order: 2
---

# IHttpClientFactory Extensions

Extension methods for `System.Net.Http.IHttpClientFactory` that provide the same type-safe HTTP operations as the HttpClient extensions, with built-in client creation and connection pooling.

**Namespace:** `RestClient.Net`

## Overview

These extension methods work with `IHttpClientFactory` from Microsoft.Extensions.Http, enabling proper HTTP connection management in ASP.NET Core applications.

## SendAsync

The core method for sending HTTP requests with full control over all parameters.

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

### Parameters

| Parameter | Type | Description |
|-----------|------|-------------|
| `httpClientFactory` | `IHttpClientFactory` | The HTTP client factory instance |
| `clientName` | `string` | The name of the configured HTTP client |
| `url` | `AbsoluteUrl` | The absolute URL for the request |
| `httpMethod` | `HttpMethod` | The HTTP method (GET, POST, PUT, etc.) |
| `deserializeSuccess` | `Deserialize<TSuccess>` | Async function to deserialize successful responses |
| `deserializeError` | `Deserialize<TError>` | Async function to deserialize error responses |
| `requestBody` | `HttpContent?` | Optional request body content |
| `headers` | `IReadOnlyDictionary<string, string>?` | Optional request headers |
| `httpOperation` | `HttpAction?` | Optional custom HTTP operation handler |
| `cancellationToken` | `CancellationToken` | Cancellation token |

---

## GetAsync

Performs a GET request using a named HTTP client.

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

### Example

```csharp
// In Program.cs - configure the named client
builder.Services.AddHttpClient("api", client =>
{
    client.BaseAddress = new Uri("https://api.example.com");
    client.DefaultRequestHeaders.Add("Accept", "application/json");
});

// In your service - use the factory
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

Performs a POST request with a request body.

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

Performs a PUT request with a request body.

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

Performs a DELETE request.

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

Performs a PATCH request with a request body.

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

Downloads a file from the specified URL to a stream.

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

Uploads a file with progress reporting.

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

## Request Factory Methods

These static methods create reusable request delegates.

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

### CreatePut

```csharp
public static PutAsync<TSuccess, TError, TParam> CreatePut<TSuccess, TError, TParam>(
    AbsoluteUrl url,
    BuildRequest<TParam> buildRequest,
    Deserialize<TSuccess> deserializeSuccess,
    Deserialize<TError> deserializeError
)
```

### CreateDelete

```csharp
public static DeleteAsync<TSuccess, TError, TParam> CreateDelete<TSuccess, TError, TParam>(
    AbsoluteUrl url,
    BuildRequest<TParam> buildRequest,
    Deserialize<TSuccess> deserializeSuccess,
    Deserialize<TError> deserializeError
)
```

### CreatePatch

```csharp
public static PatchAsync<TSuccess, TError, TParam> CreatePatch<TSuccess, TError, TParam>(
    AbsoluteUrl url,
    BuildRequest<TParam> buildRequest,
    Deserialize<TSuccess> deserializeSuccess,
    Deserialize<TError> deserializeError
)
```

### CreateHead

```csharp
public static HeadAsync<TSuccess, TError, TParam> CreateHead<TSuccess, TError, TParam>(
    AbsoluteUrl url,
    BuildRequest<TParam> buildRequest,
    Deserialize<TSuccess> deserializeSuccess,
    Deserialize<TError> deserializeError
)
```

### CreateOptions

```csharp
public static OptionsAsync<TSuccess, TError, TParam> CreateOptions<TSuccess, TError, TParam>(
    AbsoluteUrl url,
    BuildRequest<TParam> buildRequest,
    Deserialize<TSuccess> deserializeSuccess,
    Deserialize<TError> deserializeError
)
```

---

## Configuration Example

```csharp
// Program.cs
var builder = WebApplication.CreateBuilder(args);

// Configure named HTTP clients
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
.AddPolicyHandler(GetRetryPolicy()); // Add Polly policies

// Register services that use the factory
builder.Services.AddScoped<IGitHubService, GitHubService>();
```

## Best Practices

1. **Use named clients** - Configure different clients for different APIs
2. **Configure base addresses** - Set `BaseAddress` in the client configuration
3. **Add default headers** - Configure common headers like Accept, Authorization
4. **Use Polly** - Add retry and circuit breaker policies for resilience
5. **Avoid capturing HttpClient** - Let the factory create new clients as needed
