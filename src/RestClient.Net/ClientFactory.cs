
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System;
using System.Collections.Concurrent;

namespace RestClient.Net
{
    public class ClientFactory
    {
        #region Fields
        private readonly Func<string, CreateClientOptions, IClient> createClientFunc;
        private readonly ConcurrentDictionary<string, IClient> clients;
        private readonly CreateHttpClient createHttpClient;
        private readonly ILoggerFactory loggerFactory;
        #endregion

        #region Constructor
        public ClientFactory(
            CreateHttpClient createHttpClient,
            ILoggerFactory? loggerFactory = null,
            Func<string, CreateClientOptions, IClient>? createClientFunc = null)
        {

            this.createHttpClient = createHttpClient;
            this.loggerFactory = loggerFactory ?? NullLoggerFactory.Instance;

            clients = new ConcurrentDictionary<string, IClient>();

            this.createClientFunc = createClientFunc ?? new Func<string, CreateClientOptions, IClient>(MintClient);
        }
        #endregion

        #region Implementation
        public IClient CreateClient(string name, Action<CreateClientOptions>? configureClient = null)
            => name == null ? throw new ArgumentNullException(nameof(name)) :
            clients.GetOrAdd(name, (n) =>
            {
                var options = new CreateClientOptions(createHttpClient);
                configureClient?.Invoke(options);
                return createClientFunc(n, options);
            });
        #endregion

        #region Private Methods
        private IClient MintClient(string name, CreateClientOptions createClientOptions)
        {

#if NET45
            if (createClientOptions.SerializationAdapter == null) throw new InvalidOperationException("You must specify a SerializationAdapter");
#endif

            return new Client(
                createClientOptions.SerializationAdapter,
                createClientOptions.BaseUrl,
                createClientOptions.DefaultRequestHeaders,
                loggerFactory?.CreateLogger<Client>(),
                createClientOptions.CreateHttpClient,
                createClientOptions.SendHttpRequestMessage,
                createClientOptions.GetHttpRequestMessage,
                createClientOptions.ThrowExceptionOnFailure,
                name);
        }
        #endregion
    }
}