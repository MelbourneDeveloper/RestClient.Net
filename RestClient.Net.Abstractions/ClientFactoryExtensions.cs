using System;

namespace RestClientDotNet.Abstractions
{
    public static class ClientFactoryExtensions
    {
        public static IClient CreateClient(this IClientFactory restClientFactory)
        {
            return CreateClient(restClientFactory, "RestClient", null);
        }

        public static IClient CreateClient(this IClientFactory restClientFactory, string name, Uri baseUri)
        {
            if (restClientFactory == null) throw new ArgumentNullException(nameof(restClientFactory));
            var client = restClientFactory.CreateClient(name);
            client.BaseUri = baseUri;
            return client;
        }
    }
}
