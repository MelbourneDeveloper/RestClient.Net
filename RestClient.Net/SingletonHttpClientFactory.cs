using System;
using System.Net.Http;

namespace RestClientDotNet
{
    public class SingletonHttpClientFactory : IHttpClientFactory
    {
        #region Fields
        private bool disposed;
        #endregion

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

        public HttpClient HttpClient { get; }
        #endregion

        #region Constructor
        public SingletonHttpClientFactory(TimeSpan timeout, Uri baseUri) : this(timeout, baseUri, null)
        {
        }

        public SingletonHttpClientFactory(TimeSpan timeout, Uri baseUri, HttpClient httpClient)
        {
            if (httpClient == null) httpClient = new HttpClient();

            HttpClient = httpClient;

            if (baseUri != null)
            {
                HttpClient.BaseAddress = baseUri;
            }

            Timeout = timeout;
        }
        #endregion

        #region Implementation
        public HttpClient CreateClient(string name)
        {
            return HttpClient;
        }

        public void Dispose()
        {
            if (disposed) return;
            disposed = true;
            HttpClient.Dispose();
        }
        #endregion
    }

}
