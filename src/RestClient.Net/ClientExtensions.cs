

using Microsoft.Extensions.Logging;
using System;
using Urls;

#if NET45
using System.Collections.Generic;
#endif

namespace RestClient.Net
{
    public static class ClientExtensions
    {
        #region Public Methods

        /// <summary>
        /// Clones the client With a change
        /// </summary>
        public static Client With(this Client client, AbsoluteUrl baseUri)
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
            client.BaseUrl,
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
            => With(client, key.ToHeadersCollection(value));

        /// <summary>
        /// Clones the client With a change
        /// </summary>
        public static Client With(this Client client, ILogger<Client> logger)
        =>
            client != null ? new Client(
            client.SerializationAdapter,
            client.BaseUrl,
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
            client.BaseUrl,
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
                    client.BaseUrl,
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
                    client.BaseUrl,
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
                    client.BaseUrl,
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
                    client.BaseUrl,
                    client.DefaultRequestHeaders,
                    client.logger is ILogger<Client> logger ? logger : null,
                    client.createHttpClient,
                    client.sendHttpRequestMessage,
                    client.getHttpRequestMessage,
                    throwExceptionOnFailure,
                    client.Name) : throw new ArgumentNullException(nameof(client));

#if NET45
        public static bool Contains<T>(this IList<T> list, T compareItem, IEqualityComparer<T>? comparer = null)
        {
            if (list == null) throw new ArgumentNullException(nameof(list));

            comparer ??= EqualityComparer<T>.Default;

            foreach (var item in list)
            {
                if (comparer.Equals(item, compareItem))
                {
                    return true;
                }
            }

            return false;
        }
#endif

        #endregion Public Methods
    }
}