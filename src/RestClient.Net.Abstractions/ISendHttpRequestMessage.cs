﻿using Microsoft.Extensions.Logging;
using System.Net.Http;
using System.Threading.Tasks;

namespace RestClient.Net
{
    public interface ISendHttpRequestMessage
    {
        Task<HttpResponseMessage> SendHttpRequestMessage<TRequestBody>(
            HttpClient httpClient,
            IGetHttpRequestMessage httpRequestMessageFunc,
            IRequest<TRequestBody> request,
            ILogger logger,
            ISerializationAdapter serializationAdapter);
    }
}