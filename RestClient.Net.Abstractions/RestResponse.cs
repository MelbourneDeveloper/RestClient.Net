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
            IResponseProcessor responseProcessor,
            Uri baseUri,
            Uri resource,
            HttpVerb httpVerb
            ) : base(restHeadersCollection, responseProcessor, statusCode, baseUri, resource, httpVerb)
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
        public Uri Resource { get; }
        public HttpVerb HttpVerb { get; }
        #endregion

        #region Constructor
        public RestResponse(
            IRestHeadersCollection restHeadersCollection,
            IResponseProcessor responseProcessor,
            int statusCode,
            Uri baseUri,
            Uri resource,
            HttpVerb httpVerb
            )
        {
            StatusCode = statusCode;
            Headers = restHeadersCollection;
            ResponseProcessor = responseProcessor;
            BaseUri = baseUri;
            Resource = resource;
            HttpVerb = httpVerb;
        }

        public async Task<T> ReadResponseAsync<T>()
        {
            return await ResponseProcessor.ProcessRestResponseAsync<T>(BaseUri, Resource, HttpVerb);
        }
        #endregion
    }
}
