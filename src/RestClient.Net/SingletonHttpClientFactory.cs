using System;
using System.Net.Http;

namespace RestClient.Net
{
    /// <summary>
    /// Factory for using a single HttpClient. This can be used in the simplest scenarios. 
    /// </summary>
    public class SingletonHttpClientFactory : IDisposable
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
#pragma warning disable IDE0060
#pragma warning disable CA1801
        public HttpClient CreateClient(string name)
#pragma warning restore IDE0060
#pragma warning restore CA1801
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
