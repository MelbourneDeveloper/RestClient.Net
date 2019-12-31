using RestClientDotNet.Abstractions;
using System;

#if NET45
using RestClientDotNet.Abstractions.Logging;
#else
using Microsoft.Extensions.Logging;
#endif


namespace RestClientDotNet
{
    public class RestClientFactory : IRestClientFactory
    {
        #region Public Properties
        public ISerializationAdapter SerializationAdapter { get; }
        public IHttpClientFactory HttpClientFactory { get; }
        public ILogger Logger { get; }
        #endregion

        #region Constructor
        public RestClientFactory(ISerializationAdapter serializationAdapter,
                                         IHttpClientFactory httpClientFactory,
                                         ILogger logger)
        {
            SerializationAdapter = serializationAdapter;
            HttpClientFactory = httpClientFactory;
            Logger = logger;
        }
        #endregion

        #region Implementation
        public IRestClient CreateRestClient(string name)
        {
            return new RestClient(
                SerializationAdapter,
                name,
                null,
                logger: Logger,
                httpClientFactory: HttpClientFactory);
        }

        public IRestClient CreateRestClient(string name, Uri baseUri)
        {
            return new RestClient(
                SerializationAdapter,
                name,
                baseUri,
                logger: Logger,
                httpClientFactory: HttpClientFactory);
        }
        #endregion
    }
}
