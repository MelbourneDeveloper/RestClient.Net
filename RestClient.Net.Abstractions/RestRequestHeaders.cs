using System.Collections.Generic;
using System.Linq;

namespace RestClientDotNet.Abstractions
{
    public sealed class RestRequestHeaders : IRestHeadersCollection
    {
        #region Fields
        private readonly Dictionary<string, List<string>> _dictionary = new Dictionary<string, List<string>>();
        #endregion

        #region Public Properties
        IEnumerable<string> IRestHeadersCollection.this[string name] => _dictionary[name];
        public IEnumerable<string> Names => _dictionary.Keys;
        #endregion

        #region Public Methods
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
        #endregion
    }
}
