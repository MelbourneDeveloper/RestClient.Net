using RestClientDotNet.Abstractions;
using System.Net.Http;

namespace RestClientDotNet
{
    public interface IHttpRequestProcessor
    {
        HttpRequestMessage GetHttpRequestMessage<TRequestBody>(RestRequest<TRequestBody> restRequest, byte[] requestBodyData);
    }
}
