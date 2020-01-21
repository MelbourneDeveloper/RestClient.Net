using RestClient.Net.Abstractions;
using System;
using System.Net.Http;

namespace RestClient.Net
{
    public class HttpResponseMessageResponse<TResponseBody> : Response<TResponseBody>
    {
        #region Public Properties
        public HttpResponseMessage HttpResponseMessage { get; }
        public override bool IsSuccess => HttpResponseMessage.IsSuccessStatusCode;
        public HttpClient HttpClient { get; }
        #endregion

        #region Constructor
        public HttpResponseMessageResponse
        (
            HttpResponseHeadersCollection httpResponseHeadersCollection,
            int statusCode,
            HttpRequestMethod httpRequestMethod,
            byte[] responseContentData,
            TResponseBody body,
            HttpResponseMessage httpResponseMessage,
            HttpClient httpClient
            ) : base(
                httpResponseHeadersCollection,
                statusCode,
                httpRequestMethod,
                responseContentData,
                body,
                httpResponseMessage != null ? httpResponseMessage.RequestMessage.RequestUri : throw new ArgumentNullException(nameof(httpResponseMessage)))
        {
            HttpResponseMessage = httpResponseMessage;
            HttpClient = httpClient;
        }
        #endregion
    }
}
