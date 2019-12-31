using RestClientDotNet.Abstractions;
using System;
using System.Collections.Generic;
using System.Net.Http;

#pragma warning disable CA2000

namespace RestClientDotNet
{
    public class DefaultRestRequestConverter : IRestRequestConverter
    {
        #region Public Methods
        public static readonly List<HttpRequestMethod> UpdateHttpRequestMethods = new List<HttpRequestMethod> { HttpRequestMethod.Put, HttpRequestMethod.Post, HttpRequestMethod.Patch };
        #endregion

        #region Implementation
        public virtual HttpRequestMessage GetHttpRequestMessage<TRequestBody>(Request<TRequestBody> restRequest, byte[] requestBodyData)
        {
            if (restRequest == null) throw new ArgumentNullException(nameof(restRequest));

            HttpMethod httpMethod;
            switch (restRequest.HttpRequestMethod)
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
                default:
                    throw new NotImplementedException();
            }

            var httpRequestMessage = new HttpRequestMessage
            {
                Method = httpMethod,
                RequestUri = restRequest.Resource
            };

            ByteArrayContent httpContent = null;
            if (UpdateHttpRequestMethods.Contains(restRequest.HttpRequestMethod))
            {
                httpContent = new ByteArrayContent(requestBodyData);
                httpRequestMessage.Content = httpContent;
            }

            foreach (var headerName in restRequest.Headers.Names)
            {
                if (string.Compare(headerName, "Content-Type", StringComparison.OrdinalIgnoreCase) == 0)
                {
                    //Note: not sure why this is necessary...
                    //The HttpClient class seems to differentiate between content headers and request message headers, but this distinction doesn't exist in the real world...
                    //TODO: Other Content headers
                    httpContent?.Headers.Add("Content-Type", restRequest.Headers[headerName]);
                }
                else
                {
                    httpRequestMessage.Headers.Add(headerName, restRequest.Headers[headerName]);
                }
            }

            return httpRequestMessage;
        }
        #endregion
    }
}

#pragma warning restore CA2000