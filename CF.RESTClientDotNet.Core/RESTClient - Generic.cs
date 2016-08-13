using System;

namespace CF.RESTClientDotNet
{
    public partial class RESTClient<ExceptionT> : RESTClient
    {
        public RESTClient(ISerializationAdapter serializationAdapter, Uri baseUri) : base(serializationAdapter, baseUri)
        {

        }
    }
}
