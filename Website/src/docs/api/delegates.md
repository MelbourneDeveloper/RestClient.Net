---
layout: layouts/docs.njk
title: Delegates
eleventyNavigation:
  key: Delegates
  parent: API Reference
  order: 3
---

# Delegates

RestClient.Net uses delegate types to define functions for serialization, deserialization, and request building.

**Namespace:** `RestClient.Net`

## HttpRequestParts

A record struct that contains all parts of an HTTP request.

```csharp
public readonly record struct HttpRequestParts(
    RelativeUrl RelativeUrl,
    HttpContent? Body,
    IReadOnlyDictionary<string, string>? Headers
);
```

### Properties

| Property | Type | Description |
|----------|------|-------------|
| `RelativeUrl` | `RelativeUrl` | The relative URL path for the request |
| `Body` | `HttpContent?` | Optional HTTP content for the request body |
| `Headers` | `IReadOnlyDictionary<string, string>?` | Optional headers to include |

### Example

```csharp
var parts = new HttpRequestParts(
    RelativeUrl: "/users/123".ToRelativeUrl(),
    Body: new StringContent("{\"name\":\"John\"}", Encoding.UTF8, "application/json"),
    Headers: new Dictionary<string, string> { ["X-Custom-Header"] = "value" }
);
```

---

## BuildRequest&lt;TParam&gt;

A delegate that builds HTTP request parts from a typed parameter.

```csharp
public delegate HttpRequestParts BuildRequest<TParam>(TParam argument);
```

### Type Parameters

| Parameter | Description |
|-----------|-------------|
| `TParam` | The type of parameter used to build the request |

### Example

```csharp
BuildRequest<int> buildUserRequest = userId => new HttpRequestParts(
    RelativeUrl: $"/users/{userId}".ToRelativeUrl(),
    Body: null,
    Headers: null
);

var parts = buildUserRequest(42);
// parts.RelativeUrl == "/users/42"
```

---

## Deserialize&lt;T&gt;

A delegate for asynchronously deserializing an HTTP response into a typed object.

```csharp
public delegate Task<T> Deserialize<T>(
    HttpResponseMessage httpResponseMessage,
    CancellationToken cancellationToken
);
```

### Type Parameters

| Parameter | Description |
|-----------|-------------|
| `T` | The type to deserialize the response content into |

### Parameters

| Parameter | Type | Description |
|-----------|------|-------------|
| `httpResponseMessage` | `HttpResponseMessage` | The full HTTP response message |
| `cancellationToken` | `CancellationToken` | Token to cancel the operation |

### Example

```csharp
// JSON deserializer
Deserialize<User> deserializeUser = async (response, ct) =>
{
    var stream = await response.Content.ReadAsStreamAsync(ct);
    return await JsonSerializer.DeserializeAsync<User>(stream, cancellationToken: ct)
        ?? throw new InvalidOperationException("Deserialization returned null");
};

// String deserializer
Deserialize<string> deserializeString = async (response, ct) =>
    await response.Content.ReadAsStringAsync(ct);

// Header-based deserializer
Deserialize<(string Content, string? ETag)> deserializeWithETag = async (response, ct) =>
{
    var content = await response.Content.ReadAsStringAsync(ct);
    var etag = response.Headers.ETag?.Tag;
    return (content, etag);
};
```

---

## Serialize&lt;T&gt;

A delegate for asynchronously serializing a typed object into HTTP content.

```csharp
public delegate Task<HttpContent> Serialize<T>(
    T body,
    CancellationToken cancellationToken
);
```

### Type Parameters

| Parameter | Description |
|-----------|-------------|
| `T` | The type of object to serialize |

### Example

```csharp
Serialize<User> serializeUser = (user, ct) =>
{
    var json = JsonSerializer.Serialize(user);
    return Task.FromResult<HttpContent>(
        new StringContent(json, Encoding.UTF8, "application/json")
    );
};
```

---

## GetRelativeUrl&lt;TParam&gt;

A delegate that constructs a relative URL from a typed parameter.

```csharp
public delegate RelativeUrl GetRelativeUrl<TParam>(TParam argument);
```

### Example

```csharp
GetRelativeUrl<(string collection, int id)> getResourceUrl =
    param => $"/{param.collection}/{param.id}".ToRelativeUrl();

var url = getResourceUrl(("users", 42));
// url == "/users/42"
```

---

## HttpAction

A delegate that performs the core HTTP send operation.

```csharp
public delegate Task<HttpResponseMessage> HttpAction(
    HttpClient httpClient,
    HttpRequestMessage requestMessage,
    CancellationToken cancellationToken
);
```

### Purpose

This delegate allows you to override the default HTTP send behavior. Use cases include:
- Adding custom logging
- Implementing retry logic
- Modifying requests before sending
- Capturing metrics

### Example

```csharp
HttpAction customAction = async (client, request, ct) =>
{
    // Log the request
    Console.WriteLine($"Sending {request.Method} to {request.RequestUri}");

    var stopwatch = Stopwatch.StartNew();
    var response = await client.SendAsync(request, ct);
    stopwatch.Stop();

    // Log the response time
    Console.WriteLine($"Response: {response.StatusCode} in {stopwatch.ElapsedMilliseconds}ms");

    return response;
};

var result = await httpClient.SendAsync(
    url: "https://api.example.com/users".ToAbsoluteUrl(),
    httpMethod: HttpMethod.Get,
    deserializeSuccess: DeserializeJson<User[]>,
    deserializeError: DeserializeJson<ApiError>,
    headers: null,
    requestBody: null,
    httpOperation: customAction
);
```

---

## Request Delegates

These delegates represent pre-configured HTTP request functions.

### GetAsync&lt;TSuccess, TError, TParam&gt;

```csharp
public delegate Task<Result<TSuccess, HttpError<TError>>> GetAsync<TSuccess, TError, TParam>(
    HttpClient httpClient,
    TParam parameters,
    CancellationToken cancellationToken = default
);
```

### PostAsync&lt;TSuccess, TError, TRequest&gt;

```csharp
public delegate Task<Result<TSuccess, HttpError<TError>>> PostAsync<TSuccess, TError, TRequest>(
    HttpClient httpClient,
    TRequest requestBody,
    CancellationToken cancellationToken = default
);
```

### PutAsync&lt;TSuccess, TError, TRequest&gt;

```csharp
public delegate Task<Result<TSuccess, HttpError<TError>>> PutAsync<TSuccess, TError, TRequest>(
    HttpClient httpClient,
    TRequest requestBody,
    CancellationToken cancellationToken = default
);
```

### DeleteAsync&lt;TSuccess, TError, TParam&gt;

```csharp
public delegate Task<Result<TSuccess, HttpError<TError>>> DeleteAsync<TSuccess, TError, TParam>(
    HttpClient httpClient,
    TParam parameters,
    CancellationToken cancellationToken = default
);
```

### PatchAsync&lt;TSuccess, TError, TRequest&gt;

```csharp
public delegate Task<Result<TSuccess, HttpError<TError>>> PatchAsync<TSuccess, TError, TRequest>(
    HttpClient httpClient,
    TRequest requestBody,
    CancellationToken cancellationToken = default
);
```

### HeadAsync&lt;TSuccess, TError, TParam&gt;

```csharp
public delegate Task<Result<TSuccess, HttpError<TError>>> HeadAsync<TSuccess, TError, TParam>(
    HttpClient httpClient,
    TParam parameters,
    CancellationToken cancellationToken = default
);
```

### OptionsAsync&lt;TSuccess, TError, TParam&gt;

```csharp
public delegate Task<Result<TSuccess, HttpError<TError>>> OptionsAsync<TSuccess, TError, TParam>(
    HttpClient httpClient,
    TParam parameters,
    CancellationToken cancellationToken = default
);
```

---

## Usage Pattern

```csharp
// Define a typed API client using delegates
public class UserApiClient
{
    private readonly GetAsync<User, ApiError, int> _getUser;
    private readonly PostAsync<User, ApiError, CreateUserRequest> _createUser;
    private readonly DeleteAsync<Unit, ApiError, int> _deleteUser;

    public UserApiClient()
    {
        var baseUrl = "https://api.example.com".ToAbsoluteUrl();

        _getUser = HttpClientExtensions.CreateGet<User, ApiError, int>(
            url: baseUrl,
            buildRequest: id => new HttpRequestParts($"/users/{id}".ToRelativeUrl(), null, null),
            deserializeSuccess: DeserializeJson<User>,
            deserializeError: DeserializeJson<ApiError>
        );

        _createUser = HttpClientExtensions.CreatePost<User, ApiError, CreateUserRequest>(
            url: baseUrl,
            buildRequest: req => new HttpRequestParts(
                "/users".ToRelativeUrl(),
                new StringContent(JsonSerializer.Serialize(req), Encoding.UTF8, "application/json"),
                null
            ),
            deserializeSuccess: DeserializeJson<User>,
            deserializeError: DeserializeJson<ApiError>
        );
    }

    public Task<Result<User, HttpError<ApiError>>> GetUserAsync(
        HttpClient client, int id, CancellationToken ct = default)
        => _getUser(client, id, ct);

    public Task<Result<User, HttpError<ApiError>>> CreateUserAsync(
        HttpClient client, CreateUserRequest request, CancellationToken ct = default)
        => _createUser(client, request, ct);
}
```
