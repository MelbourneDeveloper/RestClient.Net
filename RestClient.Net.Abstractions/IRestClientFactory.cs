using System;

namespace RestClientDotNet.Abstractions
{
    public interface IRestClientFactory
    {
        IRestClient CreateRestClient(string name);
        IRestClient CreateRestClient(string name, Uri baseUri);
    }
}
