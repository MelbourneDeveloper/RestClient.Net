---
layout: layouts/api.njk
title: DeleteAsync Method
description: Make a type-safe DELETE request.
keywords: DeleteAsync, HTTP DELETE, RestClient.Net, type-safe HTTP
eleventyNavigation:
  key: DeleteAsync
  parent: HttpClient Extensions
  order: 4
permalink: /api/deleteasync/
---

Make a type-safe DELETE request.

## Namespace

`RestClient.Net`

## Containing Type

[HttpClientExtensions](/api/httpclient-extensions/)

## Signature

Same signature as [GetAsync](/api/getasync/).

```csharp
public static async Task<Result<TSuccess, HttpError<TError>>> DeleteAsync<TSuccess, TError>(
    this HttpClient httpClient,
    AbsoluteUrl url,
    Func<HttpResponseMessage, CancellationToken, Task<TSuccess>> deserializeSuccess,
    Func<HttpResponseMessage, CancellationToken, Task<TError>> deserializeError,
    IReadOnlyDictionary<string, string>? headers = null,
    CancellationToken cancellationToken = default
)
```

## Example

```csharp
var result = await httpClient.DeleteAsync(
    url: "https://api.example.com/users/123".ToAbsoluteUrl(),
    deserializeSuccess: async (r, ct) => true,
    deserializeError: DeserializeApiError
);

var output = result switch
{
    Ok(true) => "User deleted",
    Error(ResponseError(var err, var status, _)) => $"Error {status}: {err.Message}",
    Error(ExceptionError(var ex)) => $"Exception: {ex.Message}",
};
```

## See Also

- [GetAsync](/api/getasync/) - Same signature
- [HttpClientExtensions](/api/httpclient-extensions/) - All extension methods
