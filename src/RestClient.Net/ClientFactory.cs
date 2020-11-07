#if !NET45

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using RestClient.Net.Abstractions;
using System;
using System.Collections.Concurrent;
using System.Threading;

namespace RestClient.Net
{
    public class ClientFactory
    {
        #region Fields
        private readonly Func<string, Uri?, Lazy<IClient>> _createClientFunc;
        private readonly ConcurrentDictionary<string, Lazy<IClient>> _clients;
        private readonly CreateHttpClient _createHttpClient;
        private readonly ILoggerFactory _loggerFactory;
        #endregion

        #region Public Properties
        public ISerializationAdapter SerializationAdapter { get; }
        #endregion

        #region Constructor
        public ClientFactory(
            CreateHttpClient createHttpClient,
            ISerializationAdapter? serializationAdapter = null,
            ILoggerFactory? loggerFactory = null)
        {
            SerializationAdapter = serializationAdapter ?? new JsonSerializationAdapter();
            _createHttpClient = createHttpClient;
            _loggerFactory = loggerFactory ?? NullLoggerFactory.Instance;

            _clients = new ConcurrentDictionary<string, Lazy<IClient>>();

            _createClientFunc = (name, baseUri) => new Lazy<IClient>(() => MintClient(name, baseUri), LazyThreadSafetyMode.ExecutionAndPublication);
        }
        #endregion

        #region Implementation
        public IClient CreateClient(string name, Uri? baseUri = null) => name == null ? throw new ArgumentNullException(nameof(name)) : _clients.GetOrAdd(name, _createClientFunc(name, baseUri)).Value;
        #endregion

        #region Private Methods
        private IClient MintClient(string name, Uri? baseUri = null) =>
#pragma warning disable CA2000 // Dispose objects before losing scope
            new Client(
                SerializationAdapter,
                name,
                baseUri,
                logger: _loggerFactory?.CreateLogger<Client>(),
                createHttpClient: _createHttpClient);
#pragma warning restore CA2000 // Dispose objects before losing scope
        #endregion
    }
}

#endif