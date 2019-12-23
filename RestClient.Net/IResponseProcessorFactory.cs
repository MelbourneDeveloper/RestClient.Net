using RestClientDotNet.Abstractions;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace RestClientDotNet
{
    public interface IResponseProcessorFactory
    {
        TimeSpan Timeout { get; set; }
        Uri BaseAddress { get; }
        IRestHeadersCollection DefaultRequestHeaders { get; }
        Task<IResponseProcessor> CreateResponseProcessorAsync<TBody>(HttpVerb httpVerb, Uri baseUri, Uri resource, TBody body, string contentType, CancellationToken cancellationToken);
        void Dispose();
        RestResponseBase<TReturn> CreateResponse<TReturn>(IRestHeadersCollection headers, int statusCode, IResponseProcessor responseProcessor, Uri baseUri, Uri resource, HttpVerb httpVerb, TReturn body);
    }
}