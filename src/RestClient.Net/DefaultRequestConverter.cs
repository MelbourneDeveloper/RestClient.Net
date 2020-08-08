#if NET45
using RestClient.Net.Abstractions.Logging;
#else
using Microsoft.Extensions.Logging;
#endif

using RestClient.Net.Abstractions;
using RestClient.Net.Abstractions.Extensions;
using System;
using System.Collections.Generic;
using System.Net.Http;

#pragma warning disable CA2000

namespace RestClient.Net
{
    public class DefaultRequestConverter : IRequestConverter
    {
        #region Fields
        private readonly ILogger _logger;
        #endregion

        #region Public Methods
        public static readonly List<HttpRequestMethod> UpdateHttpRequestMethods = new List<HttpRequestMethod> { HttpRequestMethod.Put, HttpRequestMethod.Post, HttpRequestMethod.Patch };
        #endregion

        #region Constructor
        public DefaultRequestConverter(ILogger logger)
        {
            _logger = logger;
        }
        #endregion

        #region Implementation
        public virtual HttpRequestMessage GetHttpRequestMessage<TRequestBody>(Request<TRequestBody> request, byte[] requestBodyData)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));

            try
            {
                _logger?.LogTrace(new Trace(request.HttpRequestMethod, TraceEvent.Information, message: "Converting Request to HttpRequestMethod..."));

                HttpMethod httpMethod;
                if (string.IsNullOrEmpty(request.CustomHttpRequestMethod))
                {
                    switch (request.HttpRequestMethod)
                    {
                        case HttpRequestMethod.Get:
                            httpMethod = HttpMethod.Get;
                            break;
                        case HttpRequestMethod.Post:
                            httpMethod = HttpMethod.Post;
                            break;
                        case HttpRequestMethod.Put:
                            httpMethod = HttpMethod.Put;
                            break;
                        case HttpRequestMethod.Delete:
                            httpMethod = HttpMethod.Delete;
                            break;
                        case HttpRequestMethod.Patch:
                            httpMethod = new HttpMethod("PATCH");
                            break;
                        case HttpRequestMethod.Custom:
                            throw new NotImplementedException("CustomHttpRequestMethod must be specified for Custom Http Requests");
                        default:
                            throw new NotImplementedException();
                    }
                }
                else
                {
                    httpMethod = new HttpMethod(request.CustomHttpRequestMethod);
                }

                var httpRequestMessage = new HttpRequestMessage
                {
                    Method = httpMethod,
                    RequestUri = request.Resource
                };

                ByteArrayContent httpContent = null;
                if (requestBodyData != null && requestBodyData.Length > 0)
                {
                    httpContent = new ByteArrayContent(requestBodyData);
                    httpRequestMessage.Content = httpContent;
                    _logger?.LogTrace(new Trace(request.HttpRequestMethod, TraceEvent.Information, message: $"Request content was set. Length: {requestBodyData.Length}"));
                }
                else
                {
                    _logger?.LogTrace(new Trace(request.HttpRequestMethod, TraceEvent.Information, message: $"No request content setup on HttpRequestMessage"));
                }

                if (request.Headers != null)
                {
                    foreach (var headerName in request.Headers.Names)
                    {
                        if (string.Compare(headerName, MiscExtensions.ContentTypeHeaderName, StringComparison.OrdinalIgnoreCase) == 0)
                        {
                            //Note: not sure why this is necessary...
                            //The HttpClient class seems to differentiate between content headers and request message headers, but this distinction doesn't exist in the real world...
                            //TODO: Other Content headers
                            httpContent?.Headers.Add(MiscExtensions.ContentTypeHeaderName, request.Headers[headerName]);
                        }
                        else
                        {
                            httpRequestMessage.Headers.Add(headerName, request.Headers[headerName]);
                        }

                        _logger?.LogTrace(new Trace(request.HttpRequestMethod, TraceEvent.Information, message: $"Header: {headerName} processed"));
                    }
                }

                _logger?.LogTrace(new Trace(request.HttpRequestMethod, TraceEvent.Information, message: $"Successfully converted"));
                return httpRequestMessage;
            }
            catch (Exception ex)
            {
                _logger?.LogException(new Trace(
                request.HttpRequestMethod,
                TraceEvent.Error,
                request.Resource,
                message: $"Exception: {ex}"),
                ex);

                throw;
            }
        }
        #endregion
    }
}

#pragma warning restore CA2000