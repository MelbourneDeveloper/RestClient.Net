using System.Threading;
using Urls;

namespace RestClient.Net
{
    public interface IRequest
    {
        CancellationToken CancellationToken { get; }
        string? CustomHttpRequestMethod { get; }
        IHeadersCollection Headers { get; }
        HttpRequestMethod HttpRequestMethod { get; }
        AbsoluteUrl Uri { get; }
    }

    public interface IRequest<TBody> : IRequest
    {
        TBody? BodyData { get; }
    }
}