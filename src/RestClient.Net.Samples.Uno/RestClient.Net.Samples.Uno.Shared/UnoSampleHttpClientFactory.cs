using System.Net.Http;

#if __WASM__
using Uno.UI.Wasm;
#endif

namespace RestClient.Net.Samples.Uno.Shared
{
    public class UnoSampleHttpClientFactory 
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

        public HttpClient CreateClient(string name)
        {
            return httpClient;
        }
    }
}
