using RestClient.Net.Abstractions;
using System;
using System.Collections.Concurrent;
using System.Threading;

#if NET45
using RestClient.Net.Abstractions.Logging;
#else
using Microsoft.Extensions.Logging;
#endif


namespace RestClient.Net
{
    public class ClientFactory
    {
        #region Fields
        private readonly Func<string, Lazy<IClient>> _createClientFunc;
        private readonly ConcurrentDictionary<string, Lazy<IClient>> _clients;
        private readonly CreateHttpClient _createHttpClient;
        #endregion

        #region Public Properties
        public ISerializationAdapter SerializationAdapter { get; }
        public ILogger Logger { get; }
        #endregion

        #region Constructor
        public ClientFactory(
#if NETCOREAPP3_1
            ISerializationAdapter serializationAdapter = null,
#else
            ISerializationAdapter serializationAdapter,
#endif
            CreateHttpClient createHttpClient = null,
            ILogger logger = null)
        {
            SerializationAdapter = serializationAdapter;
            _createHttpClient = createHttpClient;
            Logger = logger;

            _clients = new ConcurrentDictionary<string, Lazy<IClient>>();

            _createClientFunc = name =>
            {
                return new Lazy<IClient>(() => MintClient(name), LazyThreadSafetyMode.ExecutionAndPublication);
            };
        }
        #endregion

        #region Implementation
        public IClient CreateClient(string name)
        {
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            return _clients.GetOrAdd(name, _createClientFunc).Value;
        }
        #endregion

        #region Private Methods
        private IClient MintClient(string name)
        {
#pragma warning disable CA2000 // Dispose objects before losing scope
            return new Client(
                SerializationAdapter,
                name,
                null,
                logger: Logger,
                createHttpClient: _createHttpClient);
#pragma warning restore CA2000 // Dispose objects before losing scope
        }
        #endregion
    }
}
