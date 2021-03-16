using System;
using System.Collections.Generic;
using System.Collections.Immutable;
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

        public static IHeadersCollection ToHeadersCollection(this HttpResponseHeaders? httpResponseHeaders)
            => new HeadersCollection(httpResponseHeaders == null ? ImmutableDictionary.Create<string, IEnumerable<string>>() : httpResponseHeaders.ToImmutableDictionary());

        public static IHeadersCollection ToHeadersCollection(this HttpContentHeaders? httpContentHeaders)
            => new HeadersCollection(httpContentHeaders == null ? ImmutableDictionary.Create<string, IEnumerable<string>>() : httpContentHeaders.ToImmutableDictionary());

        public static IHeadersCollection Append(this IHeadersCollection headersCollection, string key, IEnumerable<string> value)
        {
            if (headersCollection == null) throw new ArgumentNullException(nameof(headersCollection));
            if (key == null) throw new ArgumentNullException(nameof(key));
            if (value == null) throw new ArgumentNullException(nameof(value));

            var dictionary = new Dictionary<string, IEnumerable<string>>();

            foreach (var kvp in headersCollection)
            {
                dictionary.Add(kvp.Key, kvp.Value);
            }

            dictionary.Add(key, value);

            return new HeadersCollection(dictionary);
        }

        public static IHeadersCollection Append(this IHeadersCollection headersCollection, KeyValuePair<string, IEnumerable<string>> kvp)
        => Append(headersCollection, kvp.Key, kvp.Value);

        public static IHeadersCollection Append(this IHeadersCollection headersCollection, string key, string value)
        => Append(headersCollection, key, new List<string> { value });

        public static IHeadersCollection AppendDefaultRequestHeaders(this IClient client, IHeadersCollection headersCollection2)
        {
            if (client == null) throw new ArgumentNullException(nameof(client));
            if (headersCollection2 == null) throw new ArgumentNullException(nameof(headersCollection2));

            var dictionary = new Dictionary<string, IEnumerable<string>>();

            if (client.DefaultRequestHeaders != null)
            {
                foreach (var kvp in client.DefaultRequestHeaders)
                {
                    dictionary.Add(kvp.Key, kvp.Value);
                }
            }

            foreach (var kvp in headersCollection2)
            {
                if (dictionary.ContainsKey(kvp.Key)) _ = dictionary.Remove(kvp.Key);

                dictionary.Add(kvp.Key, kvp.Value);
            }

            return new HeadersCollection(dictionary);
        }

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

            if (headersCollection2 != null)
            {
                foreach (var kvp in headersCollection2)
                {
                    //TODO: This looks a bit dodgy. Need tests around this to make sure crazy stuff don't happen
                    if (dictionary.ContainsKey(kvp.Key)) _ = dictionary.Remove(kvp.Key);

                    dictionary.Add(kvp.Key, kvp.Value);
                }
            }

            return new HeadersCollection(dictionary);
        }

        public static IHeadersCollection CreateHeadersCollection(this string key, string value)
        => new HeadersCollection(ImmutableDictionary.CreateRange(new List<KeyValuePair<string, IEnumerable<string>>> { new(key, new List<string> { value }) }));

        public static IHeadersCollection CreateHeadersCollection(this KeyValuePair<string, IEnumerable<string>> kvp)
            => new HeadersCollection(ImmutableDictionary.CreateRange(new List<KeyValuePair<string, IEnumerable<string>>> { kvp }));

        public static IHeadersCollection CreateOrSetHeaderValue(
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

        //public static TResponseBody DeserializeResponseBody<TResponseBody>(this IClient restClient, byte[] response, IHeadersCollection headersCollection) => restClient == null
        //                        ? throw new ArgumentNullException(nameof(restClient))
        //                : response == null
        //                ? throw new ArgumentNullException(nameof(response))
        //                : restClient.SerializationAdapter.Deserialize<TResponseBody>(response, headersCollection);

        /// <summary>
        /// Sets the Authorization header for Basic Authentication with the specified credentials
        /// </summary>
        public static IHeadersCollection SetBasicAuthenticationHeader(this IHeadersCollection? requestHeaders, string userName, string password)
            => CreateOrSetHeaderValue(requestHeaders, Authorization, "Basic " + Convert.ToBase64String(Encoding.UTF8.GetBytes(userName + ":" + password)));

        /// <summary>
        /// Sets the Authorization header for Basic Authentication with the specified credentials
        /// </summary>
        public static IHeadersCollection SetBasicAuthenticationHeader(string userName, string password) => SetBasicAuthenticationHeader(null, userName, password);

        public static IHeadersCollection SetBearerTokenAuthenticationHeader(string bearerToken)
        => SetBearerTokenAuthenticationHeader(null, bearerToken);

        public static IHeadersCollection SetBearerTokenAuthenticationHeader(this IHeadersCollection? requestHeaders, string bearerToken)
            => CreateOrSetHeaderValue(requestHeaders, Authorization, "Bearer " + bearerToken);

        public static IHeadersCollection SetJsonContentTypeHeader()
        => SetJsonContentTypeHeader(null);

        public static IHeadersCollection SetJsonContentTypeHeader(this IHeadersCollection? requestHeaders)
        => CreateOrSetHeaderValue(requestHeaders, ContentTypeHeaderName, JsonMediaType);

        #endregion Public Methods

    }
}
