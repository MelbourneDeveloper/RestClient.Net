using System;
using System.Threading;
using System.Threading.Tasks;

namespace RestClientDotNet.Abstractions
{
    public interface IResponseProcessorFactory
    {
        Task<IResponseProcessor> GetResponseProcessor<TBody>(HttpVerb httpVerb, Uri BaseUri, Uri queryString, TBody body, string contentType, IRestHeadersCollection defaultRequestHeaders, CancellationToken cancellationToken);
    }
}