namespace RestClientDotNet.Abstractions
{
    public abstract class RestResponseBase<TResponseBody> : RestResponseBase
    {
        protected RestResponseBase(
        IRestHeaders restHeadersCollection,
        int statusCode,
        HttpVerb httpVerb,
        byte[] responseData,
        TResponseBody body
        ) : base(restHeadersCollection, statusCode, httpVerb, responseData)
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

        #region Public Methods
        public int StatusCode { get; }
        public IRestHeaders Headers { get; }
        public HttpVerb HttpVerb { get; }
        public abstract bool IsSuccess { get; }
        #endregion

        #region Constructor
        protected RestResponseBase
        (
        IRestHeaders restHeadersCollection,
        int statusCode,
        HttpVerb httpVerb,
        byte[] responseData
        )
        {
            StatusCode = statusCode;
            Headers = restHeadersCollection;
            HttpVerb = httpVerb;
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