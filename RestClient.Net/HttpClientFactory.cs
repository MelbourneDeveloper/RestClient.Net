using System;
using System.Net.Http;

namespace RestClientDotNet
{
    public class HttpClientFactory : IHttpClientFactory
    {
        public TimeSpan Timeout { get; set; }
        public Uri BaseUri { get; }

        public HttpClientFactory(TimeSpan timeout, Uri baseUri)
        {
            Timeout = timeout;
            BaseUri = baseUri;
        }

        public HttpClient Create()
        {
            var httpClient = new HttpClient();

            if (Timeout != default)
            {
                httpClient.Timeout = Timeout;
            }

            if (BaseUri != null)
            {
                httpClient.BaseAddress = BaseUri;
            }

            return httpClient;
        }
    }

}
