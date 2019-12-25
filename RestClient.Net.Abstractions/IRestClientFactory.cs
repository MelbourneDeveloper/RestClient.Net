using System;

namespace RestClientDotNet.Abstractions
{
    public interface IRestClientFactory
    {
        IRestClient CreateRestClient();
        IRestClient CreateRestClient(Uri baseUri);
    }
}
