using System;
using System.Threading;

namespace RestClient.Net.Abstractions
{
    public interface IRequest
    {
#pragma warning disable CA1819 // Properties should not return arrays
        byte[]? BodyData { get; }
#pragma warning restore CA1819 // Properties should not return arrays
        CancellationToken CancellationToken { get; }
        string? CustomHttpRequestMethod { get; }
        IHeadersCollection? Headers { get; }
        HttpRequestMethod HttpRequestMethod { get; }
        Uri Uri { get; }
    }
}