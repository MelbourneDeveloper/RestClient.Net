using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
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
        private readonly ILogger<DefaultHttpClientFactory> logger;
        #endregion

        #region Constructor
        public DefaultHttpClientFactory(Func<string, Lazy<HttpClient>>? createClientFunc = null, ILogger<DefaultHttpClientFactory>? logger = null)
        {
            httpClients = new ConcurrentDictionary<string, Lazy<HttpClient>>();
            this.logger = logger ?? NullLogger<DefaultHttpClientFactory>.Instance;

            this.createClientFunc = createClientFunc ?? (name => new Lazy<HttpClient>(() =>
            {
                this.logger.LogInformation("Created HttpClient {name}", name);
                return new HttpClient();
            }, LazyThreadSafetyMode.ExecutionAndPublication));

        }
        #endregion

        #region Implementation
        public HttpClient CreateClient(string name)
            => name == null ? throw new ArgumentNullException(nameof(name)) : httpClients.GetOrAdd(name, createClientFunc).Value;

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
