using Microsoft.Extensions.DependencyInjection;
using System;

namespace RestClient.Net.DependencyInjection
{
    public static class Extensions
    {
        public static void AddDependencyInjectionMapping(this IServiceCollection serviceCollection)
        {
            throw new NotImplementedException();
            //serviceCollection.AddSingleton(typeof(CreateHttpClient), typeof(MicrosoftHttpClientFactoryWrapper));
        }
    }
}
