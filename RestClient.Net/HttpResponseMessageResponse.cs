using RestClientDotNet.Abstractions;
using System;
using System.Net.Http;

namespace RestClientDotNet
{
    public class HttpResponseMessageResponse<TResponseBody> : Response<TResponseBody>
    {
        #region Public Properties
        public HttpResponseMessage HttpResponseMessage { get; }
        public override bool IsSuccess => HttpResponseMessage.IsSuccessStatusCode;
        #endregion

        #region Constructor
        public HttpResponseMessageResponse
        (
            IHeadersCollection restHeadersCollection,
            int statusCode,
            HttpRequestMethod HttpRequestMethod,
            byte[] responseContentData,
            TResponseBody body,
            HttpResponseMessage httpResponseMessage
            ) : base(
                restHeadersCollection,
                statusCode,
                HttpRequestMethod,
                responseContentData,
                body,
                httpResponseMessage != null ? httpResponseMessage.RequestMessage.RequestUri : throw new ArgumentNullException(nameof(httpResponseMessage)))
        {
            HttpResponseMessage = httpResponseMessage;
        }
        #endregion
    }
}
