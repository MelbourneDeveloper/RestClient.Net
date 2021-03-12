
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
        /// <summary>
        /// Constructor for mocking. Don't use this for anything other than unit tests.
        /// </summary>
        public HttpResponseMessageResponse(TResponseBody body) : this(NullHeadersCollection.Instance, 0, HttpRequestMethod.Get,
#if NET45
            new byte[0]
#else
            Array.Empty<byte>()
#endif
            , body, NullObjects.NullHttpResponseMessage, NullObjects.NullHttpClient)
        {
        }

        public HttpResponseMessageResponse
        (
            IHeadersCollection httpResponseHeadersCollection,
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
                httpResponseMessage != null ? httpResponseMessage.RequestMessage?.RequestUri : throw new ArgumentNullException(nameof(httpResponseMessage)))
        {
            HttpResponseMessage = httpResponseMessage;
            HttpClient = httpClient;
        }

        #endregion

        public override string ToString() => $"Status: {StatusCode} HttpRequestMethod: {HttpRequestMethod} Body: {Body} Request Uri: {HttpResponseMessage?.RequestMessage?.RequestUri}";

    }
}
