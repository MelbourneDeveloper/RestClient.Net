using System;

namespace RestClientDotNet.Abstractions
{
    public class RestRequest<TBody>
    {
        #region Public Properties
        public IRestHeadersCollection Headers { get; }
        public Uri Resource { get; set; }
        public HttpVerb HttpVerb { get; set; } = HttpVerb.Get;
        public string ContentType { get; set; } = "application/json";
        public TBody Body { get; set; }
        #endregion

        public RestRequest(TBody body, IRestHeadersCollection headers, IRestClient client)
        {
            Body = body;
            Headers = headers ?? throw new ArgumentNullException(nameof(headers));

            if (client == null) return;

            var contentType = client.DefaultContentType;
            if (!string.IsNullOrEmpty(contentType))
            {
                ContentType = contentType;
            }

            var clientHeaders = client.DefaultRequestHeaders;
            if (clientHeaders != null)
            {
                foreach (var header in clientHeaders)
                {
                    Headers.Add(header.Key, header.Value);
                }
            }
        }
    }
}