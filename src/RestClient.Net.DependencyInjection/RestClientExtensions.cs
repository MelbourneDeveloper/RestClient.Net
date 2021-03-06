﻿using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;

namespace RestClient.Net
{
    public static class RestClientExtensions
    {
        public static IServiceCollection AddRestClient(this IServiceCollection serviceCollection, Action<CreateClientOptions>? configureClient = null)
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
    }
}
