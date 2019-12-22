using RestClientDotNet.Abstractions;
using System;
using System.Net.Http;

namespace RestClientDotNet
{
    public class StandardRestClientFactory : IRestClientFactory
    {
        #region Public Properties
        public ISerializationAdapter SerializationAdapter { get; }
        public IHttpClientFactory HttpClientFactory { get; }
        #endregion

        #region Constructor
        public StandardRestClientFactory(ISerializationAdapter serializationAdapter, IHttpClientFactory httpClientFactory)
        {
            SerializationAdapter = serializationAdapter;
            HttpClientFactory = httpClientFactory;
        }
        #endregion

        #region Implementation
        public IRestClient CreateRestClient(Uri baseUri)
        {
            return new RestClient(SerializationAdapter, baseUri, default, null, HttpClientFactory, null);
        }

        public IRestClient CreateRestClient()
        {
            return new RestClient(SerializationAdapter, null, default, null, HttpClientFactory, null);
        }
        #endregion
    }
}
