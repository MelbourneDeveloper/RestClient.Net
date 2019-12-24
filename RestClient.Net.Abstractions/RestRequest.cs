using System;
using System.Threading;

namespace RestClientDotNet.Abstractions
{
    public class RestRequest<TRequestBody>
    {
        #region Public Properties
        public IRestHeadersCollection Headers { get; }
        public Uri Resource { get; set; }
        public HttpVerb HttpVerb { get; set; } = HttpVerb.Get;
        public string ContentType { get; set; } = "application/json";
        public TRequestBody Body { get; set; }
        public CancellationToken CancellationToken { get; set; }
        #endregion

        public RestRequest(Uri resource,
            TRequestBody body,
            IRestHeadersCollection headers,
            HttpVerb httpVerb,
            IRestClient client,
            string contentType,
            CancellationToken cancellationToken)
        {
            Body = body;
            Headers = headers;
            Resource = resource;
            HttpVerb = httpVerb;
            ContentType = contentType;
            CancellationToken = cancellationToken;

            if (Headers == null) Headers = new RestRequestHeadersCollection();

            if (client == null) return;

            var defaultContentType = client.DefaultContentType;
            if (string.IsNullOrEmpty(ContentType) && !string.IsNullOrEmpty(defaultContentType))
            {
                ContentType = defaultContentType;
            }

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