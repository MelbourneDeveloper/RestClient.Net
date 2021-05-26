using Microsoft.Extensions.Logging;
using System.Net.Http;

namespace RestClient.Net
{
    public interface IGetHttpRequestMessage
    {
        HttpRequestMessage GetHttpRequestMessage<T>(IRequest<T> request, ILogger logger, ISerializationAdapter serializationAdapter);
    }
}
