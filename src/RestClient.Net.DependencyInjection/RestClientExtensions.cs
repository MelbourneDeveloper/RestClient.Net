using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RestClient.Net.DI;
using System;
using System.Net.Http;

namespace RestClient.Net.DependencyInjection
{
    public static class RestClientExtensions
    {

        public static IServiceCollection AddRestClient(this IServiceCollection serviceCollection, Action<ClientBuilderOptions>? configureClient = null)
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
                    sp.GetService<ILoggerFactory>());

                return clientFactory.CreateClient;
            })
            .AddSingleton((sp) => sp.GetRequiredService<CreateClient>()("RestClient", configureClient));

            return serviceCollection;
        }


        //public static IClientBuilder ConfigurePrimaryClient(this IClientBuilder builder, Func<IClient> configureClient)
        //{
        //    if (builder == null)
        //    {
        //        throw new ArgumentNullException(nameof(builder));
        //    }

        //    if (configureClient == null)
        //    {
        //        throw new ArgumentNullException(nameof(configureClient));
        //    }

        //    _ = builder.Services.Configure<ClientBuilderOptions>(builder.Name, options =>
        //      {
        //          options.HttpMessageHandlerBuilderActions.Add(b => b.PrimaryClient = configureClient());
        //      });

        //    return builder;
        //}
    }



    public interface IClientBuilder
    {
        string Name { get; }
        IServiceCollection Services { get; }
    }
}
