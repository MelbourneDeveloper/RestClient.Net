using System;
using System.Threading;
using System.Threading.Tasks;

namespace RestClientDotNet.Abstractions
{
    public interface IResponseProcessor
    {
        Task<RestResponseBase<TReturn>> ProcessRestResponseAsync<TReturn, TBody>(Uri resource, HttpVerb httpVerb, TBody body, string contentType, CancellationToken cancellationToken);
        IRestHeadersCollection DefaultRequestHeaders { get; }
        TimeSpan Timeout { get; set; }
    }
}