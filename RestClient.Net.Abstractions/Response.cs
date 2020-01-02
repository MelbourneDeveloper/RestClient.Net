using System;

namespace RestClient.Net.Abstractions
{
    public abstract class Response<TResponseBody> : Response
    {
        protected Response(
        IHeadersCollection headersCollection,
        int statusCode,
        HttpRequestMethod httpRequestMethod,
        byte[] responseData,
        TResponseBody body,
        Uri requestUri
        ) : base(
            headersCollection,
            statusCode,
            httpRequestMethod,
            responseData,
            requestUri)
        {
            Body = body;
        }

#pragma warning disable CA2225 // Operator overloads have named alternates
        public static implicit operator TResponseBody(Response<TResponseBody> readResult)
#pragma warning restore CA2225 // Operator overloads have named alternates
        {
#pragma warning disable CA1062 // Validate arguments of public methods
            return readResult.Body;
#pragma warning restore CA1062 // Validate arguments of public methods
        }

        public TResponseBody Body { get; }
    }

    public abstract class Response
    {
        #region Fields
        private readonly byte[] _responseData;
        #endregion

        #region Public Properties
        public int StatusCode { get; }
        public IHeadersCollection Headers { get; }
        public HttpRequestMethod HttpRequestMethod { get; }
        public abstract bool IsSuccess { get; }
        public Uri RequestUri { get; }
        #endregion

        #region Constructor
        protected Response
        (
        IHeadersCollection headersCollection,
        int statusCode,
        HttpRequestMethod httpRequestMethod,
        byte[] responseData,
        Uri requestUri
        )
        {
            StatusCode = statusCode;
            Headers = headersCollection;
            HttpRequestMethod = httpRequestMethod;
            RequestUri = requestUri;
            _responseData = responseData;
        }
        #endregion

        #region Public Methods
        public byte[] GetResponseData()
        {
            return _responseData;
        }
        #endregion
    }
}