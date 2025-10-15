using System.Net;

namespace RestClient.Net.CsTest.Utilities;

/// <summary>
/// A custom HttpContent class that allows pushing data to the output stream asynchronously.
/// </summary>
internal sealed class PushStreamContent : HttpContent
{
    private readonly Func<Stream, HttpContent, TransportContext?, Task> _onStreamAvailable;
    private readonly string _mediaType;

    /// <summary>
    /// Initializes a new instance of the <see cref="PushStreamContent"/> class.
    /// </summary>
    /// <param name="onStreamAvailable">
    /// A delegate that writes data to the provided stream.
    /// </param>
    /// <param name="mediaType">The media type of the content.</param>
    public PushStreamContent(
        Func<Stream, HttpContent, TransportContext?, Task> onStreamAvailable,
        string mediaType
    )
    {
        _onStreamAvailable =
            onStreamAvailable ?? throw new ArgumentNullException(nameof(onStreamAvailable));
        _mediaType = mediaType ?? throw new ArgumentNullException(nameof(mediaType));
        Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(_mediaType);
    }

    /// <summary>
    /// Serializes the HTTP content to a stream as an asynchronous operation.
    /// </summary>
    /// <param name="stream">The target stream.</param>
    /// <param name="context">Transport information for the content.</param>
    /// <returns>A task that represents the asynchronous write operation.</returns>
    protected override async Task SerializeToStreamAsync(
        Stream stream,
        TransportContext? context
    ) => await _onStreamAvailable(stream, this, context).ConfigureAwait(false);

    /// <summary>
    /// Determines whether the content length can be calculated in advance.
    /// </summary>
    /// <param name="length">The length of the content.</param>
    /// <returns>
    /// Always returns false because the length cannot be determined in advance.
    /// </returns>
    protected override bool TryComputeLength(out long length)
    {
        length = -1; // Length cannot be determined.
        return false;
    }
}
