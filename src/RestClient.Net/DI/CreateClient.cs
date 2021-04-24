using RestClient.Net.Abstractions;
using System;

namespace RestClient.Net
{
    public delegate IClient CreateClient(string name, Action<ClientBuilderOptions>? configureClient = null);

}
