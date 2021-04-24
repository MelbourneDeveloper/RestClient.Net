using RestClient.Net.Abstractions;
using System;
using System.Collections.Generic;
using Urls;

namespace RestClient.Net.DI
{
    public class ClientBuilderOptions
    {
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public ClientBuilderOptions(CreateHttpClient createHttpClient) => CreateHttpClient = createHttpClient;
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public IList<Action<ClientBuilder>> HttpMessageHandlerBuilderActions { get; } = new List<Action<ClientBuilder>>();
        public AbsoluteUrl BaseUrl { get; set; } = AbsoluteUrl.Empty;
#if !NET45
        public ISerializationAdapter SerializationAdapter { get; set; } = JsonSerializationAdapter.Instance;
#else
        public ISerializationAdapter SerializationAdapter { get; set; }
#endif
        public CreateHttpClient CreateHttpClient { get; set; }
    }
}
