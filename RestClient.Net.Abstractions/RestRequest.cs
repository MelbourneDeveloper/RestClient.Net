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

        public RestRequest(TRequestBody body,
            IRestHeadersCollection headers,
            IRestClient client,
            Uri resource,
            HttpVerb httpVerb,
            string contentType,
            CancellationToken cancellationToken)
        {
            Body = body;
            Headers = headers;
            Resource = resource;
            HttpVerb = httpVerb;
            ContentType = contentType;
            CancellationToken = cancellationToken;

            if (client == null) return;

            var defaultContentType = client.DefaultContentType;
            if (!string.IsNullOrEmpty(defaultContentType))
            {
                ContentType = defaultContentType;
            }

            throw new NotImplementedException();

            //TODO:
            //var clientHeaders = client.DefaultRequestHeaders;
            //if (clientHeaders != null)
            //{
            //    foreach (var header in clientHeaders)
            //    {
            //        Headers.Add(header.Key, header.Value);
            //    }
            //}
        }
    }
}