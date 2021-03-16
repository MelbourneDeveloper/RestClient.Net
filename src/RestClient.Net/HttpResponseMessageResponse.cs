
using RestClient.Net.Abstractions;
using System;
using System.Net.Http;

namespace RestClient.Net
{
    public class HttpResponseMessageResponse<TResponseBody> : Response<TResponseBody>
    {

        #region Public Constructors

        public HttpResponseMessageResponse
        (
            IHeadersCollection httpResponseHeadersCollection,
            int statusCode,
            HttpRequestMethod httpRequestMethod,
            byte[] responseContentData,
            TResponseBody? body,
            HttpResponseMessage httpResponseMessage
            ) : base(
                httpResponseHeadersCollection,
                statusCode,
                httpRequestMethod,
                responseContentData,
                body,
                httpResponseMessage != null ? httpResponseMessage.RequestMessage?.RequestUri : throw new ArgumentNullException(nameof(httpResponseMessage)))
            => HttpResponseMessage = httpResponseMessage;

        #endregion Public Constructors

        #region Public Properties
        public HttpResponseMessage HttpResponseMessage { get; }
        public override bool IsSuccess => HttpResponseMessage.IsSuccessStatusCode;
        #endregion Public Properties

        #region Public Methods

        public override string ToString() => $"Status: {StatusCode} HttpRequestMethod: {HttpRequestMethod} Body: {Body} Request Uri: {HttpResponseMessage?.RequestMessage?.RequestUri}";

        #endregion Public Methods

    }
}
