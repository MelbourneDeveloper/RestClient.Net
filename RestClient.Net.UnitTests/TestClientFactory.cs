using RestClientDotNet.Abstractions;
using System;
using System.Net.Http;
using RestClientDotNet;

namespace RestClientDotNet.UnitTests
{
    public class TestClientFactory : IHttpClientFactory
    {
        HttpClient _testClient;

        public TestClientFactory(HttpClient testClient)
        {
            _testClient = testClient;
        }

        public TimeSpan Timeout { get => _testClient.Timeout; set => _testClient.Timeout = value; }

        public Uri BaseUri => _testClient.BaseAddress;

        public IRestHeadersCollection DefaultRequestHeaders => new RestRequestHeadersCollection(_testClient.DefaultRequestHeaders);

        public HttpClient CreateHttpClient()
        {
            return _testClient;
        }
    }
}
