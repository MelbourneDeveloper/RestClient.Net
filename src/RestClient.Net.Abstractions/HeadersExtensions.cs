using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;

namespace RestClient.Net.Abstractions.Extensions
{
    public static class HeadersExtensions
    {

        #region Internal Fields

        internal const string Authorization = "Authorization";

        internal const string ContentTypeHeaderName = "Content-Type";

        internal const string JsonMediaType = "application/json";

        #endregion Internal Fields

        #region Public Methods

        public static IHeadersCollection Append(this IHeadersCollection headersCollection, string key, IEnumerable<string> value)
        {
            if (headersCollection == null) throw new ArgumentNullException(nameof(headersCollection));
            if (key == null) throw new ArgumentNullException(nameof(key));
            if (value == null) throw new ArgumentNullException(nameof(value));

            var dictionary = headersCollection.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

            dictionary.Add(key, value);

            return new HeadersCollection(dictionary);
        }

        public static IHeadersCollection Append(this IHeadersCollection headersCollection, string key, string value)
        => Append(headersCollection, key, new List<string> { value });

        public static IHeadersCollection Append(this IHeadersCollection? headersCollection, IHeadersCollection? headersCollection2)
        {

            var dictionary = new Dictionary<string, IEnumerable<string>>();

            if (headersCollection != null)
            {
                foreach (var kvp in headersCollection)
                {
                    dictionary.Add(kvp.Key, kvp.Value);
                }
            }

            if (headersCollection2 == null) return new HeadersCollection(dictionary);

            foreach (var kvp in headersCollection2)
            {
                if (dictionary.ContainsKey(kvp.Key)) _ = dictionary.Remove(kvp.Key);

                dictionary.Add(kvp.Key, kvp.Value);
            }

            return new HeadersCollection(dictionary);
        }

        public static IHeadersCollection AppendDefaultRequestHeaders(this IClient client, IHeadersCollection headersCollection)
        {
            if (client == null) throw new ArgumentNullException(nameof(client));
            if (headersCollection == null) throw new ArgumentNullException(nameof(headersCollection));

            var dictionary = new Dictionary<string, IEnumerable<string>>();

            if (client.DefaultRequestHeaders != null)
            {
                foreach (var kvp in client.DefaultRequestHeaders)
                {
                    dictionary.Add(kvp.Key, kvp.Value);
                }
            }

            foreach (var kvp in headersCollection)
            {
                if (dictionary.ContainsKey(kvp.Key)) _ = dictionary.Remove(kvp.Key);

                dictionary.Add(kvp.Key, kvp.Value);
            }

            return new HeadersCollection(dictionary);
        }

        public static IHeadersCollection ToHeadersCollection(this string key, string value)
        => new HeadersCollection(ImmutableDictionary.CreateRange(new List<KeyValuePair<string, IEnumerable<string>>> { new(key, new List<string> { value }) }));

        public static IHeadersCollection ToHeadersCollection(this KeyValuePair<string, IEnumerable<string>> kvp)
            => new HeadersCollection(ImmutableDictionary.CreateRange(new List<KeyValuePair<string, IEnumerable<string>>> { kvp }));

        /// <summary>
        /// Sets the Authorization header for Basic Authentication with the specified credentials
        /// </summary>
        public static IHeadersCollection CreateHeadersCollectionWithBasicAuthentication(string userName, string password) => WithBasicAuthentication(null, userName, password);

        public static IHeadersCollection CreateHeadersCollectionWithBearerTokenAuthentication(string bearerToken)
        => WithBearerTokenAuthentication(null, bearerToken);

        public static IHeadersCollection CreateHeadersCollectionWithJsonContentType()
        => WithJsonContentTypeHeader(null);

        public static IHeadersCollection WithHeaderValue(
            this IHeadersCollection? requestHeaders,
            string key,
            string value)
            =>
            _ = requestHeaders == null ?
            new HeadersCollection(
                ImmutableDictionary.CreateRange(
                    new List<KeyValuePair<string, IEnumerable<string>>>
                    {
                        new(key, ImmutableList.Create(value))
                    }
                    )) :
            requestHeaders.Append(key, value);

        public static IHeadersCollection ToHeadersCollection(this HttpResponseHeaders? httpResponseHeaders)
            => new HeadersCollection(httpResponseHeaders == null ? ImmutableDictionary.Create<string, IEnumerable<string>>() : httpResponseHeaders.ToImmutableDictionary());

        public static IHeadersCollection ToHeadersCollection(this HttpContentHeaders? httpContentHeaders)
            => new HeadersCollection(httpContentHeaders == null ? ImmutableDictionary.Create<string, IEnumerable<string>>() : httpContentHeaders.ToImmutableDictionary());
        /// <summary>
        /// Sets the Authorization header for Basic Authentication with the specified credentials
        /// </summary>
        public static IHeadersCollection WithBasicAuthentication(this IHeadersCollection? requestHeaders, string userName, string password)
            => WithHeaderValue(requestHeaders, Authorization, "Basic " + Convert.ToBase64String(Encoding.UTF8.GetBytes(userName + ":" + password)));

        public static IHeadersCollection WithBearerTokenAuthentication(this IHeadersCollection? requestHeaders, string bearerToken)
            => WithHeaderValue(requestHeaders, Authorization, "Bearer " + bearerToken);

        public static IHeadersCollection WithJsonContentTypeHeader(this IHeadersCollection? requestHeaders)
        => WithHeaderValue(requestHeaders, ContentTypeHeaderName, JsonMediaType);

        public static IHeadersCollection JsonContentTypeHeaders { get; } = new HeadersCollection(ContentTypeHeaderName, JsonMediaType);

        #endregion Public Methods

    }
}
