using RestClientDotNet.Abstractions;
using System;

namespace RestClientDotNet
{
    public class StandardRestClientFactory : IRestClientFactory
    {
        #region Public Properties
        public ISerializationAdapter SerializationAdapter { get; }
        #endregion

        #region Constructor
        public StandardRestClientFactory(ISerializationAdapter serializationAdapter)
        {
            SerializationAdapter = serializationAdapter;
        }
        #endregion

        #region Implementation
        public IRestClient CreateRestClient(Uri baseUri)
        {
            return new RestClient(SerializationAdapter, baseUri);
        }

        public IRestClient CreateRestClient()
        {
            return new RestClient(SerializationAdapter);
        }
        #endregion
    }
}
