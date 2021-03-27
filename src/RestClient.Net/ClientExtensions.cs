

using Microsoft.Extensions.Logging;
using RestClient.Net.Abstractions;
using RestClient.Net.Abstractions.Extensions;
using System;

namespace RestClient.Net
{
    public static class ClientExtensions
    {

        #region Public Methods

        /// <summary>
        /// Clones the client With a change
        /// </summary>
        public static Client With(this Client client, Uri baseUri)
        =>
            client != null ? new Client(
            client.SerializationAdapter,
            baseUri,
            client.DefaultRequestHeaders,
            client.logger is ILogger<Client> logger ? logger : null,
            client.createHttpClient,
            client.sendHttpRequestMessage,
            client.getHttpRequestMessage,
            client.ThrowExceptionOnFailure,
            client.Name) : throw new ArgumentNullException(nameof(client));

        /// <summary>
        /// Clones the client With a change
        /// </summary>
        public static Client With(this Client client, IHeadersCollection defaultRequestHeaders)
        =>
            client != null ? new Client(
            client.SerializationAdapter,
            client.BaseUri,
            defaultRequestHeaders,
            client.logger is ILogger<Client> logger ? logger : null,
            client.createHttpClient,
            client.sendHttpRequestMessage,
            client.getHttpRequestMessage,
            client.ThrowExceptionOnFailure,
            client.Name) : throw new ArgumentNullException(nameof(client));

        /// <summary>
        /// Clones the client With a change
        /// </summary>
        public static Client WithDefaultRequestHeaders(this Client client, string key, string value)
            => With(client, key.CreateHeadersCollection(value));

        /// <summary>
        /// Clones the client With a change
        /// </summary>
        public static Client With(this Client client, ILogger<Client> logger)
        =>
            client != null ? new Client(
            client.SerializationAdapter,
            client.BaseUri,
            client.DefaultRequestHeaders,
            logger,
            client.createHttpClient,
            client.sendHttpRequestMessage,
            client.getHttpRequestMessage,
            client.ThrowExceptionOnFailure,
            client.Name) : throw new ArgumentNullException(nameof(client));

        /// <summary>
        /// Clones the client With a change
        /// </summary>
        public static Client With(this Client client, ISerializationAdapter serializationAdapter)
                                        =>
            client != null ? new Client(
            serializationAdapter,
            client.BaseUri,
            client.DefaultRequestHeaders,
            client.logger is ILogger<Client> logger ? logger : null,
            client.createHttpClient,
            client.sendHttpRequestMessage,
            client.getHttpRequestMessage,
            client.ThrowExceptionOnFailure,
            client.Name) : throw new ArgumentNullException(nameof(client));

        /// <summary>
        /// Clones the client With a change
        /// </summary>
        public static Client With(this Client client, CreateHttpClient createHttpClient)
            =>
                client != null ? new Client(
                    client.SerializationAdapter,
                    client.BaseUri,
                    client.DefaultRequestHeaders,
                    client.logger is ILogger<Client> logger ? logger : null,
                    createHttpClient,
                    client.sendHttpRequestMessage,
                    client.getHttpRequestMessage,
                    client.ThrowExceptionOnFailure,
                    client.Name) : throw new ArgumentNullException(nameof(client));

        /// <summary>
        /// Clones the client With a change
        /// </summary>
        public static Client With(this Client client, IGetHttpRequestMessage getHttpRequestMessage)
            =>
                client != null ? new Client(
                    client.SerializationAdapter,
                    client.BaseUri,
                    client.DefaultRequestHeaders,
                    client.logger is ILogger<Client> logger ? logger : null,
                    client.createHttpClient,
                    client.sendHttpRequestMessage,
                    getHttpRequestMessage,
                    client.ThrowExceptionOnFailure,
                    client.Name) : throw new ArgumentNullException(nameof(client));

        /// <summary>
        /// Clones the client With a change
        /// </summary>
        public static Client With(this Client client, ISendHttpRequestMessage sendHttpRequestMessage)
            =>
                client != null ? new Client(
                    client.SerializationAdapter,
                    client.BaseUri,
                    client.DefaultRequestHeaders,
                    client.logger is ILogger<Client> logger ? logger : null,
                    client.createHttpClient,
                    sendHttpRequestMessage,
                    client.getHttpRequestMessage,
                    client.ThrowExceptionOnFailure,
                    client.Name) : throw new ArgumentNullException(nameof(client));


        /// <summary>
        /// Clones the client With a change
        /// </summary>
        public static Client With(this Client client, bool throwExceptionOnFailure)
            =>
                client != null ? new Client(
                    client.SerializationAdapter,
                    client.BaseUri,
                    client.DefaultRequestHeaders,
                    client.logger is ILogger<Client> logger ? logger : null,
                    client.createHttpClient,
                    client.sendHttpRequestMessage,
                    client.getHttpRequestMessage,
                    throwExceptionOnFailure,
                    client.Name) : throw new ArgumentNullException(nameof(client));

        #endregion Public Methods
    }
}