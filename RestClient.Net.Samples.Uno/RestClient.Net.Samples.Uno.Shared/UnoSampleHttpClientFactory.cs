using RestClientDotNet;
using RestClientDotNet.Abstractions;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace RestClient.Net.Samples.Uno.Shared
{
    public class UnoSampleHttpClientFactory : IHttpClientFactory
    {
        HttpClient httpClient;

        public UnoSampleHttpClientFactory(Uri uri)
        {
#if __WASM__
            httpClient = new HttpClient(new WasmHttpHandler());
#else
            httpClient = new HttpClient();
#endif
            httpClient.BaseAddress = uri;
        }


        public TimeSpan Timeout { get => httpClient.Timeout; set => httpClient.Timeout = value; }

        public Uri BaseUri => httpClient.BaseAddress;

        public IRestHeadersCollection DefaultRequestHeaders => new RestRequestHeadersCollection(httpClient.DefaultRequestHeaders);

        public HttpClient CreateHttpClient()
        {
            return httpClient;
        }

        public void Dispose()
        {
            httpClient.Dispose();
        }
    }
}
