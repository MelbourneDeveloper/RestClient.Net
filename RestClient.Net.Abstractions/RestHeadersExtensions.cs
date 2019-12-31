using System;
using System.Collections.Generic;

namespace RestClientDotNet.Abstractions
{
    public static class RestHeadersExtensions
    {
        public static void Add(this IHeadersCollection restHeadersCollection, string name, IEnumerable<string> value)
        {
            if (restHeadersCollection == null) throw new ArgumentNullException(nameof(restHeadersCollection));
            restHeadersCollection.Add(new KeyValuePair<string, IEnumerable<string>>(name, value));
        }
        public static void Add(this IHeadersCollection restHeadersCollection, string name, string value)
        {
            if (restHeadersCollection == null) throw new ArgumentNullException(nameof(restHeadersCollection));
            if (restHeadersCollection.Contains(name))
            {
                throw new InvalidOperationException($"The name of {name} already exists in the header collection");
            }

            restHeadersCollection.Add(new KeyValuePair<string, IEnumerable<string>>(name, new List<string> { value }));
        }
    }
}
