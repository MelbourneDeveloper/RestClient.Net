---
layout: layouts/docs.njk
title: Utilities
eleventyNavigation:
  key: Utilities
  parent: API Reference
  order: 4
---

# Utilities

Helper classes and utilities provided by RestClient.Net.

**Namespace:** `RestClient.Net.Utilities`

## ProgressReportingHttpContent

An `HttpContent` implementation that reports progress during upload operations.

```csharp
public class ProgressReportingHttpContent : HttpContent
```

### Purpose

Use this class when you need to:
- Track upload progress for large files
- Display progress bars or indicators to users
- Monitor bandwidth usage

### Constructors

#### From String

```csharp
public ProgressReportingHttpContent(
    string data,
    int bufferSize = 8192,
    Action<long, long>? progress = null,
    string contentType = "application/octet-stream"
)
```

#### From Byte Array

```csharp
public ProgressReportingHttpContent(
    byte[] data,
    int bufferSize = 8192,
    Action<long, long>? progress = null,
    string contentType = "application/octet-stream"
)
```

#### From Stream

```csharp
public ProgressReportingHttpContent(
    Stream content,
    int bufferSize = 8192,
    string contentType = "application/octet-stream",
    Action<long, long>? progress = null
)
```

### Parameters

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `data`/`content` | `string`/`byte[]`/`Stream` | - | The content to upload |
| `bufferSize` | `int` | 8192 | Buffer size for reading content |
| `progress` | `Action<long, long>?` | null | Callback receiving (bytesTransferred, totalBytes) |
| `contentType` | `string` | "application/octet-stream" | MIME type of the content |

### Progress Callback

The progress callback receives two parameters:
1. `bytesTransferred` - Number of bytes sent so far
2. `totalBytes` - Total number of bytes to send

### Example: File Upload with Progress

```csharp
using var fileStream = File.OpenRead("large-file.zip");

var content = new ProgressReportingHttpContent(
    content: fileStream,
    bufferSize: 65536, // 64KB buffer for better performance
    contentType: "application/zip",
    progress: (current, total) =>
    {
        var percentage = (double)current / total * 100;
        Console.WriteLine($"Upload progress: {percentage:F1}% ({current}/{total} bytes)");
    }
);

var result = await httpClient.UploadFileAsync(
    url: "https://api.example.com/upload".ToAbsoluteUrl(),
    requestBody: content,
    deserializeSuccess: DeserializeJson<UploadResponse>,
    deserializeError: DeserializeJson<ApiError>
);
```

### Example: Progress Bar in Console

```csharp
var content = new ProgressReportingHttpContent(
    content: fileStream,
    progress: (current, total) =>
    {
        var percentage = (int)((double)current / total * 100);
        var progressBar = new string('#', percentage / 2) + new string('-', 50 - percentage / 2);
        Console.Write($"\r[{progressBar}] {percentage}%");
    }
);
```

### Example: Progress with Blazor

```csharp
@code {
    private double _uploadProgress;

    private async Task UploadFileAsync(IBrowserFile file)
    {
        using var stream = file.OpenReadStream(maxAllowedSize: 100 * 1024 * 1024);

        var content = new ProgressReportingHttpContent(
            content: stream,
            contentType: file.ContentType,
            progress: (current, total) =>
            {
                _uploadProgress = (double)current / total * 100;
                InvokeAsync(StateHasChanged);
            }
        );

        var result = await Http.UploadFileAsync(
            url: "https://api.example.com/upload".ToAbsoluteUrl(),
            requestBody: content,
            deserializeSuccess: DeserializeJson<UploadResponse>,
            deserializeError: DeserializeJson<ApiError>
        );
    }
}

<MudProgressLinear Value="@_uploadProgress" />
```

### Methods

#### SerializeToStreamAsync

```csharp
protected override async Task SerializeToStreamAsync(
    Stream stream,
    TransportContext? context
)
```

Writes the content to the stream while reporting progress.

#### TryComputeLength

```csharp
protected override bool TryComputeLength(out long length)
```

Returns the content length. Always returns `true` with the actual length.

#### Dispose

```csharp
public new void Dispose()
```

Disposes the underlying stream and base content.

---

## Best Practices

### Buffer Size Selection

| File Size | Recommended Buffer Size |
|-----------|------------------------|
| < 1 MB | 8192 (8 KB) - default |
| 1-10 MB | 32768 (32 KB) |
| 10-100 MB | 65536 (64 KB) |
| > 100 MB | 131072 (128 KB) |

Larger buffers reduce the number of I/O operations but use more memory.

### Progress Update Throttling

For very large files, you may want to throttle progress updates:

```csharp
var lastUpdate = DateTime.MinValue;

var content = new ProgressReportingHttpContent(
    content: fileStream,
    progress: (current, total) =>
    {
        var now = DateTime.UtcNow;
        if ((now - lastUpdate).TotalMilliseconds > 100) // Update every 100ms max
        {
            lastUpdate = now;
            UpdateProgressUI(current, total);
        }
    }
);
```

### Memory Considerations

- For string content: The entire string is loaded into memory
- For byte array content: The entire array is loaded into memory
- For stream content: Only the buffer size is kept in memory at a time

Use stream-based constructors for large files to minimize memory usage.
