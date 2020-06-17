using System;
using System.Threading;

namespace RestClient.Net.Abstractions
{
    public class Request<TRequestBody>
    {
        #region Public Properties
        public IHeadersCollection Headers { get; set; }
        public Uri Resource { get; set; }
        public HttpRequestMethod HttpRequestMethod { get; set; }
        public TRequestBody Body { get; set; }
        public CancellationToken CancellationToken { get; set; }
        public string CustomHttpRequestMethod { get; set; }
        #endregion

        #region Constructors
        /// <summary>
        /// Use this to construct mocked requests
        /// </summary>
        public Request()
        {

        }

        /// <summary>
        /// Construct a Request
        /// </summary>
        /// <param name="resource"></param>
        /// <param name="body"></param>
        /// <param name="headers"></param>
        /// <param name="httpRequestMethod"></param>
        /// <param name="client"></param>
        /// <param name="cancellationToken"></param>
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
        #endregion
    }
}