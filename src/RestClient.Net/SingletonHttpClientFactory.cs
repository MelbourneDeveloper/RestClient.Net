using System;
using System.Net.Http;

namespace RestClient.Net
{
    /// <summary>
    /// Factory for using a single HttpClient. This can be used in the simplest scenarios. 
    /// </summary>
    public sealed class SingletonHttpClientFactory : IDisposable
    {
        #region Fields
        private bool disposed;
        #endregion

        #region Public Properties
        public HttpClient HttpClient { get; }
        #endregion

        #region Constructor
        public SingletonHttpClientFactory(HttpClient httpClient) => HttpClient = httpClient;
        #endregion

        #region Implementation
#pragma warning disable CA1801 // Review unused parameters
#pragma warning disable IDE0060 // Remove unused parameter
        public HttpClient CreateClient(string name) => HttpClient;
#pragma warning restore IDE0060 // Remove unused parameter
#pragma warning restore CA1801 // Review unused parameters

        public void Dispose()
        {
            if (disposed) return;
            disposed = true;
            HttpClient.Dispose();
        }
        #endregion
    }

}
