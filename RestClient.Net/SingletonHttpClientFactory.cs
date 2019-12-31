using System;
using System.Net.Http;

namespace RestClientDotNet
{
    /// <summary>
    /// Factory for using a single HttpClient. This can be used in the simplest scenarios. 
    /// </summary>
    public class SingletonHttpClientFactory : IHttpClientFactory, IDisposable
    {
        #region Fields
        private bool disposed;
        #endregion

        #region Public Properties
        public HttpClient HttpClient { get; }
        #endregion

        #region Constructor
        public SingletonHttpClientFactory() : this(null)
        {
        }

        public SingletonHttpClientFactory(HttpClient httpClient)
        {
            if (httpClient == null) httpClient = new HttpClient();
            HttpClient = httpClient;
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
