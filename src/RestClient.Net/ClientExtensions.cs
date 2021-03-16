

using Microsoft.Extensions.Logging;
using RestClient.Net.Abstractions;
using RestClient.Net.Abstractions.Extensions;
using System;

namespace RestClient.Net
{
    public static class ClientExtensions
    {

        #region Public Methods

        public static Client WithBaseUri(this Client client, Uri baseUri)
        =>
            client != null ? new Client(
            client.SerializationAdapter,
            client.Name,
            baseUri,
            client.DefaultRequestHeaders,
            client.logger is ILogger<Client> logger ? logger : null,
            client.createHttpClient,
            client.sendHttpRequest,
            client.getHttpRequestMessage,
            client.Timeout,
            client.zip,
        public static Client With(this Client client, IHeadersCollection defaultRequestHeaders)
            client.ThrowExceptionOnFailure) : throw new ArgumentNullException(nameof(client));
        =>
            client != null ? new Client(
            client.SerializationAdapter,
            client.Name,
            client.BaseUri,
            defaultRequestHeaders,
            client.logger is ILogger<Client> logger ? logger : null,
            client.createHttpClient,
            client.sendHttpRequest,
            client.getHttpRequestMessage,
            client.Timeout,
            client.zip,
            client.ThrowExceptionOnFailure) : throw new ArgumentNullException(nameof(client));

        public static Client With(this Client client, string key, string value)
            => With(client, key.CreateHeadersCollection(value));

        public static Client WithLogger(this Client client, ILogger<Client> logger)
        =>
            client != null ? new Client(
            client.SerializationAdapter,
            client.Name,
            client.BaseUri,
            client.DefaultRequestHeaders,
            logger,
            client.createHttpClient,
            client.sendHttpRequest,
            client.getHttpRequestMessage,
            client.Timeout,
            client.zip,
            client.ThrowExceptionOnFailure) : throw new ArgumentNullException(nameof(client));

        public static Client WithSerializationAdapter(this Client client, ISerializationAdapter serializationAdapter)
                                        =>
            client != null ? new Client(
            serializationAdapter,
            client.Name,
            client.BaseUri,
            client.DefaultRequestHeaders,
            client.logger is ILogger<Client> logger ? logger : null,
            client.createHttpClient,
            client.sendHttpRequest,
            client.getHttpRequestMessage,
            client.Timeout,
            client.zip,
            client.ThrowExceptionOnFailure) : throw new ArgumentNullException(nameof(client));

        #endregion Public Methods
    }
}