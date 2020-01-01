using RestClient.Net.Abstractions;

#if NET45
using RestClient.Net.Abstractions.Logging;
#else
using Microsoft.Extensions.Logging;
#endif


namespace RestClient.Net
{
    public class ClientFactory : IClientFactory
    {
        #region Public Properties
        public ISerializationAdapter SerializationAdapter { get; }
        public IHttpClientFactory HttpClientFactory { get; }
        public ILogger Logger { get; }
        #endregion

        #region Constructor
        public ClientFactory(ISerializationAdapter serializationAdapter,
                                         IHttpClientFactory httpClientFactory = null,
                                         ILogger logger = null)
        {
            SerializationAdapter = serializationAdapter;
            HttpClientFactory = httpClientFactory;
            Logger = logger;
        }
        #endregion

        #region Implementation
        public IClient CreateClient(string name)
        {
            return new Client(
                SerializationAdapter,
                name,
                null,
                logger: Logger,
                httpClientFactory: HttpClientFactory);
        }
        #endregion
    }
}
