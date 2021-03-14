

using Microsoft.Extensions.Logging;
using RestClient.Net.Abstractions;
using RestClient.Net.Abstractions.Extensions;
using System;

namespace RestClient.Net
{
    public static class ClientExtensions
    {
        public static Client WithDefaultHeaders(this Client client, IHeadersCollection defaultRequestHeaders)
        =>
            client != null ? new Client(
            client.SerializationAdapter,
            client.Name,
            client.BaseUri,
            defaultRequestHeaders,
            client.logger is ILogger<Client> logger ? logger : null,
            client.createHttpClient,
            client.sendHttpRequestFunc,
            client.getHttpRequestMessage,
            client.Timeout,
            client.zip,
            client.ThrowExceptionOnFailure) : throw new ArgumentNullException(nameof(client));

        public static Client WithDefaultHeaders(this Client client, string key, string value)
            => WithDefaultHeaders(client, key.CreateHeadersCollection(value));
    }
}