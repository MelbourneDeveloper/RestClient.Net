using System;
using System.Linq;
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
            Resource = resource;
            HttpRequestMethod = httpRequestMethod;
            CancellationToken = cancellationToken;

            //Default to the headers passed in the constructor
            Headers = headers;

            //Create the collection if it's null
            if (Headers == null) Headers = new RequestHeadersCollection();

            //Return if there are no default headers
            var defaultRequestHeaders = client?.DefaultRequestHeaders;
            if (defaultRequestHeaders == null) return;
            var defaultRequestHeadersList = defaultRequestHeaders.ToList();
            if (defaultRequestHeadersList.Count == 0) return;

            //Create a new list so we don't modify the list headers passed in
            Headers = new RequestHeadersCollection();

            //Add headers that were passed in
            foreach (var kvp in defaultRequestHeadersList)
            {
                Headers.Add(kvp);
            }

            //Add the default headers
            if (headers != null)
            {
                foreach (var kvp in headers)
                {
                    Headers.Add(kvp);
                }
            }
        }
        #endregion
    }
}