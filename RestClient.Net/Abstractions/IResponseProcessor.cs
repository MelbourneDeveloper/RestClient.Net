using System;
using System.Threading.Tasks;

namespace RestClientDotNet.Abstractions
{
    public interface IResponseProcessor
    {
        Task<RestResponse<TReturn>> GetRestResponse<TReturn>(Uri baseUri, Uri queryString, HttpVerb httpVerb);
    }
}