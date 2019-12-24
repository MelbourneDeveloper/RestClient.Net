using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace RestClientDotNet.Abstractions
{
    public sealed class RestRequestHeadersCollection : IRestHeadersCollection
    {
        #region Fields
        private readonly Dictionary<string, List<string>> _dictionary = new Dictionary<string, List<string>>();
        #endregion

        IEnumerable<string> IRestHeadersCollection.this[string name] => _dictionary[name];

        public void Add(string name, string value)
        {
            _dictionary.Add(name, new List<string> { value });
        }

        public void Add(string name, IEnumerable<string> values)
        {
            _dictionary.Add(name, values.ToList());
        }

        public void Clear()
        {
            _dictionary.Clear();
        }

        public bool Contains(string name)
        {
            return _dictionary.ContainsKey(name);
        }

        public IEnumerator<KeyValuePair<string, IEnumerable<string>>> GetEnumerator()
        {
            var asdasd = _dictionary.Values;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new System.NotImplementedException();
        }
    }
}
