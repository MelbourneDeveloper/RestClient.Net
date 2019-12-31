using System;
using System.Threading;

namespace RestClientDotNet.Abstractions
{
    public class RestRequest<TRequestBody>
    {
        #region Public Properties
        public IRestHeadersCollection Headers { get; }
        public Uri Resource { get; set; }
        public HttpRequestMethod HttpRequestMethod { get; set; }
        public TRequestBody Body { get; set; }
        public CancellationToken CancellationToken { get; set; }
        #endregion

        public RestRequest(Uri resource,
            TRequestBody body,
            IRestHeadersCollection headers,
            HttpRequestMethod httpRequestMethod,
            IRestClient client,
            CancellationToken cancellationToken)
        {
            Body = body;
            Headers = headers;
            Resource = resource;
            HttpRequestMethod = httpRequestMethod;
            CancellationToken = cancellationToken;

            if (Headers == null) Headers = new RestRequestHeadersCollection();

            var defaultRequestHeaders = client?.DefaultRequestHeaders;
            if (defaultRequestHeaders == null) return;

            foreach (var kvp in defaultRequestHeaders)
            {
                Headers.Add(kvp);
            }
        }
    }
}