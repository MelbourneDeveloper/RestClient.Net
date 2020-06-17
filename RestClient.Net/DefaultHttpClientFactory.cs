using System;
using System.Collections.Concurrent;
using System.Net.Http;
using System.Threading;

namespace RestClient.Net
{
    [Obsolete("This class is not longer necessary because the Client class creates a delegate for handling HttpClient creation")]
    public class DefaultHttpClientFactory : IDisposable
    {
        #region Fields
        private bool disposed;
        private readonly ConcurrentDictionary<string, Lazy<HttpClient>> _httpClients;
        private readonly Func<string, Lazy<HttpClient>> _createClientFunc;
        #endregion

        #region Constructor
        public DefaultHttpClientFactory() : this(null)
        {
        }

        public DefaultHttpClientFactory(Func<string, Lazy<HttpClient>> createClientFunc)
        {
            _createClientFunc = createClientFunc;
            _httpClients = new ConcurrentDictionary<string, Lazy<HttpClient>>();

            if (_createClientFunc != null) return;
            _createClientFunc = name =>
            {
                return new Lazy<HttpClient>(() => new HttpClient(), LazyThreadSafetyMode.ExecutionAndPublication);
            };
        }
        #endregion

        #region Implementation
        public HttpClient CreateClient(string name)
        {
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            return _httpClients.GetOrAdd(name, _createClientFunc).Value;
        }

        public void Dispose()
        {
            if (disposed) return;
            disposed = true;

            foreach (var name in _httpClients.Keys)
            {
                _httpClients[name].Value.Dispose();
            }
        }
        #endregion
    }
}
