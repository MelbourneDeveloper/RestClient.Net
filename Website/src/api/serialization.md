---
layout: layouts/api.njk
title: Serialization
description: Complete guide to serialization in RestClient.Net - JSON, custom serializers, request/response handling.
keywords: RestClient.Net serialization, JSON serialization, HttpContent, custom serializers
eleventyNavigation:
  key: Serialization
  parent: API Reference
  order: 3
permalink: /api/serialization/
---

RestClient.Net gives you full control over how requests are serialized and responses are deserialized.

## Deserializers

Deserializers convert [`HttpResponseMessage`](https://learn.microsoft.com/en-us/dotnet/api/system.net.http.httpresponsemessage) to your model types:

```csharp
Func<HttpResponseMessage, CancellationToken, Task<TSuccess>> deserializeSuccess
Func<HttpResponseMessage, CancellationToken, Task<TError>> deserializeError
```

### JSON Deserialization

Using `System.Text.Json`:

```csharp
var result = await httpClient.GetAsync(
    url: "https://api.example.com/users/1".ToAbsoluteUrl(),
    deserializeSuccess: async (response, ct) =>
        await response.Content.ReadFromJsonAsync<User>(ct)
        ?? throw new InvalidOperationException("Null response"),
    deserializeError: async (response, ct) =>
        await response.Content.ReadFromJsonAsync<ApiError>(ct)
        ?? new ApiError("Unknown error")
);
```

### Reusable Deserializers

Create a static class with reusable deserializer methods:

```csharp
public static class Deserializers
{
    public static async Task<T> Json<T>(HttpResponseMessage response, CancellationToken ct)
        where T : class =>
        await response.Content.ReadFromJsonAsync<T>(ct)
        ?? throw new InvalidOperationException($"Failed to deserialize {typeof(T).Name}");

    public static async Task<ApiError> Error(HttpResponseMessage response, CancellationToken ct) =>
        await response.Content.ReadFromJsonAsync<ApiError>(ct)
        ?? new ApiError("Unknown error");
}

// Usage
var result = await httpClient.GetAsync(
    url: "https://api.example.com/users/1".ToAbsoluteUrl(),
    deserializeSuccess: Deserializers.Json<User>,
    deserializeError: Deserializers.Error
);
```

### Custom JSON Options

Configure [`JsonSerializerOptions`](https://learn.microsoft.com/en-us/dotnet/api/system.text.json.jsonserializeroptions) for custom serialization behavior:

```csharp
public static class Deserializers
{
    private static readonly JsonSerializerOptions Options = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    public static async Task<T> Json<T>(HttpResponseMessage response, CancellationToken ct)
        where T : class =>
        await response.Content.ReadFromJsonAsync<T>(Options, ct)
        ?? throw new InvalidOperationException($"Failed to deserialize {typeof(T).Name}");
}
```

## Serializers

Serializers convert your request body to [`HttpContent`](https://learn.microsoft.com/en-us/dotnet/api/system.net.http.httpcontent). They're used with [POST](/api/postasync/), [PUT](/api/putasync/), and [PATCH](/api/patchasync/) requests.

```csharp
Func<TBody, HttpContent> serializeRequest
```

### JSON Serialization

```csharp
var result = await httpClient.PostAsync(
    url: "https://api.example.com/users".ToAbsoluteUrl(),
    body: new CreateUserRequest("John", "john@example.com"),
    serializeRequest: body => JsonContent.Create(body),
    deserializeSuccess: Deserializers.Json<User>,
    deserializeError: Deserializers.Error
);
```

### Custom Content Types

#### Form URL Encoded

Using [`FormUrlEncodedContent`](https://learn.microsoft.com/en-us/dotnet/api/system.net.http.formurlencodedcontent):

```csharp
serializeRequest: body => new FormUrlEncodedContent(new Dictionary<string, string>
{
    ["name"] = body.Name,
    ["email"] = body.Email
})
```

#### Multipart Form Data

Using [`MultipartFormDataContent`](https://learn.microsoft.com/en-us/dotnet/api/system.net.http.multipartformdatacontent) for file uploads:

```csharp
serializeRequest: body =>
{
    var content = new MultipartFormDataContent();
    content.Add(new StringContent(body.Name), "name");
    content.Add(new ByteArrayContent(body.FileBytes), "file", body.FileName);
    return content;
}
```

#### XML

Using [`XmlSerializer`](https://learn.microsoft.com/en-us/dotnet/api/system.xml.serialization.xmlserializer):

```csharp
serializeRequest: body =>
{
    var serializer = new XmlSerializer(typeof(TBody));
    using var writer = new StringWriter();
    serializer.Serialize(writer, body);
    return new StringContent(writer.ToString(), Encoding.UTF8, "application/xml");
}
```

### Stream Response

Using [`Stream`](https://learn.microsoft.com/en-us/dotnet/api/system.io.stream) for large files:

```csharp
var result = await httpClient.GetAsync(
    url: "https://api.example.com/files/large".ToAbsoluteUrl(),
    deserializeSuccess: async (response, ct) => await response.Content.ReadAsStreamAsync(ct),
    deserializeError: Deserializers.Error
);
```

## See Also

- [HttpClient Extensions](/api/httpclient-extensions/) - Extension methods using serializers
- [Result Types](/api/result-types/) - Understanding the return types
