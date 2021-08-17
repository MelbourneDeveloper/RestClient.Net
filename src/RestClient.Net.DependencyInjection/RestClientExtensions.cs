using Microsoft.Extensions.DependencyInjection;
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
                .AddSingleton<ClientFactory>()
                .AddSingleton<CreateClient>((sp) =>
                {
                    return sp.GetService<ClientFactory>().CreateClient;
                });
        }

        public static IServiceCollection AddRestClient(this IServiceCollection services)
            => services.AddRestClient("RestClient");

        public static IServiceCollection AddRestClient(this IServiceCollection services, Action<CreateClientOptions> configureClient)
            => services.AddRestClient("RestClient", configureClient);

        public static IServiceCollection AddRestClient(this IServiceCollection services, Action<CreateClientOptions, IServiceProvider> configureClient)
            => services.AddRestClient("RestClient", configureClient);

        public static IServiceCollection AddRestClient(this IServiceCollection services, string name)
        {
            return AddRestClient_Base(services)
                .AddSingleton(sp => sp.GetRequiredService<CreateClient>()(name, null));
        }

        public static IServiceCollection AddRestClient(this IServiceCollection services, string name, Action<CreateClientOptions> configureClient)
        {
            return AddRestClient_Base(services)
                .AddSingleton(sp => sp.GetRequiredService<CreateClient>()(name, configureClient));
        }

        public static IServiceCollection AddRestClient(this IServiceCollection services, string name, Action<CreateClientOptions, IServiceProvider> configureClient)
        {
            return AddRestClient_Base(services)
                .AddSingleton(sp => sp.GetRequiredService<CreateClient>()(name, options => configureClient(options, sp)));
        }
    }
}
