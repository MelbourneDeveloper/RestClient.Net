# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Code Rules

- NO DUPLICATION - EVER!!!! REMOVING DUPLICATION is the absolute HIGHEST PRIORITY!!!
- YOU ARE NOT ALLOWED TO SKIP TESTS
- No throwing exceptions, except for in tests
- FP style code. Pure functions with immutable types.
- Keep functions under 20 LOC
- Keep files under 300 LOC
- NEVER copy files. Only MOVE files
- Don't use Git unless I explicitly ask you to
- Promote code analysis warnings to errors
- EXHAUSTION001 is a critical error and must be turned on everywhere
- Nullable reference types are enabled and MUST be obeyed
- Do not back files up
- Aggressively pursue these aims, even when it means taking more time on a task

## Build, Test, and Development Commands

### Build the project
```bash
dotnet build
```

### Run tests
```bash
dotnet test
```

### Build in Release mode
```bash
dotnet build --configuration Release
```

### Run a specific test
```bash
dotnet test --filter FullyQualifiedName~TestClassName.TestMethodName
```

## Architecture Overview

RestClient.Net is a functional HTTP client library using Result types for error handling. Key components:

### Core Components

1. **HttpClientFactoryExtensions.cs** - Primary API. Extension methods on `IHttpClientFactory` that return delegates (e.g., `GetAsync<TSuccess, TError, TParam>`). Methods use `CreateClient()` with no parameters to leverage default HTTP client configuration.

2. **HttpClientExtensions.cs** - Direct extensions on `HttpClient` for when you already have an instance.

3. **Delegates.cs** - Core types:
   - `BuildRequest<TParam>` - Transforms parameters into `HttpRequestParts` (URL + body + headers)
   - `Deserialize<T>` - Deserializes HTTP responses
   - `GetAsync/PostAsync/PutAsync/DeleteAsync/PatchAsync` - Delegates returned by `Create*` methods

4. **Result Pattern** - From the Results F# project. Returns `Result<TSuccess, HttpError<TError>>` with exhaustive pattern matching.

5. **ProgressReportingHttpContent** - Custom `HttpContent` supporting upload/download progress callbacks.

### Code Generation

**RestClient.Net.OpenApiGenerator** - Generates C# extension methods from OpenAPI 3.x specs:
- **ExtensionMethodGenerator.cs** - Creates extension methods using the `Create*` delegate pattern
- **ModelGenerator.cs** - Generates DTOs from schema components
- Produces type aliases for convenient Result pattern matching (e.g., `OkUser`, `ErrorUser`)
- All generated code uses `factory.CreateClient()` with no client name

### Key Patterns

- **Functional composition** - `Create*` methods return delegates that can be composed/reused
- **HttpRequestParts bundling** - Single struct containing URL, body, and headers instead of separate parameters
- **IHttpClientFactory integration** - Proper lifecycle management without socket exhaustion
- **Zero exceptions** - All errors return as Result types