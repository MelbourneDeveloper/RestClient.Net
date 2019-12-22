using System;
using System.Threading.Tasks;

namespace RestClientDotNet.Abstractions
{
    public interface IResponseProcessor
    {
        Task<RestResponse<TReturn>> ProcessRestResponseAsync<TReturn>(Uri baseUri, Uri queryString, HttpVerb httpVerb);
        bool IsSuccess { get; }
        int StatusCode { get; }
        IRestHeadersCollection Headers { get; }
        object UnderlyingResponseMessage { get; }
    }
}