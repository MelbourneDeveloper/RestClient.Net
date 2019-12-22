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
                //For some reason this can't be set to blank?
                if (value == default) return;

                HttpClient.Timeout = value;
            }
        }

        public Uri BaseUri => HttpClient.BaseAddress;
        public IRestHeadersCollection DefaultRequestHeaders { get; }
        public HttpClient HttpClient = new HttpClient();

        public SingletonHttpClientFactory(TimeSpan timeout, Uri baseUri)
        {
            if (baseUri != null)
            {
                HttpClient.BaseAddress = baseUri;
            }

            DefaultRequestHeaders = new RestRequestHeadersCollection(HttpClient.DefaultRequestHeaders);
            Timeout = timeout;
        }

        public HttpClient Create()
        {
            return HttpClient;
        }
    }

}
