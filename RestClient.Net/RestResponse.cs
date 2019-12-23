using RestClientDotNet.Abstractions;
using System;
using System.Threading.Tasks;

namespace RestClientDotNet
{
    public class RestResponse<TBody> : IRestResponse<TBody>
    {
        #region Public Properties
        public IResponseProcessor ResponseProcessor { get; }
        public Uri BaseUri { get; }
        public Uri Resource { get; }
        #endregion

        #region Constructor
        public RestResponse(
            IRestHeadersCollection restHeadersCollection,
            IResponseProcessor responseProcessor,
            int statusCode,
            Uri baseUri,
            Uri resource,
            HttpVerb httpVerb,
            TBody body
            ) : base(restHeadersCollection, statusCode, httpVerb, body)
        {
            ResponseProcessor = responseProcessor;
            BaseUri = baseUri;
            Resource = resource;
        }

        public async Task<T> ReadResponseAsync<T>()
        {
            return await ResponseProcessor.ProcessRestResponseAsync<T>(BaseUri, Resource, HttpVerb);
        }
        #endregion
    }
}
