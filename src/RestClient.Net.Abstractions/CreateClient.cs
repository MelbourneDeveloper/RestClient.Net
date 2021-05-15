using System;

namespace RestClient.Net.Abstractions
{
    public delegate IClient CreateClient(string name, Action<CreateClientOptions>? configureClient = null);

}
