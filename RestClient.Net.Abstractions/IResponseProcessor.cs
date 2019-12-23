using System;
using System.Threading.Tasks;

namespace RestClientDotNet.Abstractions
{
    public interface IResponseProcessor
    {
        Task<RestResponse<TReturn>> ProcessRestResponseAsync<TReturn>(Uri baseUri, Uri resource, HttpVerb httpVerb);
        bool IsSuccess { get; }
        int StatusCode { get; }
        IRestHeadersCollection Headers { get; }
        object UnderlyingResponse { get; }
    }
}