using System;

namespace RestClientDotNet.Abstractions
{
    public static class RestClientFactoryExtensions
    {
        public static IRestClient CreateRestClient(this IRestClientFactory restClientFactory)
        {
            if (restClientFactory == null) throw new ArgumentNullException(nameof(restClientFactory));
            return restClientFactory.CreateRestClient("RestClient");
        }
    }
}
