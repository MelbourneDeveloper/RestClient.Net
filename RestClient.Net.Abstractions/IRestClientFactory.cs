using System;

namespace RestClientDotNet.Abstractions
{
    public interface IRestClientFactory
    {
        IClient CreateRestClient(string name);
        IClient CreateRestClient(string name, Uri baseUri);
    }
}
