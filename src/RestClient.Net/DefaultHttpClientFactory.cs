using System;
using System.Collections.Concurrent;
using System.Net.Http;
using System.Threading;

namespace RestClient.Net
{
    public sealed class DefaultHttpClientFactory : IDisposable
    {
        #region Fields
        private bool disposed;
        private readonly ConcurrentDictionary<string, Lazy<HttpClient>> httpClients;
        private readonly Func<string, Lazy<HttpClient>> createClientFunc;
        #endregion

        #region Constructor
        public DefaultHttpClientFactory() : this(null)
        {
        }

        public DefaultHttpClientFactory(Func<string, Lazy<HttpClient>>? createClientFunc)
        {
            httpClients = new ConcurrentDictionary<string, Lazy<HttpClient>>();

            this.createClientFunc = createClientFunc ?? new Func<string, Lazy<HttpClient>>(name => new Lazy<HttpClient>(() => new HttpClient(), LazyThreadSafetyMode.ExecutionAndPublication));
        }
        #endregion

        #region Implementation
        public HttpClient CreateClient(string name) => name == null ? throw new ArgumentNullException(nameof(name)) : httpClients.GetOrAdd(name, createClientFunc).Value;

        public void Dispose()
        {
            if (disposed) return;
            disposed = true;

            foreach (var name in httpClients.Keys)
            {
                httpClients[name].Value.Dispose();
            }
        }
        #endregion
    }
}
