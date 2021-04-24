using RestClient.Net.Abstractions;
using System;

namespace RestClient.Net.DI
{
    public delegate IClient CreateClient(string name, Action<ClientBuilderOptions>? configureClient = null);

}
