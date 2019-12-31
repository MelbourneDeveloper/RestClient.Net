using RestClientDotNet.Abstractions;

namespace RestClientDotNet.UnitTests
{
    public class MockAspController
    {
        public IClient RestClient { get; }

        public MockAspController(IRestClientFactory restClientFactory)
        {
            RestClient = restClientFactory.CreateRestClient("test");
        }
    }
}