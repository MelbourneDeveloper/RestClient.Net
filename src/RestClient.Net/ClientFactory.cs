
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System;
using System.Collections.Concurrent;

namespace RestClient.Net
{
    public class ClientFactory
    {
        #region Fields
        private readonly Func<string, Action<CreateClientOptions>, IClient> createClientFunc;
        private readonly ConcurrentDictionary<string, IClient> clients;
        private readonly CreateHttpClient createHttpClient;
        private readonly ILoggerFactory loggerFactory;
        #endregion

        #region Constructor
        public ClientFactory(
            CreateHttpClient createHttpClient,
            ILoggerFactory? loggerFactory = null,
            Func<string, Action<CreateClientOptions>, IClient>? createClientFunc = null)
        {

            this.createHttpClient = createHttpClient;
            this.loggerFactory = loggerFactory ?? NullLoggerFactory.Instance;

            clients = new ConcurrentDictionary<string, IClient>();

            this.createClientFunc = createClientFunc ?? new Func<string, Action<CreateClientOptions>, IClient>((name, baseUri) => MintClient(name, baseUri));
        }
        #endregion

        #region Implementation
        public IClient CreateClient(string name, Action<CreateClientOptions>? configureClient = null)
            => name == null ? throw new ArgumentNullException(nameof(name)) :
            clients.GetOrAdd(name, createClientFunc(name, configureClient ?? new Action<CreateClientOptions>((o) => { })));
        #endregion

        #region Private Methods
        private IClient MintClient(string name, Action<CreateClientOptions>? configureClient = null)
        {
            var createClientOptions = new CreateClientOptions(createHttpClient);

            configureClient?.Invoke(createClientOptions);

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