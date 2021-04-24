#if ! NET45

using RestClient.Net.Abstractions;
using System;
using System.Collections.Generic;
using Urls;

namespace RestClient.Net.DI
{
    public class ClientBuilderOptions
    {
        public ClientBuilderOptions(CreateHttpClient createHttpClient) => CreateHttpClient = createHttpClient;

        public IList<Action<ClientBuilder>> HttpMessageHandlerBuilderActions { get; } = new List<Action<ClientBuilder>>();
        public AbsoluteUrl BaseUrl { get; set; } = AbsoluteUrl.Empty;
        public ISerializationAdapter SerializationAdapter { get; set; } = JsonSerializationAdapter.Instance;
        public CreateHttpClient CreateHttpClient { get; set; }
    }
}

#endif