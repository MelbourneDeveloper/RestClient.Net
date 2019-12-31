using System;
using System.Threading;

namespace RestClientDotNet.Abstractions
{
    public class Request<TRequestBody>
    {
        #region Public Properties
        public IHeadersCollection Headers { get; }
        public Uri Resource { get; set; }
        public HttpRequestMethod HttpRequestMethod { get; set; }
        public TRequestBody Body { get; set; }
        public CancellationToken CancellationToken { get; set; }
        #endregion

        public Request(Uri resource,
            TRequestBody body,
            IHeadersCollection headers,
            HttpRequestMethod httpRequestMethod,
            IClient client,
            CancellationToken cancellationToken)
        {
            Body = body;
            Headers = headers;
            Resource = resource;
            HttpRequestMethod = httpRequestMethod;
            CancellationToken = cancellationToken;

            if (Headers == null) Headers = new RequestHeadersCollection();

            var defaultRequestHeaders = client?.DefaultRequestHeaders;
            if (defaultRequestHeaders == null) return;

            foreach (var kvp in defaultRequestHeaders)
            {
                Headers.Add(kvp);
            }
        }
    }
}