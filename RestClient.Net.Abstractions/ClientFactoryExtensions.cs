using System;

namespace RestClient.Net.Abstractions
{
    public static class ClientFactoryExtensions
    {
        public static IClient CreateClient(this IClientFactory clientFactory)
        {
            //Use 'RestClient' by default because if this is null, the dictionary fails,
            //If this is random, then many clients will get created
            return CreateClient(clientFactory, "RestClient", null);
        }

        public static IClient CreateClient(this IClientFactory clientFactory, string name, Uri baseUri)
        {
            if (clientFactory == null) throw new ArgumentNullException(nameof(clientFactory));
            var client = clientFactory.CreateClient(name);
            client.BaseUri = baseUri;
            return client;
        }
    }
}
