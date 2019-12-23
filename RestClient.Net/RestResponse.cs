using RestClientDotNet.Abstractions;
using System;
using System.Threading.Tasks;

namespace RestClientDotNet
{
    public class RestResponse<TBody> : RestResponseBase<TBody>
    {
        #region Public Properties
        public Uri BaseUri { get; }
        public Uri Resource { get; }
        #endregion

        #region Constructor
        public RestResponse(
            IRestHeadersCollection restHeadersCollection,
            int statusCode,
            Uri baseUri,
            Uri resource,
            HttpVerb httpVerb,
            byte[] responseContentData,
            TBody body
            ) : base(restHeadersCollection, statusCode, httpVerb, responseContentData, body)
        {
            BaseUri = baseUri;
            Resource = resource;
        }
        #endregion
    }
}
