using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;

namespace RestClient.Net
{
    public static class RestClientExtensions
    {
        private static IServiceCollection AddRestClient_Base(this IServiceCollection services)
        {
            return services
                .AddSingleton<CreateHttpClient>((sp) =>
                {
                    var microsoftHttpClientFactoryWrapper = new MicrosoftHttpClientFactoryWrapper(sp.GetRequiredService<IHttpClientFactory>());
                    return microsoftHttpClientFactoryWrapper.CreateClient;
                })
                .AddSingleton<CreateClient>((sp) =>
                {
                    var clientFactory = new ClientFactory(
                        sp.GetRequiredService<CreateHttpClient>(),
                        sp.GetService<ILoggerFactory>());

                    return clientFactory.CreateClient;
                });
        }

        public static IServiceCollection AddRestClient(this IServiceCollection services)
        {
            return AddRestClient_Base(services)
                .AddSingleton(sp => sp.GetRequiredService<CreateClient>()("RestClient", null));
        }

        public static IServiceCollection AddRestClient(this IServiceCollection services, Action<CreateClientOptions> configureClient)
        {
            return AddRestClient_Base(services)
                .AddSingleton(sp => sp.GetRequiredService<CreateClient>()("RestClient", configureClient));
        }

        public static IServiceCollection AddRestClient(this IServiceCollection services, Action<CreateClientOptions, IServiceProvider> configureClient)
        {
            return AddRestClient_Base(services)
                .AddSingleton(sp => sp.GetRequiredService<CreateClient>()("RestClient", options => configureClient(options, sp)));
        }
    }
}
