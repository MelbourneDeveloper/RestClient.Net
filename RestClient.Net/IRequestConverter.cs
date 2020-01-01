using RestClient.Net.Abstractions;
using System.Net.Http;

namespace RestClient.Net
{
    public interface IRequestConverter
    {
        HttpRequestMessage GetHttpRequestMessage<TRequestBody>(Request<TRequestBody> request, byte[] requestBodyData);
    }
}
