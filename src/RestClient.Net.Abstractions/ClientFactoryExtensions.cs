using System;

namespace RestClient.Net.Abstractions
{
    public static class ClientFactoryExtensions
    {
        public static IClient CreateClient(this CreateClient clientFactory, string name, Uri? baseUri = null)
        {
            if (clientFactory == null) throw new ArgumentNullException(nameof(clientFactory));
            var client = clientFactory(name, baseUri);
            return client;
        }
    }
}
