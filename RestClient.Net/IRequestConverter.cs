using RestClient.Net.Abstractions;
using System.Net.Http;

namespace RestClient.Net
{
    /// <summary>
    /// Abstraction responsible for converting rest requests with data in to HttpRequestMessages
    /// </summary>
    public interface IRequestConverter
    {
        /// <summary>
        /// Convert rest request with data in to HttpRequestMessages
        /// </summary>
        HttpRequestMessage GetHttpRequestMessage<TRequestBody>(Request<TRequestBody> request, byte[] requestBodyData);
    }
}
