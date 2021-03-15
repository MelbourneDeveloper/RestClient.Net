

using Microsoft.Extensions.Logging;
using RestClient.Net.Abstractions;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace RestClient.Net
{
    public class DefaultSendHttpRequestMessage : ISendHttpRequestMessage
    {
        public static DefaultSendHttpRequestMessage Instance { get; } = new DefaultSendHttpRequestMessage();

        public async Task<HttpResponseMessage> SendHttpRequestMessage<TRequestBody>(HttpClient httpClient, Abstractions.IGetHttpRequestMessage httpRequestMessageFunc, IRequest<TRequestBody> request, ILogger logger)
        {
            try
            {
                if (httpClient == null) throw new ArgumentNullException(nameof(httpClient));
                if (httpRequestMessageFunc == null) throw new ArgumentNullException(nameof(httpRequestMessageFunc));
                if (request == default) throw new ArgumentNullException(nameof(request));

                var httpRequestMessage = httpRequestMessageFunc.GetHttpRequestMessage(request, logger);

                logger.LogTrace(Messages.InfoAttemptingToSend, request);

                var httpResponseMessage = await httpClient.SendAsync(httpRequestMessage, request.CancellationToken).ConfigureAwait(false);

                logger.LogInformation(Messages.InfoSendReturnedNoException);

                return httpResponseMessage;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, Messages.ErrorOnSend, request);

                throw;
            }
        }

    }
}