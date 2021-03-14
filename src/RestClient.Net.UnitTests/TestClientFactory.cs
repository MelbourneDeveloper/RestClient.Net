using RestClient.Net.Abstractions;
using System;
using System.Net.Http;

#pragma warning disable CA1801 // Review unused parameters
#pragma warning disable IDE0060 

namespace RestClient.Net.UnitTests
{
    public class TestClientFactory
    {
        private readonly HttpClient _testClient;

        public TestClientFactory(HttpClient testClient) => _testClient = testClient;

        public TimeSpan Timeout { get => _testClient.Timeout; set => _testClient.Timeout = value; }

        public IHeadersCollection DefaultRequestHeaders { get; } = NullHeadersCollection.Instance;

        public HttpClient CreateClient(string name) => _testClient;

        public void Dispose() => _testClient.Dispose();
    }
}
