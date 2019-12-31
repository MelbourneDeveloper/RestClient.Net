using RestClientDotNet.Abstractions;

namespace RestClientDotNet.UnitTests
{
    public class MockAspController
    {
        public IClient Client { get; }

        public MockAspController(IClientFactory restClientFactory)
        {
            Client = restClientFactory.CreateClient("test");
        }
    }
}