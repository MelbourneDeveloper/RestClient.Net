using System;
using System.Threading;
using System.Threading.Tasks;

namespace RestClientDotNet.Abstractions
{
    public interface IResponseProcessorFactory : IDisposable
    {
        TimeSpan Timeout { get; set; }
        Uri BaseAddress { get; }
        IRestHeadersCollection DefaultRequestHeaders { get; }
        Task<IResponseProcessor> CreateResponseProcessorAsync<TBody>(HttpVerb httpVerb, Uri baseUri, Uri resource, TBody body, string contentType, CancellationToken cancellationToken);
    }
}