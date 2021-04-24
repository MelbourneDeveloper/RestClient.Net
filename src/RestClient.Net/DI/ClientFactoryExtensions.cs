using RestClient.Net.DI;
using System;

namespace RestClient.Net.Abstractions
{
    public static class ClientFactoryExtensions
    {
        public static IClient CreateClient(this CreateClient2 createClient, string name, Action<ClientBuilderOptions> configureClient)
        {
            if (createClient == null) throw new ArgumentNullException(nameof(createClient));
            var client = createClient(name, configureClient);
            return client;
        }
    }
}
