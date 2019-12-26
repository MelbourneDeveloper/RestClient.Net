using System;
using System.Threading;

namespace RestClientDotNet.Abstractions
{
    public class RestRequest<TRequestBody>
    {
        #region Public Properties
        public IRestHeaders Headers { get; }
        public Uri Resource { get; set; }
        public HttpVerb HttpVerb { get; set; } = HttpVerb.Get;
        public TRequestBody Body { get; set; }
        public CancellationToken CancellationToken { get; set; }
        #endregion

        public RestRequest(Uri resource,
            TRequestBody body,
            IRestHeaders headers,
            HttpVerb httpVerb,
            IRestClient client,
            CancellationToken cancellationToken)
        {
            Body = body;
            Headers = headers;
            Resource = resource;
            HttpVerb = httpVerb;
            CancellationToken = cancellationToken;

            if (Headers == null) Headers = new RestRequestHeaders();

            if (client == null) return;

            var headerNames = client.DefaultRequestHeaders?.Names;
            if (headerNames != null)
            {
                foreach (var headerName in headerNames)
                {
                    Headers.Add(headerName, client.DefaultRequestHeaders[headerName]);
                }
            }
        }
    }
}