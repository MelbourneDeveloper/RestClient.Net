using System;

namespace RestClientDotNet.Abstractions
{
    public static class ClientFactoryExtensions
    {
        public static IClient CreateRestClient(this IClientFactory restClientFactory)
        {
            if (restClientFactory == null) throw new ArgumentNullException(nameof(restClientFactory));
            return restClientFactory.CreateRestClient("RestClient");
        }
    }
}
