#!/usr/bin/env node

/**
 * Generate API documentation for RestClient.Net from C# source files.
 * Extracts XML documentation comments and generates Markdown with proper links.
 *
 * Source: RestClient.Net repo root (parent of Website folder)
 * Output: Website/src/api/
 */

import fs from 'fs';
import path from 'path';
import { fileURLToPath } from 'url';

const __filename = fileURLToPath(import.meta.url);
const __dirname = path.dirname(__filename);

const WEBSITE_DIR = path.dirname(__dirname);
const RESTCLIENT_NET_DIR = path.dirname(WEBSITE_DIR);
const API_OUTPUT_DIR = path.join(WEBSITE_DIR, 'src', 'api');
const API_OUTPUT_DIR_ZH = path.join(WEBSITE_DIR, 'src', 'zh', 'api');

// .NET type to Microsoft docs URL mapping
const DOTNET_DOCS = {
  'HttpClient': 'https://learn.microsoft.com/en-us/dotnet/api/system.net.http.httpclient',
  'IHttpClientFactory': 'https://learn.microsoft.com/en-us/dotnet/api/system.net.http.ihttpclientfactory',
  'HttpContent': 'https://learn.microsoft.com/en-us/dotnet/api/system.net.http.httpcontent',
  'HttpResponseMessage': 'https://learn.microsoft.com/en-us/dotnet/api/system.net.http.httpresponsemessage',
  'HttpStatusCode': 'https://learn.microsoft.com/en-us/dotnet/api/system.net.httpstatuscode',
  'HttpMethod': 'https://learn.microsoft.com/en-us/dotnet/api/system.net.http.httpmethod',
  'CancellationToken': 'https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken',
  'Task': 'https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1',
  'Exception': 'https://learn.microsoft.com/en-us/dotnet/api/system.exception',
  'Func': 'https://learn.microsoft.com/en-us/dotnet/api/system.func-2',
  'JsonSerializerOptions': 'https://learn.microsoft.com/en-us/dotnet/api/system.text.json.jsonserializeroptions',
  'FormUrlEncodedContent': 'https://learn.microsoft.com/en-us/dotnet/api/system.net.http.formurlencodedcontent',
  'MultipartFormDataContent': 'https://learn.microsoft.com/en-us/dotnet/api/system.net.http.multipartformdatacontent',
  'XmlSerializer': 'https://learn.microsoft.com/en-us/dotnet/api/system.xml.serialization.xmlserializer',
  'Stream': 'https://learn.microsoft.com/en-us/dotnet/api/system.io.stream',
  'IReadOnlyDictionary': 'https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ireadonlydictionary-2',
};

// Internal links within the API docs
const INTERNAL_LINKS = {
  'Result': '/api/result-types/#resulttsuccessterror',
  'HttpError': '/api/result-types/#httperrorterror',
  'ResponseError': '/api/result-types/#responseerror-properties',
  'ExceptionError': '/api/result-types/#exceptionerror-properties',
  'Deserialize': '/api/serialization/',
  'Serialize': '/api/serialization/',
};

// External documentation links
const EXTERNAL_DOCS = {
  'OpenAPI': 'https://swagger.io/specification/',
  'MCP': 'https://modelcontextprotocol.io/',
  'record': 'https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/record',
  'switch expression': 'https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/operators/switch-expression',
  'global using': 'https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/using-directive#global-modifier',
  'Roslyn analyzer': 'https://learn.microsoft.com/en-us/dotnet/csharp/roslyn-sdk/',
};

/**
 * Parse XML documentation from a C# file
 */
function parseXmlDoc(content) {
  const members = [];

  // Match /// <summary> blocks followed by method/class declarations
  const docPattern = /\/\/\/\s*<summary>\s*([\s\S]*?)\/\/\/\s*<\/summary>([\s\S]*?)(?=public|private|protected|internal|\[)/g;

  let match;
  while ((match = docPattern.exec(content)) !== null) {
    const summaryLines = match[1].split('\n')
      .map(line => line.replace(/^\s*\/\/\/\s*/, '').trim())
      .filter(line => line.length > 0)
      .join(' ');

    const additionalDoc = match[2];

    // Extract param tags
    const params = [];
    const paramPattern = /\/\/\/\s*<param name="(\w+)">(.*?)<\/param>/g;
    let paramMatch;
    while ((paramMatch = paramPattern.exec(additionalDoc)) !== null) {
      params.push({ name: paramMatch[1], description: paramMatch[2].trim() });
    }

    // Extract typeparam tags
    const typeParams = [];
    const typeParamPattern = /\/\/\/\s*<typeparam name="(\w+)">(.*?)<\/typeparam>/g;
    let typeParamMatch;
    while ((typeParamMatch = typeParamPattern.exec(additionalDoc)) !== null) {
      typeParams.push({ name: typeParamMatch[1], description: typeParamMatch[2].trim() });
    }

    // Extract returns tag
    const returnsMatch = /\/\/\/\s*<returns>(.*?)<\/returns>/s.exec(additionalDoc);
    const returns = returnsMatch ? returnsMatch[1].trim() : null;

    members.push({ summary: summaryLines, params, typeParams, returns });
  }

  return members;
}

/**
 * Convert a type name to a linked version
 */
function linkType(typeName) {
  // Check .NET docs first
  for (const [type, url] of Object.entries(DOTNET_DOCS)) {
    if (typeName.includes(type)) {
      return typeName.replace(type, `[${type}](${url})`);
    }
  }

  // Check internal links
  for (const [type, url] of Object.entries(INTERNAL_LINKS)) {
    if (typeName.includes(type)) {
      return typeName.replace(type, `[${type}](${url})`);
    }
  }

  return typeName;
}

/**
 * Process see cref tags to create links
 */
function processSeeCref(text) {
  return text.replace(/<see cref="([^"]+)"\/>/g, (match, cref) => {
    const typeName = cref.split('.').pop();
    const url = DOTNET_DOCS[typeName] || INTERNAL_LINKS[typeName];
    return url ? `[\`${typeName}\`](${url})` : `\`${typeName}\``;
  });
}

/**
 * Generate markdown for a parameter table
 */
function generateParamTable(params, typeParams) {
  if (params.length === 0 && typeParams.length === 0) return '';

  let md = '### Parameters\n\n';
  md += '| Parameter | Type | Description |\n';
  md += '|-----------|------|-------------|\n';

  for (const tp of typeParams) {
    md += `| \`${tp.name}\` | Type parameter | ${processSeeCref(tp.description)} |\n`;
  }

  for (const p of params) {
    const desc = processSeeCref(p.description);
    md += `| \`${p.name}\` | See signature | ${desc} |\n`;
  }

  return md + '\n';
}

/**
 * Generate HttpClient Extensions CLASS SUMMARY page (just a table of methods with links)
 */
function generateHttpClientExtensions() {
  return `---
layout: layouts/api.njk
title: HttpClientExtensions Class
description: Extension methods for HttpClient that return Result types instead of throwing exceptions.
keywords: HttpClientExtensions, HttpClient, REST API, C# HTTP client, extension methods
eleventyNavigation:
  key: HttpClient Extensions
  parent: API Reference
  order: 1
permalink: /api/httpclient-extensions/
---

Extension methods for [\`HttpClient\`](${DOTNET_DOCS.HttpClient}) that return [\`Result<TSuccess, HttpError<TError>>\`](/api/result/) instead of throwing exceptions.

## Namespace

\`RestClient.Net\`

## Methods

| Method | Description |
|--------|-------------|
| [GetAsync&lt;TSuccess, TError&gt;](/api/getasync/) | Make a type-safe GET request |
| [PostAsync&lt;TRequest, TSuccess, TError&gt;](/api/postasync/) | Make a type-safe POST request with body |
| [PutAsync&lt;TRequest, TSuccess, TError&gt;](/api/putasync/) | Make a type-safe PUT request for full replacement |
| [DeleteAsync&lt;TSuccess, TError&gt;](/api/deleteasync/) | Make a type-safe DELETE request |
| [PatchAsync&lt;TRequest, TSuccess, TError&gt;](/api/patchasync/) | Make a type-safe PATCH request for partial updates |

## See Also

- [Result&lt;TSuccess, TError&gt;](/api/result/) - The discriminated union return type
- [HttpError&lt;TError&gt;](/api/httperror/) - HTTP-specific error wrapper
- [Serialization](/api/serialization/) - Custom serialization and deserialization
`;
}

/**
 * Generate GetAsync METHOD DETAIL page
 */
function generateGetAsync() {
  return `---
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

\`RestClient.Net\`

## Containing Type

[HttpClientExtensions](/api/httpclient-extensions/)

## Signature

\`\`\`csharp
public static async Task<Result<TSuccess, HttpError<TError>>> GetAsync<TSuccess, TError>(
    this HttpClient httpClient,
    AbsoluteUrl url,
    Func<HttpResponseMessage, CancellationToken, Task<TSuccess>> deserializeSuccess,
    Func<HttpResponseMessage, CancellationToken, Task<TError>> deserializeError,
    IReadOnlyDictionary<string, string>? headers = null,
    CancellationToken cancellationToken = default
)
\`\`\`

## Parameters

| Parameter | Type | Description |
|-----------|------|-------------|
| \`url\` | \`AbsoluteUrl\` | The request URL (use \`.ToAbsoluteUrl()\` extension) |
| \`deserializeSuccess\` | [\`Func<HttpResponseMessage, CancellationToken, Task<TSuccess>>\`](${DOTNET_DOCS.Func}) | Function to deserialize success response |
| \`deserializeError\` | [\`Func<HttpResponseMessage, CancellationToken, Task<TError>>\`](${DOTNET_DOCS.Func}) | Function to deserialize error response |
| \`headers\` | [\`IReadOnlyDictionary<string, string>?\`](${DOTNET_DOCS.IReadOnlyDictionary}) | Optional request headers |
| \`cancellationToken\` | [\`CancellationToken\`](${DOTNET_DOCS.CancellationToken}) | Optional cancellation token |

## Returns

[\`Task<Result<TSuccess, HttpError<TError>>>\`](${DOTNET_DOCS.Task}) - A discriminated union that is either:
- [\`Ok<TSuccess>\`](/api/ok/) - Success with deserialized data
- [\`Error<HttpError<TError>>\`](/api/error/) - Error with [ResponseError](/api/responseerror/) or [ExceptionError](/api/exceptionerror/)

## Example

\`\`\`csharp
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
\`\`\`

## See Also

- [HttpClientExtensions](/api/httpclient-extensions/) - All extension methods
- [Result&lt;TSuccess, TError&gt;](/api/result/) - The return type
- [Serialization](/api/serialization/) - Deserializer examples
`;
}

/**
 * Generate PostAsync METHOD DETAIL page
 */
function generatePostAsync() {
  return `---
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

\`RestClient.Net\`

## Containing Type

[HttpClientExtensions](/api/httpclient-extensions/)

## Signature

\`\`\`csharp
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
\`\`\`

## Parameters

| Parameter | Type | Description |
|-----------|------|-------------|
| \`url\` | \`AbsoluteUrl\` | The request URL |
| \`body\` | \`TRequest\` | The request body object |
| \`serializeRequest\` | [\`Func<TRequest, HttpContent>\`](${DOTNET_DOCS.Func}) | Function to serialize the request body |
| \`deserializeSuccess\` | [\`Func<HttpResponseMessage, CancellationToken, Task<TSuccess>>\`](${DOTNET_DOCS.Func}) | Function to deserialize success response |
| \`deserializeError\` | [\`Func<HttpResponseMessage, CancellationToken, Task<TError>>\`](${DOTNET_DOCS.Func}) | Function to deserialize error response |
| \`headers\` | [\`IReadOnlyDictionary<string, string>?\`](${DOTNET_DOCS.IReadOnlyDictionary}) | Optional request headers |
| \`cancellationToken\` | [\`CancellationToken\`](${DOTNET_DOCS.CancellationToken}) | Optional cancellation token |

## Returns

[\`Task<Result<TSuccess, HttpError<TError>>>\`](${DOTNET_DOCS.Task}) - A discriminated union that is either:
- [\`Ok<TSuccess>\`](/api/ok/) - Success with deserialized data
- [\`Error<HttpError<TError>>\`](/api/error/) - Error with [ResponseError](/api/responseerror/) or [ExceptionError](/api/exceptionerror/)

## Example

\`\`\`csharp
var newUser = new CreateUserRequest("John", "john@example.com");

var result = await httpClient.PostAsync(
    url: "https://api.example.com/users".ToAbsoluteUrl(),
    body: newUser,
    serializeRequest: SerializeJson,
    deserializeSuccess: DeserializeUser,
    deserializeError: DeserializeApiError
);
\`\`\`

## See Also

- [HttpClientExtensions](/api/httpclient-extensions/) - All extension methods
- [Serialization](/api/serialization/) - Serializer examples
`;
}

/**
 * Generate PutAsync METHOD DETAIL page
 */
function generatePutAsync() {
  return `---
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

\`RestClient.Net\`

## Containing Type

[HttpClientExtensions](/api/httpclient-extensions/)

## Signature

Same signature as [PostAsync](/api/postasync/).

\`\`\`csharp
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
\`\`\`

## Example

\`\`\`csharp
var updatedUser = new UpdateUserRequest("John Updated", "john.updated@example.com");

var result = await httpClient.PutAsync(
    url: "https://api.example.com/users/123".ToAbsoluteUrl(),
    body: updatedUser,
    serializeRequest: SerializeJson,
    deserializeSuccess: DeserializeUser,
    deserializeError: DeserializeApiError
);
\`\`\`

## See Also

- [PostAsync](/api/postasync/) - Same signature, for creating resources
- [PatchAsync](/api/patchasync/) - For partial updates
`;
}

/**
 * Generate DeleteAsync METHOD DETAIL page
 */
function generateDeleteAsync() {
  return `---
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

\`RestClient.Net\`

## Containing Type

[HttpClientExtensions](/api/httpclient-extensions/)

## Signature

Same signature as [GetAsync](/api/getasync/).

\`\`\`csharp
public static async Task<Result<TSuccess, HttpError<TError>>> DeleteAsync<TSuccess, TError>(
    this HttpClient httpClient,
    AbsoluteUrl url,
    Func<HttpResponseMessage, CancellationToken, Task<TSuccess>> deserializeSuccess,
    Func<HttpResponseMessage, CancellationToken, Task<TError>> deserializeError,
    IReadOnlyDictionary<string, string>? headers = null,
    CancellationToken cancellationToken = default
)
\`\`\`

## Example

\`\`\`csharp
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
\`\`\`

## See Also

- [GetAsync](/api/getasync/) - Same signature
- [HttpClientExtensions](/api/httpclient-extensions/) - All extension methods
`;
}

/**
 * Generate PatchAsync METHOD DETAIL page
 */
function generatePatchAsync() {
  return `---
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

\`RestClient.Net\`

## Containing Type

[HttpClientExtensions](/api/httpclient-extensions/)

## Signature

Same signature as [PostAsync](/api/postasync/).

\`\`\`csharp
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
\`\`\`

## Example

\`\`\`csharp
var patch = new PatchUserRequest { Email = "new.email@example.com" };

var result = await httpClient.PatchAsync(
    url: "https://api.example.com/users/123".ToAbsoluteUrl(),
    body: patch,
    serializeRequest: SerializeJson,
    deserializeSuccess: DeserializeUser,
    deserializeError: DeserializeApiError
);
\`\`\`

## See Also

- [PostAsync](/api/postasync/) - Same signature, for creating resources
- [PutAsync](/api/putasync/) - For full replacement
`;
}

/**
 * Generate Result Types page
 */
function generateResultTypes() {
  return `---
layout: layouts/api.njk
title: Result Types
description: Complete reference for RestClient.Net Result types - discriminated unions for type-safe HTTP error handling with pattern matching.
keywords: Result types, HttpError, discriminated unions, pattern matching, C# error handling
eleventyNavigation:
  key: Result Types
  parent: API Reference
  order: 2
permalink: /api/result-types/
---

RestClient.Net uses [discriminated unions](${EXTERNAL_DOCS.record}) to represent HTTP responses. This forces you to handle all possible outcomes at compile time.

## Result&lt;TSuccess, TError&gt;

The core result type that represents either success or failure.

\`\`\`csharp
public abstract record Result<TSuccess, TError>
{
    public record Ok(TSuccess Value) : Result<TSuccess, TError>;
    public record Error(TError Value) : Result<TSuccess, TError>;
}
\`\`\`

### Pattern Matching

Use [switch expressions](${EXTERNAL_DOCS['switch expression']}) to handle all cases:

\`\`\`csharp
var message = result switch
{
    Result<User, HttpError<ApiError>>.Ok(var user) =>
        $"Got user: {user.Name}",

    Result<User, HttpError<ApiError>>.Error(var error) =>
        $"Error occurred: {error}"
};
\`\`\`

## HttpError&lt;TError&gt;

Represents HTTP-specific errors. Can be either a response error (server returned an error status) or an exception error (network failure, timeout, etc.).

\`\`\`csharp
public abstract record HttpError<TError>
{
    public record ResponseError(
        TError Error,
        HttpStatusCode StatusCode,
        HttpResponseHeaders Headers
    ) : HttpError<TError>;

    public record ExceptionError(Exception Exception) : HttpError<TError>;
}
\`\`\`

### ResponseError Properties

| Property | Type | Description |
|----------|------|-------------|
| \`Error\` | \`TError\` | Your deserialized error model |
| \`StatusCode\` | [\`HttpStatusCode\`](${DOTNET_DOCS.HttpStatusCode}) | The HTTP status code (e.g., 404, 500) |
| \`Headers\` | \`HttpResponseHeaders\` | Response headers for accessing metadata |

### ExceptionError Properties

| Property | Type | Description |
|----------|------|-------------|
| \`Exception\` | [\`Exception\`](${DOTNET_DOCS.Exception}) | The caught exception (timeout, network error, etc.) |

### Full Pattern Matching Example

\`\`\`csharp
var message = result switch
{
    Result<User, HttpError<ApiError>>.Ok(var user) =>
        $"Success: {user.Name}",

    Result<User, HttpError<ApiError>>.Error(
        HttpError<ApiError>.ResponseError(var err, var status, _)) =>
        $"API Error {status}: {err.Message}",

    Result<User, HttpError<ApiError>>.Error(
        HttpError<ApiError>.ExceptionError(var ex)) =>
        $"Exception: {ex.Message}",
};
\`\`\`

## Type Aliases

The full type names are verbose. Define [global using aliases](${EXTERNAL_DOCS['global using']}) in \`GlobalUsings.cs\`:

\`\`\`csharp
// GlobalUsings.cs - Define once, use everywhere

// Result aliases
global using OkUser = Outcome.Result<User, Outcome.HttpError<ApiError>>
    .Ok<User, Outcome.HttpError<ApiError>>;

global using ErrorUser = Outcome.Result<User, Outcome.HttpError<ApiError>>
    .Error<User, Outcome.HttpError<ApiError>>;

// HttpError aliases
global using ResponseErrorUser = Outcome.HttpError<ApiError>.ResponseError;
global using ExceptionErrorUser = Outcome.HttpError<ApiError>.ExceptionError;
\`\`\`

### Using Type Aliases

With aliases defined, pattern matching becomes much cleaner:

\`\`\`csharp
var message = result switch
{
    OkUser(var user) => $"Success: {user.Name}",
    ErrorUser(ResponseErrorUser(var err, var status, _)) => $"API Error {status}: {err.Message}",
    ErrorUser(ExceptionErrorUser(var ex)) => $"Exception: {ex.Message}",
};
\`\`\`

## Exhaustion Analyzer

The Exhaustion [Roslyn analyzer](${EXTERNAL_DOCS['Roslyn analyzer']}) ensures you handle all cases:

\`\`\`csharp
// This won't compile!
var message = result switch
{
    OkUser(var user) => "Success",
    ErrorUser(ResponseErrorUser(...)) => "API Error",
    // COMPILE ERROR: Missing ExceptionError case!
};
\`\`\`

The compiler error:

\`\`\`
error EXHAUSTION001: Switch on Result is not exhaustive;
Missing: Error<User, HttpError<ApiError>> with ExceptionError
\`\`\`

## Handling Specific Status Codes

\`\`\`csharp
var message = result switch
{
    OkUser(var user) => $"Success: {user.Name}",

    ErrorUser(ResponseErrorUser(_, HttpStatusCode.NotFound, _)) =>
        "User not found",

    ErrorUser(ResponseErrorUser(_, HttpStatusCode.Unauthorized, _)) =>
        "Authentication required",

    ErrorUser(ResponseErrorUser(var err, var status, _)) =>
        $"Error {(int)status}: {err.Message}",

    ErrorUser(ExceptionErrorUser(var ex)) =>
        $"Network error: {ex.Message}",
};
\`\`\`

## See Also

- [HttpClient Extensions](/api/httpclient-extensions/) - Extension methods that return Result types
- [Serialization](/api/serialization/) - Custom serialization and deserialization
`;
}

/**
 * Generate Serialization page
 */
function generateSerialization() {
  return `---
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

Deserializers convert [\`HttpResponseMessage\`](${DOTNET_DOCS.HttpResponseMessage}) to your model types:

\`\`\`csharp
Func<HttpResponseMessage, CancellationToken, Task<TSuccess>> deserializeSuccess
Func<HttpResponseMessage, CancellationToken, Task<TError>> deserializeError
\`\`\`

### JSON Deserialization

Using \`System.Text.Json\`:

\`\`\`csharp
var result = await httpClient.GetAsync(
    url: "https://api.example.com/users/1".ToAbsoluteUrl(),
    deserializeSuccess: async (response, ct) =>
        await response.Content.ReadFromJsonAsync<User>(ct)
        ?? throw new InvalidOperationException("Null response"),
    deserializeError: async (response, ct) =>
        await response.Content.ReadFromJsonAsync<ApiError>(ct)
        ?? new ApiError("Unknown error")
);
\`\`\`

### Reusable Deserializers

Create a static class with reusable deserializer methods:

\`\`\`csharp
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
\`\`\`

### Custom JSON Options

Configure [\`JsonSerializerOptions\`](${DOTNET_DOCS.JsonSerializerOptions}) for custom serialization behavior:

\`\`\`csharp
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
\`\`\`

## Serializers

Serializers convert your request body to [\`HttpContent\`](${DOTNET_DOCS.HttpContent}). They're used with [POST](/api/httpclient-extensions/#postasync), [PUT](/api/httpclient-extensions/#putasync), and [PATCH](/api/httpclient-extensions/#patchasync) requests.

\`\`\`csharp
Func<TBody, HttpContent> serializeRequest
\`\`\`

### JSON Serialization

\`\`\`csharp
var result = await httpClient.PostAsync(
    url: "https://api.example.com/users".ToAbsoluteUrl(),
    body: new CreateUserRequest("John", "john@example.com"),
    serializeRequest: body => JsonContent.Create(body),
    deserializeSuccess: Deserializers.Json<User>,
    deserializeError: Deserializers.Error
);
\`\`\`

### Custom Content Types

#### Form URL Encoded

Using [\`FormUrlEncodedContent\`](${DOTNET_DOCS.FormUrlEncodedContent}):

\`\`\`csharp
serializeRequest: body => new FormUrlEncodedContent(new Dictionary<string, string>
{
    ["name"] = body.Name,
    ["email"] = body.Email
})
\`\`\`

#### Multipart Form Data

Using [\`MultipartFormDataContent\`](${DOTNET_DOCS.MultipartFormDataContent}) for file uploads:

\`\`\`csharp
serializeRequest: body =>
{
    var content = new MultipartFormDataContent();
    content.Add(new StringContent(body.Name), "name");
    content.Add(new ByteArrayContent(body.FileBytes), "file", body.FileName);
    return content;
}
\`\`\`

#### XML

Using [\`XmlSerializer\`](${DOTNET_DOCS.XmlSerializer}):

\`\`\`csharp
serializeRequest: body =>
{
    var serializer = new XmlSerializer(typeof(TBody));
    using var writer = new StringWriter();
    serializer.Serialize(writer, body);
    return new StringContent(writer.ToString(), Encoding.UTF8, "application/xml");
}
\`\`\`

### Stream Response

Using [\`Stream\`](${DOTNET_DOCS.Stream}) for large files:

\`\`\`csharp
var result = await httpClient.GetAsync(
    url: "https://api.example.com/files/large".ToAbsoluteUrl(),
    deserializeSuccess: async (response, ct) => await response.Content.ReadAsStreamAsync(ct),
    deserializeError: Deserializers.Error
);
\`\`\`

## See Also

- [HttpClient Extensions](/api/httpclient-extensions/) - Extension methods using serializers
- [Result Types](/api/result-types/) - Understanding the return types
`;
}

/**
 * Generate OpenAPI Generator page
 */
function generateOpenApiGenerator() {
  return `---
layout: layouts/docs.njk
title: OpenAPI Generator
lang: en
permalink: /api/openapi-generator/
eleventyNavigation:
  key: OpenAPI Generator
  parent: API Reference
  order: 5
---

Generate type-safe C# clients from [OpenAPI 3.x](${EXTERNAL_DOCS.OpenAPI}) specifications.

## Installation

\`\`\`bash
dotnet add package RestClient.Net.OpenApiGenerator
\`\`\`

## CLI Usage

\`\`\`bash
dotnet run --project RestClient.Net.OpenApiGenerator.Cli -- \\
  -u api.yaml \\
  -o Generated \\
  -n YourApi.Generated
\`\`\`

### CLI Options

| Option | Short | Description |
|--------|-------|-------------|
| \`--openapi-url\` | \`-u\` | Path to OpenAPI spec (YAML or JSON) |
| \`--output\` | \`-o\` | Output directory for generated files |
| \`--namespace\` | \`-n\` | C# namespace for generated code |
| \`--client-name\` | \`-c\` | Prefix for generated client class names |

## Generated Code

The generator creates:

1. **Model classes** - [Records](${EXTERNAL_DOCS.record}) for all schemas
2. **[HttpClient](/api/httpclient-extensions/) extension methods** - For each endpoint
3. **[Result type aliases](/api/result-types/#type-aliases)** - For concise pattern matching

### Example Output

For an OpenAPI spec with a \`/users/{id}\` endpoint:

\`\`\`csharp
// Generated extension method
public static async Task<ResultUser> GetUserById(
    this HttpClient httpClient,
    string id,
    CancellationToken ct = default)
{
    return await httpClient.GetAsync(
        url: $"https://api.example.com/users/{id}".ToAbsoluteUrl(),
        deserializeSuccess: async (r, c) => await r.Content.ReadFromJsonAsync<User>(c),
        deserializeError: async (r, c) => await r.Content.ReadFromJsonAsync<ErrorResponse>(c),
        ct
    );
}

// Generated type alias
global using ResultUser = Outcome.Result<User, Outcome.HttpError<ErrorResponse>>;
global using OkUser = ResultUser.Ok<User, Outcome.HttpError<ErrorResponse>>;
global using ErrorUser = ResultUser.Error<User, Outcome.HttpError<ErrorResponse>>;
\`\`\`

### Usage

\`\`\`csharp
using YourApi.Generated;

var httpClient = factory.CreateClient();

// Type-safe API call
var result = await httpClient.GetUserById("123");

// Pattern match on result
var output = result switch
{
    OkUser(var user) => $"Found: {user.Name}",
    ErrorUser(ResponseErrorUser(var err, var status, _)) => $"Error {status}",
    ErrorUser(ExceptionErrorUser(var ex)) => $"Exception: {ex.Message}",
};
\`\`\`

## Supported OpenAPI Features

- **HTTP Methods:** GET, POST, PUT, DELETE, PATCH
- **Parameters:** path, query, header
- **Request Bodies:** JSON, form data
- **Responses:** All status codes, multiple content types
- **Schemas:** objects, arrays, enums, [oneOf](https://swagger.io/docs/specification/data-models/oneof-anyof-allof-not/), [allOf](https://swagger.io/docs/specification/data-models/oneof-anyof-allof-not/), [anyOf](https://swagger.io/docs/specification/data-models/oneof-anyof-allof-not/)
- **References:** [$ref](https://swagger.io/docs/specification/using-ref/) for local and remote schemas

## See Also

- [HttpClient Extensions](/api/httpclient-extensions/) - The extension methods generated
- [Result Types](/api/result-types/) - Understanding the return types
- [MCP Generator](/api/mcp-generator/) - Generate AI tools from OpenAPI
`;
}

/**
 * Generate MCP Generator page
 */
function generateMcpGenerator() {
  return `---
layout: layouts/docs.njk
title: MCP Generator
lang: en
permalink: /api/mcp-generator/
eleventyNavigation:
  key: MCP Generator
  parent: API Reference
  order: 6
---

Generate [Model Context Protocol (MCP)](${EXTERNAL_DOCS.MCP}) servers from OpenAPI specifications for AI integration with Claude Code and other tools.

## Installation

\`\`\`bash
dotnet add package RestClient.Net.McpGenerator
\`\`\`

## Prerequisites

First, generate the REST client using the [OpenAPI Generator](/api/openapi-generator/):

\`\`\`bash
dotnet run --project RestClient.Net.OpenApiGenerator.Cli -- \\
  -u api.yaml -o Generated -n YourApi.Generated
\`\`\`

## CLI Usage

\`\`\`bash
dotnet run --project RestClient.Net.McpGenerator.Cli -- \\
  --openapi-url api.yaml \\
  --output-file Generated/McpTools.g.cs \\
  --namespace YourApi.Mcp \\
  --server-name YourApi \\
  --ext-namespace YourApi.Generated \\
  --tags "Search,Resources"
\`\`\`

### CLI Options

| Option | Description |
|--------|-------------|
| \`--openapi-url\` | Path to [OpenAPI](${EXTERNAL_DOCS.OpenAPI}) specification |
| \`--output-file\` | Output file for generated MCP tools |
| \`--namespace\` | C# namespace for MCP server |
| \`--server-name\` | Name of the MCP server |
| \`--ext-namespace\` | Namespace of generated REST client |
| \`--tags\` | OpenAPI tags to include (comma-separated) |

## Generated Code

The generator creates MCP tool definitions that wrap the [HttpClient extensions](/api/httpclient-extensions/):

\`\`\`csharp
[McpServerToolType]
public static partial class McpTools
{
    [McpServerTool(Name = "get_user")]
    [Description("Get user by ID")]
    public static async Task<string> GetUser(
        [Description("User ID")] string id,
        HttpClient httpClient,
        CancellationToken ct)
    {
        var result = await httpClient.GetUserById(id, ct);
        return result switch
        {
            OkUser(var user) => JsonSerializer.Serialize(user),
            ErrorUser(var error) => $"Error: {error}"
        };
    }
}
\`\`\`

## Claude Code Integration

Add to your Claude Code configuration:

\`\`\`json
{
  "mcpServers": {
    "yourapi": {
      "command": "dotnet",
      "args": ["run", "--project", "YourApi.McpServer"]
    }
  }
}
\`\`\`

## Tool Naming

OpenAPI operations are converted to MCP tool names:

| OpenAPI | MCP Tool |
|---------|----------|
| \`GET /users/{id}\` | \`get_user\` |
| \`POST /users\` | \`create_user\` |
| \`PUT /users/{id}\` | \`update_user\` |
| \`DELETE /users/{id}\` | \`delete_user\` |

## See Also

- [OpenAPI Generator](/api/openapi-generator/) - Generate the REST client first
- [Result Types](/api/result-types/) - How results are handled
- [HttpClient Extensions](/api/httpclient-extensions/) - The underlying HTTP methods
`;
}

/**
 * Generate API index page
 */
function generateIndex() {
  return `---
layout: layouts/base.njk
title: API Reference
lang: en
permalink: /api/
---
<div style="max-width: 800px; margin: 0 auto; padding: 2rem;">
  <h1>API Reference</h1>
  <p style="color: var(--color-muted); margin-bottom: 2rem;">Complete API documentation for RestClient.Net</p>

  <h2>Core API</h2>
  <div class="features-grid" style="display: grid; grid-template-columns: repeat(auto-fill, minmax(250px, 1fr)); gap: 1rem; margin-bottom: 2rem;">
    <a href="/api/httpclient-extensions/" class="feature-card" style="display: block; padding: 1.5rem; border: 1px solid var(--color-border); border-radius: 8px; text-decoration: none;">
      <h3 style="margin: 0 0 0.5rem 0;">HttpClient Extensions</h3>
      <p style="margin: 0; color: var(--color-muted);"><a href="/api/httpclient-extensions/#getasync" style="color: var(--color-primary);">GetAsync</a>, <a href="/api/httpclient-extensions/#postasync" style="color: var(--color-primary);">PostAsync</a>, <a href="/api/httpclient-extensions/#putasync" style="color: var(--color-primary);">PutAsync</a>, <a href="/api/httpclient-extensions/#deleteasync" style="color: var(--color-primary);">DeleteAsync</a>, <a href="/api/httpclient-extensions/#patchasync" style="color: var(--color-primary);">PatchAsync</a></p>
    </a>
    <a href="/api/result-types/" class="feature-card" style="display: block; padding: 1.5rem; border: 1px solid var(--color-border); border-radius: 8px; text-decoration: none;">
      <h3 style="margin: 0 0 0.5rem 0;">Result Types</h3>
      <p style="margin: 0; color: var(--color-muted);"><a href="/api/result-types/#resulttsuccessterror" style="color: var(--color-primary);">Result&lt;T,E&gt;</a>, <a href="/api/result-types/#httperrorterror" style="color: var(--color-primary);">HttpError</a>, <a href="/api/result-types/#type-aliases" style="color: var(--color-primary);">Type Aliases</a></p>
    </a>
    <a href="/api/serialization/" class="feature-card" style="display: block; padding: 1.5rem; border: 1px solid var(--color-border); border-radius: 8px; text-decoration: none;">
      <h3 style="margin: 0 0 0.5rem 0;">Serialization</h3>
      <p style="margin: 0; color: var(--color-muted);"><a href="/api/serialization/#json-deserialization" style="color: var(--color-primary);">JSON</a>, <a href="/api/serialization/#custom-content-types" style="color: var(--color-primary);">Custom Serializers</a>, <a href="/api/serialization/#stream-response" style="color: var(--color-primary);">Response Handling</a></p>
    </a>
  </div>

  <h2>Code Generators</h2>
  <div class="features-grid" style="display: grid; grid-template-columns: repeat(auto-fill, minmax(250px, 1fr)); gap: 1rem; margin-bottom: 2rem;">
    <a href="/api/openapi-generator/" class="feature-card" style="display: block; padding: 1.5rem; border: 1px solid var(--color-border); border-radius: 8px; text-decoration: none;">
      <h3 style="margin: 0 0 0.5rem 0;">OpenAPI Generator</h3>
      <p style="margin: 0; color: var(--color-muted);">Generate type-safe C# clients from <a href="https://swagger.io/specification/" style="color: var(--color-primary);">OpenAPI 3.x</a></p>
    </a>
    <a href="/api/mcp-generator/" class="feature-card" style="display: block; padding: 1.5rem; border: 1px solid var(--color-border); border-radius: 8px; text-decoration: none;">
      <h3 style="margin: 0 0 0.5rem 0;">MCP Generator</h3>
      <p style="margin: 0; color: var(--color-muted);">Generate <a href="https://modelcontextprotocol.io/" style="color: var(--color-primary);">MCP servers</a> for AI integration</p>
    </a>
  </div>

  <h2>Guides</h2>
  <div class="features-grid" style="display: grid; grid-template-columns: repeat(auto-fill, minmax(250px, 1fr)); gap: 1rem; margin-bottom: 2rem;">
    <a href="/docs/" class="feature-card" style="display: block; padding: 1.5rem; border: 1px solid var(--color-border); border-radius: 8px; text-decoration: none;">
      <h3 style="margin: 0 0 0.5rem 0;">Getting Started</h3>
      <p style="margin: 0; color: var(--color-muted);">Installation and basic usage</p>
    </a>
    <a href="/docs/exhaustion/" class="feature-card" style="display: block; padding: 1.5rem; border: 1px solid var(--color-border); border-radius: 8px; text-decoration: none;">
      <h3 style="margin: 0 0 0.5rem 0;">Exhaustion Analyzer</h3>
      <p style="margin: 0; color: var(--color-muted);">Compile-time exhaustiveness checking with <a href="https://learn.microsoft.com/en-us/dotnet/csharp/roslyn-sdk/" style="color: var(--color-primary);">Roslyn</a></p>
    </a>
  </div>

  <h2 style="margin-top: 3rem;">NuGet Packages</h2>
  <table style="width: 100%; border-collapse: collapse;">
    <thead>
      <tr style="border-bottom: 2px solid var(--color-border);">
        <th style="text-align: left; padding: 0.75rem;">Package</th>
        <th style="text-align: left; padding: 0.75rem;">Description</th>
      </tr>
    </thead>
    <tbody>
      <tr style="border-bottom: 1px solid var(--color-border);">
        <td style="padding: 0.75rem;"><a href="https://www.nuget.org/packages/RestClient.Net"><code>RestClient.Net</code></a></td>
        <td style="padding: 0.75rem;">Core library with <a href="/api/httpclient-extensions/">HttpClient extensions</a></td>
      </tr>
      <tr style="border-bottom: 1px solid var(--color-border);">
        <td style="padding: 0.75rem;"><a href="https://www.nuget.org/packages/RestClient.Net.OpenApiGenerator"><code>RestClient.Net.OpenApiGenerator</code></a></td>
        <td style="padding: 0.75rem;"><a href="/api/openapi-generator/">OpenAPI 3.x client generator</a></td>
      </tr>
      <tr style="border-bottom: 1px solid var(--color-border);">
        <td style="padding: 0.75rem;"><a href="https://www.nuget.org/packages/RestClient.Net.McpGenerator"><code>RestClient.Net.McpGenerator</code></a></td>
        <td style="padding: 0.75rem;"><a href="/api/mcp-generator/">MCP server generator</a></td>
      </tr>
      <tr style="border-bottom: 1px solid var(--color-border);">
        <td style="padding: 0.75rem;"><a href="https://www.nuget.org/packages/Exhaustion"><code>Exhaustion</code></a></td>
        <td style="padding: 0.75rem;"><a href="https://learn.microsoft.com/en-us/dotnet/csharp/roslyn-sdk/">Roslyn analyzer</a> for <a href="/api/result-types/#exhaustion-analyzer">switch exhaustiveness</a></td>
      </tr>
    </tbody>
  </table>

  <h2 style="margin-top: 3rem;">External References</h2>
  <ul>
    <li><a href="https://learn.microsoft.com/en-us/dotnet/api/system.net.http.httpclient">HttpClient</a> - .NET HTTP client</li>
    <li><a href="https://learn.microsoft.com/en-us/dotnet/api/system.net.http.ihttpclientfactory">IHttpClientFactory</a> - Factory for HttpClient pooling</li>
    <li><a href="https://learn.microsoft.com/en-us/dotnet/api/system.text.json">System.Text.Json</a> - JSON serialization</li>
    <li><a href="https://swagger.io/specification/">OpenAPI Specification</a> - API specification format</li>
    <li><a href="https://modelcontextprotocol.io/">Model Context Protocol</a> - AI tool integration</li>
  </ul>
</div>
`;
}

/**
 * Main entry point
 */
function main() {
  console.log('Generating API documentation for RestClient.Net...');
  console.log(`Source: ${RESTCLIENT_NET_DIR}`);
  console.log(`Output: ${API_OUTPUT_DIR}`);

  // Ensure output directory exists
  fs.mkdirSync(API_OUTPUT_DIR, { recursive: true });

  // Generate all pages
  // Class summary pages link to individual member detail pages
  const pages = [
    { file: 'index.njk', content: generateIndex() },
    // HttpClientExtensions class summary + individual method pages
    { file: 'httpclient-extensions.md', content: generateHttpClientExtensions() },
    { file: 'getasync.md', content: generateGetAsync() },
    { file: 'postasync.md', content: generatePostAsync() },
    { file: 'putasync.md', content: generatePutAsync() },
    { file: 'deleteasync.md', content: generateDeleteAsync() },
    { file: 'patchasync.md', content: generatePatchAsync() },
    // Other pages
    { file: 'result-types.md', content: generateResultTypes() },
    { file: 'serialization.md', content: generateSerialization() },
    { file: 'openapi-generator.md', content: generateOpenApiGenerator() },
    { file: 'mcp-generator.md', content: generateMcpGenerator() },
  ];

  for (const { file, content } of pages) {
    const outputPath = path.join(API_OUTPUT_DIR, file);
    fs.writeFileSync(outputPath, content);
    console.log(`  Generated: ${file}`);
  }

  console.log('\n=== API documentation generation complete ===');
}

main();
