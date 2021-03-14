using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;

namespace RestClient.Net.Abstractions.Extensions
{
    public static class MiscExtensions
    {

        #region Internal Fields

        internal const string ContentTypeHeaderName = "Content-Type";

        internal const string JsonMediaType = "application/json";

        #endregion Internal Fields

        #region Private Fields


        #endregion Private Fields

        #region Public Methods

        public static TResponseBody DeserializeResponseBody<TResponseBody>(this IClient restClient, byte[] response, IHeadersCollection headersCollection) => restClient == null
                        ? throw new ArgumentNullException(nameof(restClient))
                        : response == null
                        ? throw new ArgumentNullException(nameof(response))
                        : restClient.SerializationAdapter.Deserialize<TResponseBody>(response, headersCollection);

        /// <summary>
        /// Sets the Authorization header for Basic Authentication with the specified credentials
        /// </summary>
        public static IHeadersCollection SetBasicAuthenticationHeader(this IHeadersCollection requestHeaders, string userName, string password)
        {
            if (requestHeaders == null) throw new ArgumentNullException(nameof(requestHeaders));
            var credentials = Convert.ToBase64String(Encoding.UTF8.GetBytes(userName + ":" + password));
            return requestHeaders.Append("Authorization", "Basic " + credentials);
        }

        public static IHeadersCollection SetBearerTokenAuthenticationHeader(string bearerToken) => SetBearerTokenAuthenticationHeader(new RequestHeadersCollection(new Dictionary<string, IEnumerable<string>>()), bearerToken);

        public static IHeadersCollection SetBearerTokenAuthenticationHeader(this IHeadersCollection requestHeaders, string bearerToken)
        {
            return requestHeaders == null
                ? throw new ArgumentNullException(nameof(requestHeaders))
                : requestHeaders.Append("Authorization", "Bearer " + bearerToken);
        }

        public static IHeadersCollection SetJsonContentTypeHeader(this IHeadersCollection? requestHeaders)
        => CreateOrSetHeaderValue(requestHeaders, ContentTypeHeaderName, ContentTypeHeaderName);

        public static IHeadersCollection CreateOrSetHeaderValue(
            this IHeadersCollection? requestHeaders,
            string key,
            string value)
            =>
            _ = requestHeaders == null ?
            new RequestHeadersCollection(
                ImmutableDictionary.CreateRange(
                    new List<KeyValuePair<string, IEnumerable<string>>>
                    {
                        new KeyValuePair<string, IEnumerable<string>>
                        (key, ImmutableList.Create(value))
                    }
                    )) :
            requestHeaders.Append(key, value);

        #endregion Public Methods

    }
}