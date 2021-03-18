using System;

namespace RestClient.Net.Abstractions
{
    public static class ClientFactoryExtensions
    {
        public static IClient CreateClient(this CreateClient createClient, string name, Uri? baseUri = null)
        {
            if (createClient == null) throw new ArgumentNullException(nameof(createClient));
            var client = createClient(name, baseUri);
            return client;
        }
    }
}
