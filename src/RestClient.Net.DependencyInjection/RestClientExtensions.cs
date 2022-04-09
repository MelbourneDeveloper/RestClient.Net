using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;

namespace RestClient.Net
{
    public static class RestClientExtensions
    {
        /// <summary>
        /// Adds CreateClient to the container so you can mint IClients. Use this overload if you need to configure a singleton IClient
        /// </summary>
        /// <param name="serviceCollection"></param>
        /// <param name="configureSingletonClient">Specify this to configure a singleton IClient</param>
        /// <returns></returns>
        public static IServiceCollection AddRestClient(
            this IServiceCollection serviceCollection,
            Action<CreateClientOptions> configureSingletonClient)
            => AddRestClient(serviceCollection, configureSingletonClient: configureSingletonClient, createClient: null);

        /// <summary>
        /// Adds CreateClient to the container so you can mint IClients. Use this overload if you don't need to configure a singleton IClient (i.e. you will use CreateClient instead)
        /// </summary>
        /// <param name="serviceCollection"></param>
        /// <param name="configureSingletonClient">Specify this if you need to configure a singleton IClient</param>
        /// <param name="createClient">Specify this if you need to take control of construction</param>
        /// <returns></returns>
        public static IServiceCollection AddRestClient(
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
            });

            _ = serviceCollection.AddSingleton((sp) =>
            sp.GetRequiredService<CreateClient>()("RestClient", configureSingletonClient));

            return serviceCollection;
        }
    }
}
