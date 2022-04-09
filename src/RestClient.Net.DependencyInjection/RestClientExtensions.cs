using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;

namespace RestClient.Net
{
    public static class RestClientExtensions
    {
        [Obsolete("The configureClient parameter in this overload is misleading because it only affects one client. The next version will not have this overload. Please consider using the other overload.")]
        public static IServiceCollection AddRestClient(
            this IServiceCollection serviceCollection,
            Action<CreateClientOptions> configureSingletonClient)
            => AddRestClient(serviceCollection, configureSingletonClient: configureSingletonClient, createClient: null);

        /// <summary>
        /// Adds a CreateClient function to get or mint clients. Allows you to mint clients with createClient if you need to take control of construction
        /// </summary>
        public static IServiceCollection AddRestClient(
            this IServiceCollection serviceCollection,
            Func<string, CreateClientOptions, IServiceProvider, IClient>? createClient = null)
            => AddRestClient(serviceCollection, configureSingletonClient: null, createClient: createClient);

        private static IServiceCollection AddRestClient(
            this IServiceCollection serviceCollection,
            Action<CreateClientOptions>? configureSingletonClient = null,
            Func<string, CreateClientOptions, IServiceProvider, IClient>? createClient = null
            )
        {
            _ = serviceCollection
            .AddSingleton<CreateHttpClient>((sp) =>
            {
                var microsoftHttpClientFactoryWrapper = new MicrosoftHttpClientFactoryWrapper(sp.GetRequiredService<IHttpClientFactory>());
                return microsoftHttpClientFactoryWrapper.CreateClient;
            })
            .AddSingleton<CreateClient>((sp) =>
            {
                var clientFactory = new ClientFactory(
                    sp.GetRequiredService<CreateHttpClient>(),
                    sp.GetService<ILoggerFactory>(),
                    createClient != null ? (name, options) =>
                    createClient.Invoke(name, options, sp) : null
                    );

                return clientFactory.CreateClient;
            })
            .AddSingleton((sp) => sp.GetRequiredService<CreateClient>()("RestClient", configureSingletonClient));

            return serviceCollection;
        }
    }
}
