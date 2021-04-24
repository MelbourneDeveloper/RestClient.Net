using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RestClient.Net.Abstractions;
using System;
using System.Collections.Generic;
using System.Net.Http;
using Urls;

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
                  options.ClientBuildActions.Add(b => b.PrimaryClient = configureClient());
              });

            return builder;
        }
    }

    public class ClientBuilderOptions
    {
        public IList<Action<ClientBuilder>> ClientBuildActions { get; } = new List<Action<ClientBuilder>>();
        public AbsoluteUrl BaseUrl { get; set; } = AbsoluteUrl.Empty;
    }

    public abstract class ClientBuilder
    {
        protected ClientBuilder(IClient primaryClient) => PrimaryClient = primaryClient;

        public IClient PrimaryClient { get; internal set; }

        public abstract IClient Build();
    }

    public interface IClientBuilder
    {
        string Name { get; }
        IServiceCollection Services { get; }
    }
}
