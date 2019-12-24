using System.Collections.Generic;
using System.Linq;

namespace RestClientDotNet.Abstractions
{
#pragma warning disable CA1711 // Identifiers should not have incorrect suffix
    public sealed class RestRequestHeadersCollection : IRestHeadersCollection
#pragma warning restore CA1711 // Identifiers should not have incorrect suffix
    {
        #region Fields
        private readonly Dictionary<string, List<string>> _dictionary = new Dictionary<string, List<string>>();
        #endregion

        IEnumerable<string> IRestHeadersCollection.this[string name] => _dictionary[name];

        public IEnumerable<string> Names => _dictionary.Keys;

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
    }
}
