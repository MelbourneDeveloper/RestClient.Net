using Microsoft.Extensions.DependencyInjection;
using RestClient.Net.Abstractions;
using System.Net.Http;

namespace RestClient.Net.DependencyInjection
{
    public static class Extensions
    {

        public static IServiceCollection AddDependencyInjectionMapping(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<CreateHttpClient>((sp) =>
            {
                var microsoftHttpClientFactoryWrapper = new MicrosoftHttpClientFactoryWrapper(sp.GetRequiredService<IHttpClientFactory>());
                return microsoftHttpClientFactoryWrapper.CreateClient;
            });

            serviceCollection.AddSingleton<CreateClient>((sp) =>
            {
                var clientFactory = new ClientFactory(sp.GetRequiredService<CreateHttpClient>(), sp.GetRequiredService<ISerializationAdapter>());
                return clientFactory.CreateClient;
            });

            return serviceCollection;
        }
    }
}
