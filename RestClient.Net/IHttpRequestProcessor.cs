using RestClientDotNet.Abstractions;
using System.Net.Http;
using System.Threading.Tasks;

namespace RestClientDotNet
{
    public interface IHttpRequestProcessor
    {
        HttpRequestMessage GetHttpRequestMessage<TRequestBody>(RestRequest<TRequestBody> restRequest, byte[] requestBodyData);
        Task<HttpResponseMessage> SendRestRequestAsync<TRequestBody>(HttpClient httpClient, RestRequest<TRequestBody> restRequest, byte[] requestBodyData);
    }
}
