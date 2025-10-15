namespace RestClient.Net.CsTest.Fakes;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

// Helper classes
internal sealed class FakeHttpMessageHandler(
    Func<HttpRequestMessage, Task<HttpResponseMessage>> sendAsyncFunc,
    Exception? exception = null,
    Func<
        HttpRequestMessage,
        MultipartFormDataContent,
        CancellationToken,
        Task
    >? onMultipartUploadAvailable = null,
    Func<HttpRequestMessage, Stream, CancellationToken, Task>? onUploadStreamAvailable = null
) : HttpMessageHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken
    )
    {
        if (
            onMultipartUploadAvailable != null
            && request.Content is MultipartFormDataContent multipartContent
        )
        {
            await onMultipartUploadAvailable(request, multipartContent, cancellationToken)
                .ConfigureAwait(false);
        }
        else if (onUploadStreamAvailable != null && request.Content != null)
        {
            using var stream = await request
                .Content.ReadAsStreamAsync(cancellationToken)
                .ConfigureAwait(false);
            await onUploadStreamAvailable(request, stream, cancellationToken).ConfigureAwait(false);
        }

        return exception != null
            ? throw exception
            : await sendAsyncFunc(request).ConfigureAwait(false);
    }
}
