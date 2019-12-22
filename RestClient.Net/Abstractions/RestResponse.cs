using System;
using System.Threading.Tasks;

namespace RestClientDotNet.Abstractions
{
    public class RestResponse<TBody> : RestResponse
    {
        public TBody Body { get; }

        public RestResponse(
            TBody body,
            IRestHeadersCollection restHeadersCollection,
            int statusCode,
            IResponseProcessor responseProcessor
            ) : base(restHeadersCollection, responseProcessor, statusCode)
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
        public IRestHeadersCollection Headers { get; }
        public IResponseProcessor ResponseProcessor { get; }
        public Uri BaseUri { get; }
        public Uri QueryString { get; }
        public HttpVerb HttpVerb { get; }
        #endregion

        #region Constructor
        public RestResponse(
            IRestHeadersCollection restHeadersCollection,
            IResponseProcessor responseProcessor,
            int statusCode)
        {
            StatusCode = statusCode;
            Headers = restHeadersCollection;
            ResponseProcessor = responseProcessor;
        }

        public async Task<T> ToModel<T>()
        {
            return await ResponseProcessor.ProcessRestResponseAsync<T>(BaseUri, QueryString, HttpVerb);
        }
        #endregion
    }
}
