using System;
using System.Net.Http;
using RestClientDotNet.Abstractions;

namespace RestClientDotNet
{
    public class SingletonHttpClientFactory : IHttpClientFactory
    {
        public TimeSpan Timeout
        {
            get
            {
                return HttpClient.Timeout;
            }
            set
            {
                HttpClient.Timeout = value;
            }
        }

        public Uri BaseUri { get; }
        public IRestHeadersCollection DefaultRequestHeaders { get; }
        public HttpClient HttpClient = new HttpClient();

        public SingletonHttpClientFactory(TimeSpan timeout, Uri baseUri)
        {
            if (BaseUri != null)
            {
                HttpClient.BaseAddress = BaseUri;
            }

            DefaultRequestHeaders = new RestRequestHeadersCollection(HttpClient.DefaultRequestHeaders);
            Timeout = timeout;
            BaseUri = baseUri;
        }

        public HttpClient Create()
        {
            return HttpClient;
        }
    }

}
