using System.Net.Http;

namespace RestClientDotNet
{
    public class SingletonHttpClientFactory : IHttpClientFactory
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
