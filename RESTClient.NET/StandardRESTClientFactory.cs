using System;

namespace CF.RESTClientDotNet
{
    public class StandardRESTClientFactory : IRESTClientFactory
    {
        public RESTClient CreateRESTClient(Uri baseUri)
        {
            return new RESTClient(baseUri);
        }
    }
}
