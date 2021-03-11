using System;
#pragma warning disable CA2225 // Operator overloads have named alternates

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
        //protected Response() : base()
        //{
        //}

        protected Response(
        IHeadersCollection headersCollection,
        int statusCode,
        HttpRequestMethod httpRequestMethod,
        byte[] responseData,
        TResponseBody body,
        Uri? requestUri
        ) : base(
            headersCollection,
            statusCode,
            httpRequestMethod,
            responseData,
            requestUri) => Body = body;

        public static implicit operator TResponseBody(Response<TResponseBody> readResult)
#pragma warning disable CA1065 // Do not raise exceptions in unexpected locations
            => readResult != null ? readResult.Body : throw new ArgumentNullException(nameof(readResult));
#pragma warning restore CA1065 // Do not raise exceptions in unexpected locations
        #endregion
    }

    public abstract class Response
    {
        #region Fields
        private readonly byte[]? _responseData;
        #endregion

        #region Public Properties
        public virtual int StatusCode { get; set; }
        public virtual IHeadersCollection? Headers { get; set; }
        public virtual HttpRequestMethod HttpRequestMethod { get; set; }
        public abstract bool IsSuccess { get; }
        public virtual Uri? RequestUri { get; set; }
        #endregion

        #region Constructor
        /// <summary>
        /// Only used for mocking or other inheritance
        /// </summary>
        //protected Response()
        //{
        //}

        protected Response
        (
        IHeadersCollection headersCollection,
        int statusCode,
        HttpRequestMethod httpRequestMethod,
        byte[] responseData,
        Uri? requestUri
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
        public virtual byte[]? GetResponseData() => _responseData;
        #endregion
    }
}