using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace RestClient.Net.Abstractions
{
    public sealed class RequestHeadersCollection : IHeadersCollection
    {
        #region Fields
        private readonly Dictionary<string, IEnumerable<string>> dictionary = new();
        #endregion

        #region Public Properties
        IEnumerable<string> IHeadersCollection.this[string name] => dictionary[name];
        public IEnumerable<string> Names => dictionary.Keys;
        #endregion

        #region Public Methods
        public void Add(KeyValuePair<string, IEnumerable<string>> keyValuePair) => dictionary.Add(keyValuePair.Key, keyValuePair.Value);

        public void Clear() => dictionary.Clear();

        public bool Contains(string name) => dictionary.ContainsKey(name);

        public IEnumerator<KeyValuePair<string, IEnumerable<string>>> GetEnumerator() => dictionary.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => dictionary.GetEnumerator();
        #endregion

        public override string ToString() => string.Join("\r\n", dictionary.Select(kvp => $"{kvp.Key}: {kvp.Value.Select(v => string.Join(", ", v))}\r\n"));

    }
}
