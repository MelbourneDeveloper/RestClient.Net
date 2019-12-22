using System;
using System.Net.Http;
using RestClientDotNet.Abstractions;

namespace RestClientDotNet
{
    public class SingletonHttpClientFactory : IHttpClientFactory
    {
        #region Public Properties
        public TimeSpan Timeout
        {
            get => HttpClient.Timeout;
            set
            {
                //For some reason this can't be set to blank?
                if (value == default) return;

                HttpClient.Timeout = value;
            }
        }

        public Uri BaseUri => HttpClient.BaseAddress;
        public IRestHeadersCollection DefaultRequestHeaders { get; }
        public HttpClient HttpClient { get; } = new HttpClient();
        #endregion

        #region Constructor
        public SingletonHttpClientFactory(TimeSpan timeout, Uri baseUri)
        {
            if (baseUri != null)
            {
                HttpClient.BaseAddress = baseUri;
            }

            DefaultRequestHeaders = new RestRequestHeadersCollection(HttpClient.DefaultRequestHeaders);
            Timeout = timeout;
        }
        #endregion

        #region Implementation
        public HttpClient CreateHttpClient()
        {
            return HttpClient;
        }
        #endregion
    }

}
