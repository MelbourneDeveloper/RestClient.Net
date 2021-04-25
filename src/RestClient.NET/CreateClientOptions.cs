using RestClient.Net.Abstractions;
using Urls;

namespace RestClient.Net
{
    public class CreateClientOptions

    {
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public CreateClientOptions(CreateHttpClient createHttpClient) => CreateHttpClient = createHttpClient;
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public AbsoluteUrl BaseUrl { get; set; } = AbsoluteUrl.Empty;
#if !NET45
        public ISerializationAdapter SerializationAdapter { get; set; } = JsonSerializationAdapter.Instance;
#else
        public ISerializationAdapter SerializationAdapter { get; set; }
#endif
        public CreateHttpClient CreateHttpClient { get; set; }

        public ISendHttpRequestMessage SendHttpRequestMessage { get; set; }
        public IGetHttpRequestMessage GetHttpRequestMessage { get; set; }
        public IHeadersCollection HeadersCollection { get; set; }
        public bool ThrowExceptionOnFailure { get; set; }
    }
}