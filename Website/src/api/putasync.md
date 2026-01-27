---
layout: layouts/api.njk
title: PutAsync Method
description: Make a type-safe PUT request for full resource replacement.
keywords: PutAsync, HTTP PUT, RestClient.Net, type-safe HTTP
eleventyNavigation:
  key: PutAsync
  parent: HttpClient Extensions
  order: 3
permalink: /api/putasync/
---

Make a type-safe PUT request for full resource replacement.

## Namespace

`RestClient.Net`

## Containing Type

[HttpClientExtensions](/api/httpclient-extensions/)

## Signature

Same signature as [PostAsync](/api/postasync/).

```csharp
public static async Task<Result<TSuccess, HttpError<TError>>> PutAsync<TRequest, TSuccess, TError>(
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

## Example

```csharp
var updatedUser = new UpdateUserRequest("John Updated", "john.updated@example.com");

var result = await httpClient.PutAsync(
    url: "https://api.example.com/users/123".ToAbsoluteUrl(),
    body: updatedUser,
    serializeRequest: SerializeJson,
    deserializeSuccess: DeserializeUser,
    deserializeError: DeserializeApiError
);
```

## See Also

- [PostAsync](/api/postasync/) - Same signature, for creating resources
- [PatchAsync](/api/patchasync/) - For partial updates
