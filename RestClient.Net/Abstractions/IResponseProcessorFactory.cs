using System;
using System.Threading;
using System.Threading.Tasks;

namespace RestClientDotNet.Abstractions
{
    public interface IResponseProcessorFactory
    {
        TimeSpan Timeout { get; set; }
        Uri BaseAddress { get; }

        Task<IResponseProcessor> GetResponseProcessor<TBody>(HttpVerb httpVerb, Uri BaseUri, Uri queryString, TBody body, string contentType, CancellationToken cancellationToken);
        void Dispose();
    }
}