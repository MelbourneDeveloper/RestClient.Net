

using Microsoft.Extensions.Logging;
using RestClient.Net.Abstractions;
using RestClient.Net.Abstractions.Extensions;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace RestClient.Net
{
    public static class ClientExtensions
    {
        #region Public Methods

        public static Client With(this Client client, IHeadersCollection defaultRequestHeaders)
        =>
            client != null ? new Client(
            client.SerializationAdapter,
            client.Name,
            client.BaseUri,
            defaultRequestHeaders,
            client.logger is ILogger<Client> logger ? logger : null,
            client.createHttpClient,
            client.sendHttpRequestFunc,
            client.getHttpRequestMessage,
            client.Timeout,
            client.zip,
            client.ThrowExceptionOnFailure) : throw new ArgumentNullException(nameof(client));

        public static Client With(this Client client, string key, string value)
            => With(client, key.CreateHeadersCollection(value));

        #endregion Public Methods

        #region Internal Methods

        internal static HttpRequestMessage DefaultGetHttpRequestMessage(IRequest request, ILogger logger)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));

            try
            {
                logger.LogTrace("Converting Request to HttpRequestMethod... Event: {event} Request: {request}", TraceEvent.Information, request);

                var httpMethod = string.IsNullOrEmpty(request.CustomHttpRequestMethod)
                    ? request.HttpRequestMethod switch
                    {
                        HttpRequestMethod.Get => HttpMethod.Get,
                        HttpRequestMethod.Post => HttpMethod.Post,
                        HttpRequestMethod.Put => HttpMethod.Put,
                        HttpRequestMethod.Delete => HttpMethod.Delete,
                        HttpRequestMethod.Patch => new HttpMethod("PATCH"),
                        HttpRequestMethod.Custom => throw new NotImplementedException("CustomHttpRequestMethod must be specified for Custom Http Requests"),
                        _ => throw new NotImplementedException()
                    }
                    : new HttpMethod(request.CustomHttpRequestMethod);

                var httpRequestMessage = new HttpRequestMessage
                {
                    Method = httpMethod,
                    RequestUri = request.Uri
                };

                ByteArrayContent? httpContent = null;
                if (request.BodyData != null && request.BodyData.Length > 0)
                {
                    httpContent = new ByteArrayContent(request.BodyData);
                    httpRequestMessage.Content = httpContent;
                    logger.LogTrace("Request content was set. Length: {requestBodyLength}", request.BodyData.Length);
                }
                else
                {
                    logger.LogTrace("No request content set up on HttpRequestMessage");
                }

                if (request.Headers != null)
                {
                    foreach (var headerName in request.Headers.Names)
                    {
                        if (string.Compare(headerName, HeadersExtensions.ContentTypeHeaderName, StringComparison.OrdinalIgnoreCase) == 0)
                        {
                            //Note: not sure why this is necessary...
                            //The HttpClient class seems to differentiate between content headers and request message headers, but this distinction doesn't exist in the real world...
                            //TODO: Other Content headers
                            httpContent?.Headers.Add(HeadersExtensions.ContentTypeHeaderName, request.Headers[headerName]);
                        }
                        else
                        {
                            httpRequestMessage.Headers.Add(headerName, request.Headers[headerName]);
                        }
                    }

                    logger.LogTrace("Headers added to request");
                }

                logger.LogTrace("Successfully converted IRequest to HttpRequestMessage");

                return httpRequestMessage;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Exception on DefaultGetHttpRequestMessage Event: {event}", TraceEvent.Request);

                throw;
            }
        }

        internal static async Task<HttpResponseMessage> DefaultSendHttpRequestMessageFunc(HttpClient httpClient, GetHttpRequestMessage httpRequestMessageFunc, IRequest request, ILogger logger)
        {
            try
            {
                if (httpClient == null) throw new ArgumentNullException(nameof(httpClient));

                var httpRequestMessage = httpRequestMessageFunc(request, logger);

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

        #endregion Internal Methods
    }
}