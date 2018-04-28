using System;

namespace CF.RESTClientDotNet
{
    public interface IRESTClientFactory
    {
        RESTClient CreateRESTClient(Uri baseUri, ISerializationAdapter serializationAdapter);
    }
}
