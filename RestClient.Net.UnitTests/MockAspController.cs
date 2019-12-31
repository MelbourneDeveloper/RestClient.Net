using RestClientDotNet.Abstractions;

namespace RestClientDotNet.UnitTests
{
    public class MockAspController
    {
        public IClient RestClient { get; }

        public MockAspController(IClientFactory restClientFactory)
        {
            RestClient = restClientFactory.CreateClient("test");
        }
    }
}