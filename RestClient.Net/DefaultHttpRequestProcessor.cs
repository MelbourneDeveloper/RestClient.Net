using RestClientDotNet.Abstractions;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

#pragma warning disable CA2000

namespace RestClientDotNet
{
    public class DefaultHttpRequestProcessor : IHttpRequestProcessor
    {
        public static readonly List<HttpVerb> UpdateVerbs = new List<HttpVerb> { HttpVerb.Put, HttpVerb.Post, HttpVerb.Patch };

        public virtual HttpRequestMessage GetHttpRequestMessage<TRequestBody>(RestRequest<TRequestBody> restRequest, byte[] requestBodyData)
        {
            if (restRequest == null) throw new ArgumentNullException(nameof(restRequest));

            HttpMethod httpMethod;
            switch (restRequest.HttpVerb)
            {
                case HttpVerb.Get:
                    httpMethod = HttpMethod.Get;
                    break;
                case HttpVerb.Post:
                    httpMethod = HttpMethod.Post;
                    break;
                case HttpVerb.Put:
                    httpMethod = HttpMethod.Put;
                    break;
                case HttpVerb.Delete:
                    httpMethod = HttpMethod.Delete;
                    break;
                case HttpVerb.Patch:
                    httpMethod = new HttpMethod("PATCH");
                    break;
                default:
                    throw new NotImplementedException();
            }

            var httpRequestMessage = new HttpRequestMessage
            {
                Method = httpMethod,
                RequestUri = restRequest.Resource,
            };

            ByteArrayContent httpContent = null;
            if (UpdateVerbs.Contains(restRequest.HttpVerb))
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

        public virtual Task<HttpResponseMessage> SendRestRequestAsync<TRequestBody>(HttpClient httpClient, RestRequest<TRequestBody> restRequest, byte[] requestBodyData)
        {
            if (restRequest == null) throw new ArgumentNullException(nameof(restRequest));
            if (httpClient == null) throw new ArgumentNullException(nameof(httpClient));

            var httpRequestMessage = GetHttpRequestMessage(restRequest, requestBodyData);

            return httpClient.SendAsync(httpRequestMessage, restRequest.CancellationToken);
        }
    }
}

#pragma warning restore CA2000