using System;

namespace RestClientDotNet.Abstractions
{
    public interface IRestClientFactory
    {
        IRestClient CreateRestClient(string name);
    }

    public static class RestClientFactoryExtensions
    {
        public static IRestClient CreateRestClient(this IRestClient restClient, Uri baseUri)
        {
            restClient.CreateRestClient("RestClient");
        }
    }
}
