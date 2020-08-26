using System;
using System.Threading;

namespace RestClient.Net.Abstractions
{
    public interface IRequest
    {
#pragma warning disable CA1819 // Properties should not return arrays
        byte[]? BodyData { get; }
#pragma warning restore CA1819 // Properties should not return arrays
        CancellationToken CancellationToken { get; set; }
        string? CustomHttpRequestMethod { get; set; }
        IHeadersCollection? Headers { get; set; }
        HttpRequestMethod HttpRequestMethod { get; set; }
        Uri? Resource { get; set; }
    }
}