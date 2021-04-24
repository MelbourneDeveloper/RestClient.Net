using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RestClient.Net.Abstractions;
using RestClient.Net.DI;
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
                    sp.GetService<ILoggerFactory>());

                return new CreateClient((a, b) => clientFactory.CreateClient("asdasds", a));
            });

            return serviceCollection;
        }


        public static IClientBuilder ConfigurePrimaryClient(this IClientBuilder builder, Func<IClient> configureClient)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            if (configureClient == null)
            {
                throw new ArgumentNullException(nameof(configureClient));
            }

            _ = builder.Services.Configure<ClientBuilderOptions>(builder.Name, options =>
              {
                  options.HttpMessageHandlerBuilderActions.Add(b => b.PrimaryClient = configureClient());
              });

            return builder;
        }
    }



    public interface IClientBuilder
    {
        string Name { get; }
        IServiceCollection Services { get; }
    }
}
