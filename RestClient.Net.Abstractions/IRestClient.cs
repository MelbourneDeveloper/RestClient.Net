using System;
using System.Threading;
using System.Threading.Tasks;

namespace RestClientDotNet.Abstractions
{
    public interface IRestClient
    {
        Task<IRestResponse<TReturn>> SendAsync<TReturn, TBody>(Uri resource, HttpVerb httpVerb, string contentType, TBody body, CancellationToken cancellationToken);
        IRestHeadersCollection DefaultRequestHeaders { get; }
        string DefaultContentType { get; }
        TimeSpan Timeout { get; set; }
    }
}