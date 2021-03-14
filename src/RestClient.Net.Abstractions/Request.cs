using System;
using System.Threading;

namespace RestClient.Net.Abstractions
{
    public class Request : IRequest
    {
        #region Fields
        private readonly IClient client;
        #endregion

        #region Public Properties
#pragma warning disable CA1819 // Properties should not return arrays
        public byte[]? BodyData { get; }
#pragma warning restore CA1819 // Properties should not return arrays
        public IHeadersCollection? Headers { get; }
        public Uri? Resource { get; }
        public HttpRequestMethod HttpRequestMethod { get; }
        public CancellationToken CancellationToken { get; }
        public string? CustomHttpRequestMethod { get; }
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
            CancellationToken cancellationToken,
            string? customHttpRequestMethod = null)
        {
            BodyData = bodyData;
            Resource = resource;
            HttpRequestMethod = httpRequestMethod;
            CancellationToken = cancellationToken;
            CustomHttpRequestMethod = customHttpRequestMethod;
            this.client = client;

            //Default to the headers passed in the constructor
            Headers = headers ?? NullHeadersCollection.Instance;

            //Return if there are no default headers
            var defaultRequestHeaders = client?.DefaultRequestHeaders;
            if (defaultRequestHeaders == null) return;

            Headers = Headers.Append(defaultRequestHeaders);
        }

        public override string ToString() => $"\r\nClient BaseUri: {client.BaseUri}\r\nResource: {Resource}\r\nHeaders: {Headers} Method: {HttpRequestMethod}";


    }
}