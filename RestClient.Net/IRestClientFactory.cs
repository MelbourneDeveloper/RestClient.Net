using System;

namespace RestClientDotNet
{
    public interface IRestClientFactory
    {
        IRestClient CreateRESTClient(Uri baseUri);
    }
}
