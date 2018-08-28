using System;

namespace RestClientDotNet
{
    public class StandardRestClientFactory : IRestClientFactory
    {
        public ISerializationAdapter SerializationAdapter { get; }

        public StandardRestClientFactory(ISerializationAdapter serializationAdapter)
        {
            SerializationAdapter = serializationAdapter;
        }

        public RestClient CreateRESTClient(Uri baseUri)
        {
            return new RestClient(SerializationAdapter, baseUri);
        }
    }
}
