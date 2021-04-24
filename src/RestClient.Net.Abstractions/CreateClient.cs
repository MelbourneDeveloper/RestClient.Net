using RestClient.Net.DI;
using System;

namespace RestClient.Net.Abstractions
{
    /// <summary>
    /// Dependency Injection abstraction for creating and managing rest clients. Use this abstraction when more than one rest client is needed for the application.
    /// </summary>
    public delegate IClient CreateClient(string name, Action<ClientBuiderOptions> configureClient);
}
