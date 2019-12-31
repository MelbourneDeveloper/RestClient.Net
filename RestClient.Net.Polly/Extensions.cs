using Microsoft.Extensions.DependencyInjection;
using RestClient.Net;

namespace RestClient.Net.Polly
{
    public static class Extensions
    {
        public static void AddDependencyInjectionMapping(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton(typeof(IHttpClientFactory), typeof(MicrosoftHttpClientFactoryWrapper));
        }
    }
}
