using System;

namespace RestClientDotNet.Abstractions
{
    public abstract class RestResponseBase<TResponseBody> : RestResponseBase
    {
        protected RestResponseBase(
        IRestHeadersCollection restHeadersCollection,
        int statusCode,
        HttpRequestMethod httpRequestMethod,
        byte[] responseData,
        TResponseBody body,
        Uri requestUri
        ) : base(
            restHeadersCollection,
            statusCode,
            httpRequestMethod,
            responseData,
            requestUri)
        {
            Body = body;
        }

#pragma warning disable CA2225 // Operator overloads have named alternates
        public static implicit operator TResponseBody(RestResponseBase<TResponseBody> readResult)
#pragma warning restore CA2225 // Operator overloads have named alternates
        {
#pragma warning disable CA1062 // Validate arguments of public methods
            return readResult.Body;
#pragma warning restore CA1062 // Validate arguments of public methods
        }

        public TResponseBody Body { get; }
    }

    public abstract class RestResponseBase
    {
        #region Fields
        private readonly byte[] _responseData;
        #endregion

        #region Public Properties
        public int StatusCode { get; }
        public IRestHeadersCollection Headers { get; }
        public HttpRequestMethod HttpRequestMethod { get; }
        public abstract bool IsSuccess { get; }
        public Uri RequestUri { get; }
        #endregion

        #region Constructor
        protected RestResponseBase
        (
        IRestHeadersCollection restHeadersCollection,
        int statusCode,
        HttpRequestMethod httpRequestMethod,
        byte[] responseData,
        Uri requestUri
        )
        {
            StatusCode = statusCode;
            Headers = restHeadersCollection;
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