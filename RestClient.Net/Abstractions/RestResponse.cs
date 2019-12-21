namespace RestClientDotNet.Abstractions
{
    public class RestResponse<TBody> : RestResponse
    {
        public TBody Body { get; }

        public RestResponse(
            TBody body,
            IRestHeadersCollection restHeadersCollection,
            int statusCode,
            object underlyingResponse,
            IErrorHandler errorHandler
            ) : base(restHeadersCollection, statusCode, underlyingResponse, errorHandler)
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

    public class RestResponse : IRestResponse
    {
        #region Public Properties
        public int StatusCode { get; }
        public object UnderlyingResponse { get; }
        public IRestHeadersCollection Headers { get; }
        public IErrorHandler ErrorHandler { get; }
        #endregion

        #region Constructor
        public RestResponse(IRestHeadersCollection restHeadersCollection, int statusCode, object underlyingResponse, IErrorHandler errorHandler)
        {
            StatusCode = statusCode;
            UnderlyingResponse = underlyingResponse;
            Headers = restHeadersCollection;
            ErrorHandler = errorHandler;
        }

        public T ToErrorModel<T>()
        {
            return ErrorHandler.GetErrorModel<T>();
        }
        #endregion
    }

    public interface IErrorHandler
    {
        T GetErrorModel<T>();
    }

    public interface IErrorHandlerFactory
    {
        IErrorHandler Create(object response);
    }
}
