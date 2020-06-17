using System;

namespace RestClient.Net.Abstractions
{
    public abstract class Response<TResponseBody> : Response
    {
        #region Public Properties
        public virtual TResponseBody Body { get; set; }
        #endregion

        #region Constructors
        /// <summary>
        /// Only used for mocking or other inheritance
        /// </summary>
        protected Response() : base()
        {
        }

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
        #endregion
    }

    public abstract class Response
    {
        #region Fields
        private readonly byte[] _responseData;
        #endregion

        #region Public Properties
        public virtual int StatusCode { get; }
        public virtual IHeadersCollection Headers { get; }
        public virtual HttpRequestMethod HttpRequestMethod { get; }
        public abstract bool IsSuccess { get; }
        public virtual Uri RequestUri { get; }
        #endregion

        #region Constructor
        /// <summary>
        /// Only used for mocking or other inheritance
        /// </summary>
        protected Response()
        {
        }

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
        public virtual byte[] GetResponseData()
        {
            return _responseData;
        }
        #endregion
    }
}