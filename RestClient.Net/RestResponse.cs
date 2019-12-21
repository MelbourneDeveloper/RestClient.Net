namespace RestClientDotNet
{
    public class RestResponse<T>
    {
        #region Public Properties
        public T Body { get; }
        public int StatusCode { get; }
        public object UnderlyingResponse { get; }
        public IRestHeadersCollection RestHeadersCollection { get; }
        #endregion

        #region Constructor
        public RestResponse(T body, IRestHeadersCollection restHeadersCollection, int statusCode, object underlyingResponse)
        {
            Body = body;
            StatusCode = statusCode;
            UnderlyingResponse = underlyingResponse;
            RestHeadersCollection = restHeadersCollection;
        }
        #endregion

        #region Implicit Operator
#pragma warning disable CA2225 // Operator overloads have named alternates
        public static implicit operator T(RestResponse<T> readResult)
#pragma warning restore CA2225 // Operator overloads have named alternates
        {
#pragma warning disable CA1062 // Validate arguments of public methods
            return readResult.Body;
#pragma warning restore CA1062 // Validate arguments of public methods
        }
        #endregion
    }
}
