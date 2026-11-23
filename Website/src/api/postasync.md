---
layout: layouts/api.njk
title: PostAsync Method
description: Make a type-safe POST request with a request body that returns Result instead of throwing exceptions.
keywords: PostAsync, HTTP POST, RestClient.Net, type-safe HTTP
eleventyNavigation:
  key: PostAsync
  parent: HttpClient Extensions
  order: 2
permalink: /api/postasync/
---

Make a type-safe POST request with a request body.

## Namespace

`RestClient.Net`

## Containing Type

[HttpClientExtensions](/api/httpclient-extensions/)

## Signature

```csharp
public static async Task<Result<TSuccess, HttpError<TError>>> PostAsync<TRequest, TSuccess, TError>(
    this HttpClient httpClient,
    AbsoluteUrl url,
    TRequest body,
    Func<TRequest, HttpContent> serializeRequest,
    Func<HttpResponseMessage, CancellationToken, Task<TSuccess>> deserializeSuccess,
    Func<HttpResponseMessage, CancellationToken, Task<TError>> deserializeError,
    IReadOnlyDictionary<string, string>? headers = null,
    CancellationToken cancellationToken = default
)
```

## Parameters

| Parameter | Type | Description |
|-----------|------|-------------|
| `url` | `AbsoluteUrl` | The request URL |
| `body` | `TRequest` | The request body object |
| `serializeRequest` | [`Func<TRequest, HttpContent>`](https://learn.microsoft.com/en-us/dotnet/api/system.func-2) | Function to serialize the request body |
| `deserializeSuccess` | [`Func<HttpResponseMessage, CancellationToken, Task<TSuccess>>`](https://learn.microsoft.com/en-us/dotnet/api/system.func-2) | Function to deserialize success response |
| `deserializeError` | [`Func<HttpResponseMessage, CancellationToken, Task<TError>>`](https://learn.microsoft.com/en-us/dotnet/api/system.func-2) | Function to deserialize error response |
| `headers` | [`IReadOnlyDictionary<string, string>?`](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ireadonlydictionary-2) | Optional request headers |
| `cancellationToken` | [`CancellationToken`](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken) | Optional cancellation token |

## Returns

[`Task<Result<TSuccess, HttpError<TError>>>`](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1) - A discriminated union that is either:
- [`Ok<TSuccess>`](/api/reference/outcome-result-2-ok-2/) - Success with deserialized data
- [`Error<HttpError<TError>>`](/api/reference/outcome-result-2-error-2/) - Error with [ResponseError](/api/reference/outcome-httperror-1-errorresponseerror/) or [ExceptionError](/api/reference/outcome-httperror-1-exceptionerror/)

## Example

```csharp
var newUser = new CreateUserRequest("John", "john@example.com");

var result = await httpClient.PostAsync(
    url: "https://api.example.com/users".ToAbsoluteUrl(),
    body: newUser,
    serializeRequest: SerializeJson,
    deserializeSuccess: DeserializeUser,
    deserializeError: DeserializeApiError
);
```

## See Also

- [HttpClientExtensions](/api/httpclient-extensions/) - All extension methods
- [Serialization](/api/serialization/) - Serializer examples
