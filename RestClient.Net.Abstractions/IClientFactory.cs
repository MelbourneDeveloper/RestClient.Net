using System;

namespace RestClientDotNet.Abstractions
{
    public interface IClientFactory
    {
        IClient CreateRestClient(string name);
        IClient CreateRestClient(string name, Uri baseUri);
    }
}
