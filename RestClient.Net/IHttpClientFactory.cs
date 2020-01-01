using System.Net.Http;

namespace RestClient.Net
{
    /// <summary>
    /// A factory abstraction for a component that can create System.Net.Http.HttpClient instances with custom configuration for a given logical name.
    /// Note: this is based on the Microsoft interface by the same name. It is only available in .Net Core 2.1 +: https://docs.microsoft.com/en-us/dotnet/api/system.net.http.ihttpclientfactory?view=dotnet-plat-ext-3.1. 
    /// This interface is duplicated so that this framework is decoupled from Microsoft.Extensions.Http and its dependencies while still allowing for Microsoft Dependency Injection with a simple map. 
    /// </summary>
    public interface IHttpClientFactory
    {
        HttpClient CreateClient(string name);
    }
}
