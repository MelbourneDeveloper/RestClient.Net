using System;
using System.Threading.Tasks;

namespace RestClientDotNet.Abstractions
{
    public interface IResponseProcessor
    {
        Task<RestResponseBase<TReturn>> ProcessRestResponseAsync<TReturn>(Uri resource, HttpVerb httpVerb);
        IRestHeadersCollection DefaultRequestHeaders { get; }
    }
}