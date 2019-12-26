using System.Net.Http;

namespace RestClientDotNet
{
    /// <summary>
    /// Generates HttpClients as necessary
    /// Base on this Microsoft interface which is only available in .Net Core 2.1 +: https://docs.microsoft.com/en-us/dotnet/api/system.net.http.ihttpclientfactory?view=dotnet-plat-ext-3.1
    /// </summary>
    public interface IHttpClientFactory
    {
        HttpClient CreateClient(string name);
    }
}
