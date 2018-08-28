using System;

namespace RestClientDotNet
{
    public class StandardRESTClientFactory : IRestClientFactory
    {
        public ISerializationAdapter SerializationAdapter { get; }

        public StandardRESTClientFactory(ISerializationAdapter serializationAdapter)
        {
            SerializationAdapter = serializationAdapter;
        }

        public RestClient CreateRESTClient(Uri baseUri)
        {
            return new RestClient(SerializationAdapter, baseUri);
        }
    }
}
