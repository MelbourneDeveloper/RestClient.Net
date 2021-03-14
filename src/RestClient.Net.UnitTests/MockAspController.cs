using RestClient.Net.Abstractions;
using System;

namespace RestClient.Net.UnitTests
{
    public class MockAspController
    {
        public IClient Client { get; }

        public MockAspController(CreateClient clientFactory) => Client = clientFactory != null ? clientFactory("test") : throw new ArgumentNullException(nameof(clientFactory));
    }
}