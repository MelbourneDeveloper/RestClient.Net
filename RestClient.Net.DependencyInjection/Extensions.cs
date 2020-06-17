using Microsoft.Extensions.DependencyInjection;
using RestClient.Net.Abstractions;
using System;
using System.Net.Http;

namespace RestClient.Net.DependencyInjection
{
    public static class Extensions
    {
        public static void AddDependencyInjectionMapping(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<CreateHttpClient>((sp) => 
            {
                var microsoftHttpClientFactoryWrapper = new MicrosoftHttpClientFactoryWrapper(sp.GetRequiredService<IHttpClientFactory>());
                return microsoftHttpClientFactoryWrapper.CreateClient; 
            } );

            serviceCollection.AddSingleton<CreateClient>((sp) =>
            {
                var clientFactory = new ClientFactory(sp.GetRequiredService<ISerializationAdapter>(), sp.GetRequiredService<CreateHttpClient>());
                return clientFactory.CreateClient;
            });

        }
    }
}
