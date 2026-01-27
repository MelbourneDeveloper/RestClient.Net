---
layout: layouts/api.njk
title: GetAsync Method
description: Make a type-safe GET request that returns Result instead of throwing exceptions.
keywords: GetAsync, HTTP GET, RestClient.Net, type-safe HTTP
eleventyNavigation:
  key: GetAsync
  parent: HttpClient Extensions
  order: 1
permalink: /api/getasync/
---

Make a type-safe GET request.

## Namespace

`RestClient.Net`

## Containing Type

[HttpClientExtensions](/api/httpclient-extensions/)

## Signature

```csharp
public static async Task<Result<TSuccess, HttpError<TError>>> GetAsync<TSuccess, TError>(
    this HttpClient httpClient,
    AbsoluteUrl url,
    Func<HttpResponseMessage, CancellationToken, Task<TSuccess>> deserializeSuccess,
    Func<HttpResponseMessage, CancellationToken, Task<TError>> deserializeError,
    IReadOnlyDictionary<string, string>? headers = null,
    CancellationToken cancellationToken = default
)
```

## Parameters

| Parameter | Type | Description |
|-----------|------|-------------|
| `url` | `AbsoluteUrl` | The request URL (use `.ToAbsoluteUrl()` extension) |
| `deserializeSuccess` | [`Func<HttpResponseMessage, CancellationToken, Task<TSuccess>>`](https://learn.microsoft.com/en-us/dotnet/api/system.func-2) | Function to deserialize success response |
| `deserializeError` | [`Func<HttpResponseMessage, CancellationToken, Task<TError>>`](https://learn.microsoft.com/en-us/dotnet/api/system.func-2) | Function to deserialize error response |
| `headers` | [`IReadOnlyDictionary<string, string>?`](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ireadonlydictionary-2) | Optional request headers |
| `cancellationToken` | [`CancellationToken`](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken) | Optional cancellation token |

## Returns

[`Task<Result<TSuccess, HttpError<TError>>>`](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1) - A discriminated union that is either:
- [`Ok<TSuccess>`](/api/ok/) - Success with deserialized data
- [`Error<HttpError<TError>>`](/api/error/) - Error with [ResponseError](/api/responseerror/) or [ExceptionError](/api/exceptionerror/)

## Example

```csharp
var result = await httpClient.GetAsync(
    url: "https://api.example.com/users/1".ToAbsoluteUrl(),
    deserializeSuccess: DeserializeUser,
    deserializeError: DeserializeApiError
);

var output = result switch
{
    OkUser(var user) => $"Found: {user.Name}",
    ErrorUser(ResponseErrorUser(var err, var status, _)) => $"API Error {status}: {err.Message}",
    ErrorUser(ExceptionErrorUser(var ex)) => $"Exception: {ex.Message}",
};
```

## See Also

- [HttpClientExtensions](/api/httpclient-extensions/) - All extension methods
- [Result&lt;TSuccess, TError&gt;](/api/result/) - The return type
- [Serialization](/api/serialization/) - Deserializer examples
