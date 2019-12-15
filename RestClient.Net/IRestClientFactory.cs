using System;

namespace RestClientDotNet
{
    public interface IRestClientFactory
    {
        IRestClient CreateRestClient(Uri baseUri);
        IRestClient CreateRestClient();
    }
}
