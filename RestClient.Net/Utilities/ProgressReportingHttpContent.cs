using System.Net;
using System.Text;

namespace RestClient.Net.Utilities;

/// <summary>
/// Represents an HTTP content type that reports progress during upload and download operations.
/// </summary>
public class ProgressReportingHttpContent : HttpContent
{
    private const int DefaultBufferSize = 8192;

#pragma warning disable CA2213 // Disposable fields should be disposed
    private readonly Stream _content;
#pragma warning restore CA2213 // Disposable fields should be disposed
    private readonly int _bufferSize;
    private readonly Action<long, long>? _progress;
    private readonly long _contentLength;

    /// <summary>
    /// Initializes a new instance of the <see cref="ProgressReportingHttpContent"/> class using a string as content.
    /// </summary>
    /// <param name="data">The string data to be sent, which will be converted to bytes using UTF8 encoding.</param>
    /// <param name="bufferSize">The size of the buffer used for reading the content. Default is 8192 bytes.</param>
    /// <param name="progress">Optional callback to report progress during transfer. Receives current bytes and total bytes.</param>
    /// <param name="contentType">The MIME type of the content. Defaults to "application/octet-stream".</param>
    public ProgressReportingHttpContent(
        string data,
        int bufferSize = DefaultBufferSize,
        Action<long, long>? progress = null,
        string contentType = "application/octet-stream"
    )
        : this(
            content: new MemoryStream(Encoding.UTF8.GetBytes(data)),
            progress: progress,
            bufferSize: bufferSize,
            contentType: contentType
        ) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="ProgressReportingHttpContent"/> class using a byte array as content.
    /// </summary>
    /// <param name="data">The byte array containing the content to be sent.</param>
    /// <param name="bufferSize">The size of the buffer used for reading the content. Default is 8192 bytes.</param>
    /// <param name="progress">Optional callback to report progress during transfer. Receives current bytes and total bytes.</param>
    /// <param name="contentType">The MIME type of the content. Defaults to "application/octet-stream".</param>
    public ProgressReportingHttpContent(
        byte[] data,
        int bufferSize = DefaultBufferSize,
        Action<long, long>? progress = null,
        string contentType = "application/octet-stream"
    )
        : this(
            content: new MemoryStream(data),
            progress: progress,
            bufferSize: bufferSize,
            contentType: contentType
        ) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="ProgressReportingHttpContent"/> class using a stream as content.
    /// </summary>
    /// <param name="content">The stream containing the content to be sent.</param>
    /// <param name="bufferSize">The size of the buffer used for reading the content. Default is 8192 bytes.</param>
    /// <param name="contentType">The MIME type of the content. Defaults to "application/octet-stream".</param>
    /// <param name="progress">Optional callback to report progress during transfer. Receives current bytes and total bytes.</param>
    public ProgressReportingHttpContent(
        Stream content,
        int bufferSize = DefaultBufferSize,
        string contentType = "application/octet-stream",
        Action<long, long>? progress = null
    )
    {
        _content = content;
        _bufferSize = bufferSize;
        _progress = progress;
        _contentLength = content.Length;
        Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(contentType);
    }

    /// <summary>
    /// Serializes the HTTP content to a stream asynchronously while reporting progress.
    /// </summary>
    /// <param name="stream">The target stream to write the content to.</param>
    /// <param name="context">The transport context (unused).</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    protected override async Task SerializeToStreamAsync(Stream stream, TransportContext? context)
    {
        var buffer = new byte[_bufferSize];
        long totalBytesRead = 0;
        int bytesRead;

        while ((bytesRead = await _content.ReadAsync(buffer).ConfigureAwait(false)) != 0)
        {
            await stream.WriteAsync(buffer.AsMemory(0, bytesRead)).ConfigureAwait(false);
            totalBytesRead += bytesRead;
            _progress?.Invoke(totalBytesRead, _contentLength);
        }
    }

    /// <summary>
    /// Computes the length of the content if possible.
    /// </summary>
    /// <param name="length">When this method returns, contains the computed length of the content, if available.</param>
    /// <returns>true if the length was computed successfully; otherwise, false.</returns>
    protected override bool TryComputeLength(out long length)
    {
        length = _contentLength;
        return true;
    }

    /// <summary>
    /// Releases the unmanaged resources used by the <see cref="ProgressReportingHttpContent"/> and optionally disposes of the managed resources.
    /// </summary>
    public new void Dispose()
    {
        _content.Dispose();
        base.Dispose();
    }
}
