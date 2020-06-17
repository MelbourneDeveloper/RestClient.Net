using Microsoft.Extensions.DependencyInjection;
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
        }
    }
}
