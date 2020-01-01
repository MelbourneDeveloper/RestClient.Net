using Microsoft.Extensions.DependencyInjection;

namespace RestClient.Net.DependencyInjection
{
    public static class Extensions
    {
        public static void AddDependencyInjectionMapping(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton(typeof(IHttpClientFactory), typeof(MicrosoftHttpClientFactoryWrapper));
        }
    }
}
