

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using RestClient.Net.Abstractions;
using RestClient.Net.Abstractions.Extensions;
using System;
using System.Net.Http;

namespace RestClient.Net
{
    public class DefaultGetHttpRequestMessage : IGetHttpRequestMessage
    {
        public static DefaultGetHttpRequestMessage Instance { get; } = new DefaultGetHttpRequestMessage();

        public HttpRequestMessage GetHttpRequestMessage<T>(IRequest<T> request, ILogger logger, ISerializationAdapter serializationAdapter)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));
            if (serializationAdapter == null) throw new ArgumentNullException(nameof(serializationAdapter));

            logger ??= NullLogger.Instance;

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
                if (request.BodyData != null)
                {
                    var bodyDataData = serializationAdapter.Serialize(request.BodyData, request.Headers);
                    httpContent = new ByteArrayContent(bodyDataData);
                    httpRequestMessage.Content = httpContent;
                    logger.LogTrace("Request content was set. Length: {requestBodyLength}", bodyDataData.Length);
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
    }
}
