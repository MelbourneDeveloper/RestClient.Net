using RestClientDotNet;
using System.Net.Http;

#if __WASM__
using Uno.UI.Wasm;
#endif

namespace RestClient.Net.Samples.Uno.Shared
{
    public class UnoSampleHttpClientFactory : SingletonHttpClientFactory
    {
        public UnoSampleHttpClientFactory() : base
            (
#if __WASM__
            new HttpClient(new WasmHttpHandler())
#else
            new HttpClient()
#endif
            )
        {
        }
    }
}
