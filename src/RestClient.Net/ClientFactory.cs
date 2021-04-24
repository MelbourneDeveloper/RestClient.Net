
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using RestClient.Net.Abstractions;
using RestClient.Net.DI;
using System;
using System.Collections.Concurrent;
using System.Threading;

namespace RestClient.Net
{
    public class ClientFactory
    {
        #region Fields
        private readonly Func<string, Action<ClientBuilderOptions>?, Lazy<IClient>> createClientFunc;
        private readonly ConcurrentDictionary<string, Lazy<IClient>> clients;
        private readonly CreateHttpClient createHttpClient;
        private readonly ILoggerFactory loggerFactory;
        #endregion

        #region Constructor
        public ClientFactory(
            CreateHttpClient createHttpClient,
            ILoggerFactory? loggerFactory = null)
        {

            this.createHttpClient = createHttpClient;
            this.loggerFactory = loggerFactory ?? NullLoggerFactory.Instance;

            clients = new ConcurrentDictionary<string, Lazy<IClient>>();

            createClientFunc = (name, baseUri) => new Lazy<IClient>(() => MintClient(name, baseUri), LazyThreadSafetyMode.ExecutionAndPublication);
        }
        #endregion

        #region Implementation
        public IClient CreateClient(string name, Action<ClientBuilderOptions>? configureClient = null)
            => name == null ? throw new ArgumentNullException(nameof(name)) : clients.GetOrAdd(name, createClientFunc(name, configureClient)).Value;
        #endregion

        #region Private Methods
        private IClient MintClient(string name, Action<ClientBuilderOptions>? configureClient = null)
        {
            var obj = new ClientBuilderOptions(createHttpClient);

            configureClient?.Invoke(obj);

            return new Client(
                obj.SerializationAdapter,
                obj.BaseUrl,
                createHttpClient: obj.CreateHttpClient,
                logger: loggerFactory?.CreateLogger<Client>(),
                name: name);
        }
        #endregion
    }
}