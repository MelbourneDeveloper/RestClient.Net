using System;
using System.Collections.Generic;

namespace RestClient.Net.Abstractions
{
    public static class HeadersExtensions
    {
        public static IHeadersCollection Append(this IHeadersCollection headersCollection, IHeadersCollection headersCollection2)
        {
            if (headersCollection == null) throw new ArgumentNullException(nameof(headersCollection));
            if (headersCollection2 == null) throw new ArgumentNullException(nameof(headersCollection2));

            var dictionary = new Dictionary<string, IEnumerable<string>>();

            foreach (var kvp in headersCollection)
            {
                dictionary.Add(kvp.Key, kvp.Value);
            }

            foreach (var kvp in headersCollection2)
            {
                dictionary.Add(kvp.Key, kvp.Value);
            }

            return new RequestHeadersCollection(dictionary);
        }

        public static IHeadersCollection Append(this IHeadersCollection headersCollection, string key, string value)
        {
            if (headersCollection == null) throw new ArgumentNullException(nameof(headersCollection));
            if (key == null) throw new ArgumentNullException(nameof(key));
            if (value == null) throw new ArgumentNullException(nameof(value));

            var dictionary = new Dictionary<string, IEnumerable<string>>();

            foreach (var kvp in headersCollection)
            {
                dictionary.Add(kvp.Key, kvp.Value);
            }

            dictionary.Add(key, new List<string> { key });

            return new RequestHeadersCollection(dictionary);
        }
    }
}
