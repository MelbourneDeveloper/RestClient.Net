#nullable disable

using RestClient.Net.Abstractions;
using System;
using System.Net.Http;

namespace RestClient.Net
{
    public class HttpResponseMessageResponse<TResponseBody> : Response<TResponseBody>
    {
        #region Public Properties
        public HttpResponseMessage HttpResponseMessage { get; set; }
        public override bool IsSuccess => HttpResponseMessage.IsSuccessStatusCode;
        public HttpClient HttpClient { get; set; }
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor for mocking. Don't use this for anything other than unit tests.
        /// </summary>
        public HttpResponseMessageResponse(TResponseBody body) : this(null, 0, HttpRequestMethod.Get, null, body, new HttpResponseMessage(), null)
        {
        }

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
                httpResponseMessage != null ? httpResponseMessage.RequestMessage?.RequestUri : throw new ArgumentNullException(nameof(httpResponseMessage)))
        {
            HttpResponseMessage = httpResponseMessage;
            HttpClient = httpClient;
        }

        //#pragma warning disable CA1000 // Do not declare static members on generic types
        //        public static HttpResponseMessageResponse<T> NewHttpResponseMessageResponseFromDefaultStruct<T>
        //#pragma warning restore CA1000 // Do not declare static members on generic types
        //            (
        //            HttpResponseHeadersCollection httpResponseHeadersCollection,
        //            int statusCode,
        //            HttpRequestMethod httpRequestMethod,
        //            byte[] responseContentData,
        //            HttpResponseMessage httpResponseMessage,
        //            HttpClient httpClient
        //            ) where T : struct
        //        {
        //            return new HttpResponseMessageResponse<T>(httpResponseHeadersCollection, statusCode, httpRequestMethod, responseContentData, default, httpResponseMessage, httpClient);
        //        }

        //public static HttpResponseMessageResponse<T?> ValueTypeNullable<T>() where T : struct
        //    => new HttpResponseMessageResponse<T?>(null);

        //public static HttpResponseMessageResponse<T> ReferenceTypeNotNull<T>() where T : class, new()
        //    => new HttpResponseMessageResponse<T>(new T());

        //public static HttpResponseMessageResponse<T?> ReferenceTypeNullable<T>() where T : class
        //    => new HttpResponseMessageResponse<T?>(null);

        #endregion
    }
}
