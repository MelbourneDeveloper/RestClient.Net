#if !NET45
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
#else
using RestClient.Net.Abstractions.Logging;
#endif
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
#if !NET45
            ISerializationAdapter? serializationAdapter = null,
#else
            ISerializationAdapter serializationAdapter,
#endif
            ILoggerFactory? loggerFactory = null)
        {
#if !NET45
            SerializationAdapter = serializationAdapter ?? new JsonSerializationAdapter();
#else
            SerializationAdapter = serializationAdapter;
#endif
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
            new Client(
                SerializationAdapter,
                name,
                baseUri,
#if !NET45
                logger: _loggerFactory?.CreateLogger<Client>(),
#else
                logger: _loggerFactory?.CreateLogger(nameof(Client)),
#endif
                createHttpClient: _createHttpClient);
        #endregion
    }
}