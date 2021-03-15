using System;
using System.Threading;

namespace RestClient.Net.Abstractions
{
    public interface IRequest<TBody>
    {
#pragma warning disable CA1819 // Properties should not return arrays
        TBody? BodyData { get; }
#pragma warning restore CA1819 // Properties should not return arrays
        CancellationToken CancellationToken { get; }
        string? CustomHttpRequestMethod { get; }
        IHeadersCollection? Headers { get; }
        HttpRequestMethod HttpRequestMethod { get; }
        Uri? Uri { get; }
    }
}