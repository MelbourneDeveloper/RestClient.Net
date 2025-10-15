using System.Net;
using System.Text;
using RestClient.Net.CsTest.Utilities;

namespace RestClient.Net.CsTest.Fakes;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

// Helper classes
internal sealed class FakeHttpMessageHandlerStreaming(
    string chunkedData,
    Func<HttpRequestMessage, Task<HttpResponseMessage>> sendAsyncFunc,
    Action<HttpRequestMessage>? onRequestSent = null
) : HttpMessageHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken
    )
    {
        onRequestSent?.Invoke(request);

        using var response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new PushStreamContent(
                async (stream, content, context) =>
                {
                    var data = Encoding.UTF8.GetBytes(chunkedData);
                    foreach (var chunk in data.SplitIntoChunks(6))
                    {
                        await stream.WriteAsync(chunk, cancellationToken).ConfigureAwait(false);
                        await stream.FlushAsync(cancellationToken).ConfigureAwait(false);
                        await Task.Delay(100, cancellationToken).ConfigureAwait(false);
                    }
                    stream.Close();
                },
                "application/octet-stream"
            ),
        };

        return await sendAsyncFunc(request).ConfigureAwait(false);
    }
}
