using System;
using System.Linq;
using System.Threading;

namespace RestClient.Net.Abstractions
{
    public class Request : IRequest
    {
        #region Public Properties
#pragma warning disable CA1819 // Properties should not return arrays
        public byte[]? BodyData { get; }
#pragma warning restore CA1819 // Properties should not return arrays
        public IHeadersCollection? Headers { get; set; }
        public Uri? Resource { get; set; }
        public HttpRequestMethod HttpRequestMethod { get; set; }
        public CancellationToken CancellationToken { get; set; }
        public string? CustomHttpRequestMethod { get; set; }
        #endregion

        /// <summary>
        /// Construct a Request
        /// </summary>
        /// <param name="resource"></param>
        /// <param name="body"></param>
        /// <param name="headers"></param>
        /// <param name="httpRequestMethod"></param>
        /// <param name="client"></param>
        /// <param name="cancellationToken"></param>
        public Request(
            Uri? resource,
            byte[]? bodyData,
            IHeadersCollection? headers,
            HttpRequestMethod httpRequestMethod,
            IClient client,
            CancellationToken cancellationToken)
        {
            BodyData = bodyData;
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
    }
}