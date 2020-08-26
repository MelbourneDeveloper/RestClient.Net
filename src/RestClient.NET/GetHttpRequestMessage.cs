using RestClient.Net.Abstractions;
using System.Net.Http;

namespace RestClient.Net
{
    public delegate HttpRequestMessage GetHttpRequestMessage(IRequest request);
}
