using System.Threading;
using Uris;

namespace RestClient.Net.Abstractions
{
    public interface IRequest
    {
        CancellationToken CancellationToken { get; }
        string? CustomHttpRequestMethod { get; }
        IHeadersCollection Headers { get; }
        HttpRequestMethod HttpRequestMethod { get; }
        AbsoluteUri Uri { get; }
    }

    public interface IRequest<TBody> : IRequest
    {
        TBody? BodyData { get; }
    }
}