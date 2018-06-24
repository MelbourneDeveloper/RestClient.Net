using System;

namespace CF.RESTClientDotNet
{
    public class StandardRESTClientFactory : IRESTClientFactory
    {
        public ISerializationAdapter SerializationAdapter { get; }

        public StandardRESTClientFactory(ISerializationAdapter serializationAdapter)
        {
            SerializationAdapter = serializationAdapter;
        }

        public RESTClient CreateRESTClient(Uri baseUri)
        {
            return new RESTClient(SerializationAdapter, baseUri);
        }
    }
}
