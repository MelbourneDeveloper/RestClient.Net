using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace RestClient.Net
{
    public class DefaultSendHttpRequestMessage : ISendHttpRequestMessage
    {
        public static DefaultSendHttpRequestMessage Instance { get; } = new DefaultSendHttpRequestMessage();

        public async Task<HttpResponseMessage> SendHttpRequestMessage<TRequestBody>(
            HttpClient httpClient,
            IGetHttpRequestMessage httpRequestMessageFunc,
            IRequest<TRequestBody> request,
            ILogger logger,
            ISerializationAdapter serializationAdapter)
        {
            if (httpClient == null) throw new ArgumentNullException(nameof(httpClient));
            if (httpRequestMessageFunc == null) throw new ArgumentNullException(nameof(httpRequestMessageFunc));
            if (request == default) throw new ArgumentNullException(nameof(request));

            logger ??= NullLogger.Instance;

            try
            {
                var httpRequestMessage = httpRequestMessageFunc.GetHttpRequestMessage(request, logger, serializationAdapter);

                logger.LogTrace(Messages.InfoAttemptingToSend, request);


                try
                {
                    var httpResponseMessage = await httpClient.SendAsync(httpRequestMessage, request.CancellationToken).ConfigureAwait(false);

                    logger.LogInformation(Messages.InfoSendReturnedNoException);

                    return httpResponseMessage;

                }
                catch (ArgumentException aex)
                {
                    if (aex.Message == "The value cannot be null or empty. (Parameter 'mediaType')")
                    {
                        var isRequest = httpRequestMessage.Content?.Headers.ContentType == null;
                        var errorTypePart = isRequest ? $"The request is missing the {HeadersExtensions.ContentTypeHeaderName} header" :
                            $"The request has the {HeadersExtensions.ContentTypeHeaderName} header so perhaps the response doesn't include it";
                        throw
                            new MissingHeaderException(
                                $"The media type header is missing. {errorTypePart}",
                                isRequest,
                                aex); ;
                    }
                    throw;
                }
            }
            catch (OperationCanceledException oce)
            {
                logger.LogError(oce, Messages.ErrorMessageOperationCancelled, request);
                throw;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, Messages.ErrorOnSend, request);

                throw;
            }
        }
    }
}