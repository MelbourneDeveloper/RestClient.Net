using System;
using RestClientDotNet.Abstractions;

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
        public IRestClient CreateRestClient()
        {
            return new RestClient(
                SerializationAdapter,
                HttpClientFactory,
                Tracer,
                null);
        }

        public IRestClient CreateRestClient(Uri baseUri)
        {
            return new RestClient(
                SerializationAdapter,
                HttpClientFactory,
                Tracer,
                baseUri);
        }
        #endregion
    }
}
