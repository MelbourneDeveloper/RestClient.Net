---
layout: layouts/docs.njk
title: HttpClient Extensions
eleventyNavigation:
  key: HttpClient Extensions
  parent: API Reference
  order: 1
---

# HttpClient Extensions

Extension methods for `System.Net.Http.HttpClient` that return `Result<TSuccess, HttpError<TError>>` instead of throwing exceptions.

**Namespace:** `RestClient.Net`

## SendAsync

The core method for sending HTTP requests with full control over all parameters.

```csharp
public static async Task<Result<TSuccess, HttpError<TError>>> SendAsync<TSuccess, TError>(
    this HttpClient httpClient,
    AbsoluteUrl url,
    HttpMethod httpMethod,
    Deserialize<TSuccess> deserializeSuccess,
    Deserialize<TError> deserializeError,
    IReadOnlyDictionary<string, string>? headers = null,
    HttpContent? requestBody = null,
    HttpAction? httpOperation = null,
    CancellationToken cancellationToken = default
)
```

### Type Parameters

| Parameter | Description |
|-----------|-------------|
| `TSuccess` | The type to deserialize successful (2xx) responses into |
| `TError` | The type to deserialize error (non-2xx) responses into |

### Parameters

| Parameter | Type | Description |
|-----------|------|-------------|
| `httpClient` | `HttpClient` | The HTTP client instance |
| `url` | `AbsoluteUrl` | The absolute URL for the request |
| `httpMethod` | `HttpMethod` | The HTTP method (GET, POST, PUT, etc.) |
| `deserializeSuccess` | `Deserialize<TSuccess>` | Async function to deserialize successful responses |
| `deserializeError` | `Deserialize<TError>` | Async function to deserialize error responses |
| `headers` | `IReadOnlyDictionary<string, string>?` | Optional request headers |
| `requestBody` | `HttpContent?` | Optional request body content |
| `httpOperation` | `HttpAction?` | Optional custom HTTP operation handler |
| `cancellationToken` | `CancellationToken` | Cancellation token |

### Returns

`Task<Result<TSuccess, HttpError<TError>>>` - A Result that is either:
- `Ok<TSuccess>` containing the deserialized response for 2xx status codes
- `Error<HttpError<TError>>` containing either an `ErrorResponseError` or `ExceptionError`

---

## GetAsync

Performs a GET request.

```csharp
public static Task<Result<TSuccess, HttpError<TError>>> GetAsync<TSuccess, TError>(
    this HttpClient httpClient,
    AbsoluteUrl url,
    Deserialize<TSuccess> deserializeSuccess,
    Deserialize<TError> deserializeError,
    IReadOnlyDictionary<string, string>? headers = null,
    CancellationToken cancellationToken = default
)
```

### Example

```csharp
var result = await httpClient.GetAsync(
    url: "https://api.example.com/users/1".ToAbsoluteUrl(),
    deserializeSuccess: DeserializeJson<User>,
    deserializeError: DeserializeJson<ApiError>
);

var output = result switch
{
    Result<User, HttpError<ApiError>>.Ok<User, HttpError<ApiError>>(var user)
        => $"Found: {user.Name}",
    Result<User, HttpError<ApiError>>.Error<User, HttpError<ApiError>>(var error)
        => $"Error occurred",
};
```

---

## PostAsync

Performs a POST request with a request body.

```csharp
public static Task<Result<TSuccess, HttpError<TError>>> PostAsync<TSuccess, TError>(
    this HttpClient httpClient,
    AbsoluteUrl url,
    HttpContent? requestBody,
    Deserialize<TSuccess> deserializeSuccess,
    Deserialize<TError> deserializeError,
    IReadOnlyDictionary<string, string>? headers = null,
    CancellationToken cancellationToken = default
)
```

### Example

```csharp
var content = new StringContent(
    JsonSerializer.Serialize(new { Name = "John", Email = "john@example.com" }),
    Encoding.UTF8,
    "application/json"
);

var result = await httpClient.PostAsync(
    url: "https://api.example.com/users".ToAbsoluteUrl(),
    requestBody: content,
    deserializeSuccess: DeserializeJson<User>,
    deserializeError: DeserializeJson<ApiError>
);
```

---

## PutAsync

Performs a PUT request with a request body.

```csharp
public static Task<Result<TSuccess, HttpError<TError>>> PutAsync<TSuccess, TError>(
    this HttpClient httpClient,
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
    this HttpClient httpClient,
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
    this HttpClient httpClient,
    AbsoluteUrl url,
    HttpContent? requestBody,
    Deserialize<TSuccess> deserializeSuccess,
    Deserialize<TError> deserializeError,
    IReadOnlyDictionary<string, string>? headers = null,
    CancellationToken cancellationToken = default
)
```

---

## HeadAsync

Performs a HEAD request (retrieves headers only).

```csharp
public static Task<Result<TSuccess, HttpError<TError>>> HeadAsync<TSuccess, TError>(
    this HttpClient httpClient,
    AbsoluteUrl url,
    Deserialize<TSuccess> deserializeSuccess,
    Deserialize<TError> deserializeError,
    IReadOnlyDictionary<string, string>? headers = null,
    CancellationToken cancellationToken = default
)
```

---

## OptionsAsync

Performs an OPTIONS request (retrieves allowed methods).

```csharp
public static Task<Result<TSuccess, HttpError<TError>>> OptionsAsync<TSuccess, TError>(
    this HttpClient httpClient,
    AbsoluteUrl url,
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
    this HttpClient httpClient,
    AbsoluteUrl url,
    Stream destinationStream,
    Deserialize<TError> deserializeError,
    IReadOnlyDictionary<string, string>? headers = null,
    CancellationToken cancellationToken = default
)
```

### Example

```csharp
using var fileStream = File.Create("downloaded-file.pdf");
var result = await httpClient.DownloadFileAsync(
    url: "https://api.example.com/files/report.pdf".ToAbsoluteUrl(),
    destinationStream: fileStream,
    deserializeError: DeserializeJson<ApiError>
);

if (result.IsOk)
{
    Console.WriteLine("File downloaded successfully");
}
```

---

## UploadFileAsync

Uploads a file to the specified URL.

```csharp
public static Task<Result<TSuccess, HttpError<TError>>> UploadFileAsync<TSuccess, TError>(
    this HttpClient httpClient,
    AbsoluteUrl url,
    HttpContent requestBody,
    Deserialize<TSuccess> deserializeSuccess,
    Deserialize<TError> deserializeError,
    IReadOnlyDictionary<string, string>? headers = null,
    CancellationToken cancellationToken = default
)
```

### Example

```csharp
using var fileStream = File.OpenRead("document.pdf");
var content = new ProgressReportingHttpContent(
    content: fileStream,
    progress: (current, total) => Console.WriteLine($"Progress: {current}/{total}")
);

var result = await httpClient.UploadFileAsync(
    url: "https://api.example.com/upload".ToAbsoluteUrl(),
    requestBody: content,
    deserializeSuccess: DeserializeJson<UploadResponse>,
    deserializeError: DeserializeJson<ApiError>
);
```

---

## Request Factory Methods

These methods create reusable request delegates that can be invoked multiple times.

### CreateGet

Creates a GET request delegate.

```csharp
public static GetAsync<TSuccess, TError, TParam> CreateGet<TSuccess, TError, TParam>(
    AbsoluteUrl url,
    BuildRequest<TParam> buildRequest,
    Deserialize<TSuccess> deserializeSuccess,
    Deserialize<TError> deserializeError
)
```

### CreatePost

Creates a POST request delegate.

```csharp
public static PostAsync<TSuccess, TError, TParam> CreatePost<TSuccess, TError, TParam>(
    AbsoluteUrl url,
    BuildRequest<TParam> buildRequest,
    Deserialize<TSuccess> deserializeSuccess,
    Deserialize<TError> deserializeError
)
```

### CreatePut

Creates a PUT request delegate.

```csharp
public static PutAsync<TSuccess, TError, TParam> CreatePut<TSuccess, TError, TParam>(
    AbsoluteUrl url,
    BuildRequest<TParam> buildRequest,
    Deserialize<TSuccess> deserializeSuccess,
    Deserialize<TError> deserializeError
)
```

### CreateDelete

Creates a DELETE request delegate.

```csharp
public static DeleteAsync<TSuccess, TError, TParam> CreateDelete<TSuccess, TError, TParam>(
    AbsoluteUrl url,
    BuildRequest<TParam> buildRequest,
    Deserialize<TSuccess> deserializeSuccess,
    Deserialize<TError> deserializeError
)
```

### CreatePatch

Creates a PATCH request delegate.

```csharp
public static PatchAsync<TSuccess, TError, TParam> CreatePatch<TSuccess, TError, TParam>(
    AbsoluteUrl url,
    BuildRequest<TParam> buildRequest,
    Deserialize<TSuccess> deserializeSuccess,
    Deserialize<TError> deserializeError
)
```

### Factory Example

```csharp
// Define the request factory once
var getUser = HttpClientExtensions.CreateGet<User, ApiError, int>(
    url: "https://api.example.com".ToAbsoluteUrl(),
    buildRequest: userId => new HttpRequestParts(
        RelativeUrl: $"/users/{userId}".ToRelativeUrl(),
        Body: null,
        Headers: null
    ),
    deserializeSuccess: DeserializeJson<User>,
    deserializeError: DeserializeJson<ApiError>
);

// Use it multiple times
var user1 = await getUser(httpClient, 1, CancellationToken.None);
var user2 = await getUser(httpClient, 2, CancellationToken.None);
```
