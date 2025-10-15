namespace RestClient.Net;

#pragma warning disable CA2016 // Forward the 'CancellationToken' parameter to methods

/// <summary>
/// Polyfill extensions for .NET Standard 2.1 compatibility.
/// </summary>
public static class Pollyfill
{
#if NETSTANDARD2_1
    /// <summary>
    /// Reads HTTP content as a stream with cancellation token support for .NET Standard 2.1.
    /// </summary>
    /// <param name="content">The HTTP content to read.</param>
    /// <param name="cancellationToken">Cancellation token to cancel the operation.</param>
    /// <returns>A task representing the stream.</returns>
    public static async Task<Stream> ReadAsStreamAsync(
        this HttpContent content,
        CancellationToken cancellationToken
    )
    {
        var readTask = content.ReadAsStreamAsync();

        var tcs = new TaskCompletionSource<Stream>();

        using (cancellationToken.Register(() => tcs.TrySetCanceled()))
        {
            var completedTask = await Task.WhenAny(readTask, tcs.Task).ConfigureAwait(false);
            return await completedTask.ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Reads stream reader to end with cancellation token support for .NET Standard 2.1.
    /// </summary>
    /// <param name="reader">The stream reader to read from.</param>
    /// <param name="cancellationToken">Cancellation token to cancel the operation.</param>
    /// <returns>A task representing the string content.</returns>
    public static async Task<string> ReadToEndAsync(
        this StreamReader reader,
        CancellationToken cancellationToken
    )
    {
        var readTask = reader.ReadToEndAsync();
        var tcs = new TaskCompletionSource<string>();

        using (cancellationToken.Register(() => tcs.TrySetCanceled()))
        {
            var completedTask = await Task.WhenAny(readTask, tcs.Task).ConfigureAwait(false);
            return await completedTask.ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Copies HTTP content to a stream with cancellation token support for .NET Standard 2.1.
    /// </summary>
    /// <param name="content">The HTTP content to copy.</param>
    /// <param name="destinationStream">The destination stream.</param>
    /// <param name="cancellationToken">Cancellation token to cancel the operation.</param>
    /// <returns>A task representing the copy operation.</returns>
    public static async Task CopyToAsync(
        this HttpContent content,
        Stream destinationStream,
        CancellationToken cancellationToken
    )
    {
        var copyTask = content.CopyToAsync(destinationStream);
        var tcs = new TaskCompletionSource<bool>();

        using (cancellationToken.Register(() => tcs.TrySetCanceled()))
        {
            var completedTask = await Task.WhenAny(copyTask, tcs.Task).ConfigureAwait(false);
            await completedTask.ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Reads HTTP content as a string with cancellation token support for .NET Standard 2.1.
    /// </summary>
    /// <param name="content">The HTTP content to read.</param>
    /// <param name="cancellationToken">Cancellation token to cancel the operation.</param>
    /// <returns>A task representing the string content.</returns>
    public static async Task<string> ReadAsStringAsync(
        this HttpContent content,
        CancellationToken cancellationToken
    )
    {
        var readTask = content.ReadAsStringAsync();
        var tcs = new TaskCompletionSource<string>();

        using (cancellationToken.Register(() => tcs.TrySetCanceled()))
        {
            var completedTask = await Task.WhenAny(readTask, tcs.Task).ConfigureAwait(false);
            return await completedTask.ConfigureAwait(false);
        }
    }
#endif
}
