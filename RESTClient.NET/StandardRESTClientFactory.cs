using System;

namespace CF.RESTClientDotNet
{
    public class StandardRESTClientFactory : IRESTClientFactory
    {
        public RESTClient CreateRESTClient(Uri baseUri, ISerializationAdapter serializationAdapter)
        {
            return new RESTClient(baseUri, serializationAdapter);
        }
    }
}
