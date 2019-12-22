using System;
using System.Threading;
using System.Threading.Tasks;

namespace RestClientDotNet.Abstractions
{
    public interface IRestClient
    {
        Task<RestResponse<TReturn>> SendAsync<TReturn, TBody>(Uri queryString, HttpVerb httpVerb, string contentType, TBody body, CancellationToken cancellationToken);
        IRestHeadersCollection DefaultRequestHeaders { get; }
        string DefaultContentType { get; }
        TimeSpan Timeout { get; set; }
    }
}