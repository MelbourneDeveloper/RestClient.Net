# RestClient.Net AvaloniaUI Sample

A beautiful cross-platform desktop application showcasing **RestClient.Net** HTTP operations using the **Result pattern** for functional error handling.

## Features

This sample demonstrates all major HTTP operations using the JSONPlaceholder API:

- **GET** - Load todos and posts
- **POST** - Create new todos and posts
- **PUT** - Update existing items
- **PATCH** - Partial updates
- **DELETE** - Remove items

## Key Highlights

✨ **Functional Error Handling** - Uses the Result pattern to handle success/failure cases without exceptions

✨ **Strongly-Typed Responses** - Type-safe deserialization for both success and error responses

✨ **Modern MVVM Architecture** - Built with CommunityToolkit.Mvvm for clean separation of concerns

✨ **Dependency Injection** - Uses Microsoft.Extensions.Http for proper HttpClient lifecycle management

✨ **Beautiful UI** - Clean, modern interface with status indicators and progress feedback

## Running the Sample

```bash
cd Samples/RestClient.AvaloniaUI.Sample
dotnet run
```

## How It Works

The sample uses `HttpClientExtensions` from RestClient.Net to perform HTTP operations:

```csharp
// GET example
var result = await _httpClient.GetAsync<List<Todo>, ApiError>(
    url,
    DeserializeTodos,
    DeserializeError,
    cancellationToken
);

// POST example
var result = await _httpClient.PostAsync<Todo, ApiError>(
    url,
    requestBody,
    DeserializeTodo,
    DeserializeError,
    cancellationToken
);

// Handle results with pattern matching
result.Match(
    ok: data => HandleSuccess(data),
    error: error => HandleError(error)
);
```

## Architecture

- **Models/** - Data models (Todo, Post, ApiError)
- **Services/** - API service layer using HttpClientExtensions
- **ViewModels/** - MVVM view models with commands and observable properties
- **Views/** - Avalonia XAML UI
- **Converters/** - Value converters for UI bindings

## Technologies Used

- **AvaloniaUI** - Cross-platform UI framework
- **RestClient.Net** - HTTP client with Result pattern
- **CommunityToolkit.Mvvm** - MVVM helpers and source generators
- **Microsoft.Extensions.Http** - HttpClient factory pattern
- **JSONPlaceholder** - Free fake REST API for testing
