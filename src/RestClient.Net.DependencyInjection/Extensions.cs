using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RestClient.Net.Abstractions;
using System;
using System.Net.Http;

namespace RestClient.Net.DependencyInjection
{
    public static class Extensionss
    {

        public static IServiceCollection AddRestClient(this IServiceCollection serviceCollection, ISerializationAdapter? serializationAdapter = null)
        {
            serializationAdapter ??= new JsonSerializationAdapter();

            _ = serviceCollection
            .AddSingleton(serializationAdapter)
            .AddSingleton<CreateHttpClient>((sp) =>
            {
                var microsoftHttpClientFactoryWrapper = new MicrosoftHttpClientFactoryWrapper(sp.GetRequiredService<IHttpClientFactory>());
                return microsoftHttpClientFactoryWrapper.CreateClient;
            })
            .AddSingleton<CreateClient>((sp) =>
            {
                var clientFactory = new ClientFactory(
                    sp.GetRequiredService<CreateHttpClient>(),
                    sp.GetRequiredService<ISerializationAdapter>(),
                    sp.GetService<ILoggerFactory>());
                return clientFactory.CreateClient;
            });

            return serviceCollection;
        }

        public static IServiceCollection AddRestClient<T, TI>(this IServiceCollection serviceCollection, Func<IClient> createClient)
        {
#pragma warning disable IDE0022 // Use expression body for methods
            return serviceCollection;
#pragma warning restore IDE0022 // Use expression body for methods
        }
    }
}
