using RestClientDotNet.Abstractions;
using System;

#if NETSTANDARD2_0 
using System.Net.Http;
#endif

namespace RestClientDotNet
{
    public class RestClientFactory : IRestClientFactory
    {
        #region Public Properties
        public ISerializationAdapter SerializationAdapter { get; }
        public IHttpClientFactory HttpClientFactory { get; }
        public ITracer Tracer { get; }
        #endregion

        #region Constructor
        public RestClientFactory(ISerializationAdapter serializationAdapter,
                                         IHttpClientFactory httpClientFactory,
                                         ITracer tracer)
        {
            SerializationAdapter = serializationAdapter;
            HttpClientFactory = httpClientFactory;
            Tracer = tracer;
        }
        #endregion

        #region Implementation
        public IRestClient CreateRestClient(string name)
        {
            return new RestClient(
                name,
                null,
                SerializationAdapter,
                Tracer,
                default,
                HttpClientFactory,
                null,
                null,
                null
                );
        }

        public IRestClient CreateRestClient(string name, Uri baseUri)
        {
            return new RestClient(
                name,
                baseUri,
                SerializationAdapter,
                Tracer,
                default,
                HttpClientFactory,
                null,
                null,
                null
                );
        }
        #endregion
    }
}
