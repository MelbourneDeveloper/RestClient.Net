using System.Collections;
using System.Collections.Generic;

namespace RestClient.Net.Abstractions
{
    public sealed class RequestHeadersCollection : IHeadersCollection
    {
        #region Fields
        private readonly Dictionary<string, IEnumerable<string>> _dictionary = new Dictionary<string, IEnumerable<string>>();
        #endregion

        #region Public Properties
        IEnumerable<string> IHeadersCollection.this[string name] => _dictionary[name];
        public IEnumerable<string> Names => _dictionary.Keys;
        #endregion

        #region Public Methods
        public void Add(KeyValuePair<string, IEnumerable<string>> keyValuePair) => _dictionary.Add(keyValuePair.Key, keyValuePair.Value);

        public void Clear() => _dictionary.Clear();

        public bool Contains(string name) => _dictionary.ContainsKey(name);

        public IEnumerator<KeyValuePair<string, IEnumerable<string>>> GetEnumerator() => _dictionary.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => _dictionary.GetEnumerator();
        #endregion
    }
}
