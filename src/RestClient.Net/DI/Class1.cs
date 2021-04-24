using RestClient.Net.Abstractions;
using System;

namespace RestClient.Net.DI
{
    public delegate IClient CreateClient2(string name, Action<ClientBuilderOptions> configureClient);

}
