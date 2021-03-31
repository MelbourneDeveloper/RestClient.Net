using Uris;
#pragma warning disable CA2225 // Operator overloads have named alternates

namespace RestClient.Net.Abstractions
{
    public abstract class Response : IResponse
    {
        #region Fields
        private readonly byte[] responseData;
        #endregion

        #region Public Properties
        public bool IsSuccess => StatusCode is >= 200 and <= 299;
        public virtual int StatusCode { get; }
        public virtual IHeadersCollection Headers { get; }
        public virtual HttpRequestMethod HttpRequestMethod { get; }
        public virtual AbsoluteUri RequestUri { get; }
        #endregion

        #region Constructor
        protected Response
        (
        IHeadersCollection headersCollection,
        int statusCode,
        HttpRequestMethod httpRequestMethod,
        byte[] responseData,
        AbsoluteUri requestUri
        )
        {
            StatusCode = statusCode;
            Headers = headersCollection;
            HttpRequestMethod = httpRequestMethod;
            RequestUri = requestUri;
            this.responseData = responseData;
        }
        #endregion

        #region Public Methods
        public virtual byte[] GetResponseData() => responseData;
        #endregion
    }
}