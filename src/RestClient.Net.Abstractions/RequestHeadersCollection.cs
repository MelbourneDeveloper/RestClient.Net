using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace RestClient.Net.Abstractions
{
    public sealed class RequestHeadersCollection : IHeadersCollection
    {
        #region Fields
        private readonly IDictionary<string, IEnumerable<string>> _dictionary;
        #endregion

        public RequestHeadersCollection(IDictionary<string, IEnumerable<string>> dictionary) => _dictionary = dictionary;

        #region Public Properties
        IEnumerable<string> IHeadersCollection.this[string name] => dictionary[name];
        public IEnumerable<string> Names => dictionary.Keys;
        #endregion

        #region Public Methods
        public bool Contains(string name) => _dictionary.ContainsKey(name);

        public IEnumerator<KeyValuePair<string, IEnumerable<string>>> GetEnumerator() => dictionary.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => dictionary.GetEnumerator();
        #endregion

        public override string ToString() => string.Join("\r\n", dictionary.Select(kvp => $"{kvp.Key}: {kvp.Value.Select(v => string.Join(", ", v))}\r\n"));

    }
}
