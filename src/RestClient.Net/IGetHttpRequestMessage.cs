using Microsoft.Extensions.Logging;
using RestClient.Net.Abstractions;
using System.Net.Http;

namespace RestClient.Net
{
    public interface IGetHttpRequestMessage
    {
        HttpRequestMessage GetHttpRequestMessage<T>(IRequest<T> request, ILogger logger);
    }
}


