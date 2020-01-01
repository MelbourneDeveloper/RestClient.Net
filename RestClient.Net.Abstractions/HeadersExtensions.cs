using System;
using System.Collections.Generic;

namespace RestClient.Net.Abstractions
{
    public static class HeadersExtensions
    {
        public static void Add(this IHeadersCollection headersCollection, string name, IEnumerable<string> value)
        {
            if (headersCollection == null) throw new ArgumentNullException(nameof(headersCollection));
            headersCollection.Add(new KeyValuePair<string, IEnumerable<string>>(name, value));
        }
        public static void Add(this IHeadersCollection headersCollection, string name, string value)
        {
            if (headersCollection == null) throw new ArgumentNullException(nameof(headersCollection));
            if (headersCollection.Contains(name))
            {
                throw new InvalidOperationException($"The name of {name} already exists in the header collection");
            }

            headersCollection.Add(new KeyValuePair<string, IEnumerable<string>>(name, new List<string> { value }));
        }
    }
}
