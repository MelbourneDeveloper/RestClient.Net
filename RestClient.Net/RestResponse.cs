using RestClientDotNet.Abstractions;

namespace RestClientDotNet
{
    public class RestResponse<TBody> : RestResponse
    {
        public TBody Body { get; }

        public RestResponse(TBody body, IRestHeadersCollection restHeadersCollection, int statusCode, object underlyingResponse) : base(restHeadersCollection, statusCode, underlyingResponse)
        {
            Body = body;
        }

        #region Implicit Operator
        public static implicit operator TBody(RestResponse<TBody> readResult)
        {
            return readResult.Body;
        }

        public TBody ToTBody()
        {
            return Body;
        }
        #endregion
    }

    public class RestResponse
    {
        #region Public Properties
        public int StatusCode { get; }
        public object UnderlyingResponse { get; }
        public IRestHeadersCollection Headers { get; }
        #endregion

        #region Constructor
        public RestResponse(IRestHeadersCollection restHeadersCollection, int statusCode, object underlyingResponse)
        {
            StatusCode = statusCode;
            UnderlyingResponse = underlyingResponse;
            Headers = restHeadersCollection;
        }
        #endregion
    }
}
