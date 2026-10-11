---
layout: layouts/api.njk
title: PatchAsync Method
description: Make a type-safe PATCH request for partial updates.
keywords: PatchAsync, HTTP PATCH, RestClient.Net, type-safe HTTP
eleventyNavigation:
  key: PatchAsync
  parent: HttpClient Extensions
  order: 5
permalink: /api/patchasync/
---

Make a type-safe PATCH request for partial updates.

## Namespace

`RestClient.Net`

## Containing Type

[HttpClientExtensions](/api/httpclient-extensions/)

## Signature

Same signature as [PostAsync](/api/postasync/).

```csharp
public static async Task<Result<TSuccess, HttpError<TError>>> PatchAsync<TRequest, TSuccess, TError>(
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
var patch = new PatchUserRequest { Email = "new.email@example.com" };

var result = await httpClient.PatchAsync(
    url: "https://api.example.com/users/123".ToAbsoluteUrl(),
    body: patch,
    serializeRequest: SerializeJson,
    deserializeSuccess: DeserializeUser,
    deserializeError: DeserializeApiError
);
```

## See Also

- [PostAsync](/api/postasync/) - Same signature, for creating resources
- [PutAsync](/api/putasync/) - For full replacement
