using RestClient.Net.Abstractions;
using System;
using System.Net.Http;

#if __WASM__
using Uno.UI.Wasm;
#endif

namespace RestClient.Net.Samples.Uno.Shared
{
    public class UnoSampleHttpClientFactory : IHttpClientFactory
    {
        HttpClient httpClient;

        public UnoSampleHttpClientFactory()
        {
#if __WASM__
            httpClient = new HttpClient(new WasmHttpHandler());
#else
            httpClient = new HttpClient();
#endif
        }

        public TimeSpan Timeout { get => httpClient.Timeout; set => httpClient.Timeout = value; }

        public IHeadersCollection DefaultRequestHeaders => new RequestHeadersCollection();

        public HttpClient CreateClient(string name)
        {
            return httpClient;
        }

        public void Dispose()
        {
            httpClient.Dispose();
        }
    }
}
