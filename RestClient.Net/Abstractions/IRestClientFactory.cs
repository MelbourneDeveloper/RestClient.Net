using System;

namespace RestClientDotNet.Abstractions
{
    public interface IRestClientFactory
    {
        IRestClient CreateRestClient(Uri baseUri);
        IRestClient CreateRestClient();
    }
}
