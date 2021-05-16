using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace RestClient.Net.Abstractions
{
    public sealed class HeadersCollection : IHeadersCollection
    {
        #region Fields
        private readonly IDictionary<string, IEnumerable<string>> dictionary;
        #endregion

        #region Public Constructors

        public HeadersCollection(IDictionary<string, IEnumerable<string>> dictionary) => this.dictionary = dictionary;

        public HeadersCollection(string key, string value) : this(ImmutableDictionary.CreateRange(
                    new List<KeyValuePair<string, IEnumerable<string>>>
                    {
                        new(key, ImmutableList.Create(value))
                    }
                    ))
        {
        }

        #endregion Public Constructors

        #region Public Properties
        public static HeadersCollection Empty { get; } = new HeadersCollection(ImmutableDictionary.Create<string, IEnumerable<string>>());
        public IEnumerable<string> Names => dictionary.Keys;
        IEnumerable<string> IHeadersCollection.this[string name] => dictionary[name];
        #endregion Public Properties

        #region Public Methods
        public bool Contains(string name) => dictionary.ContainsKey(name);

        public IEnumerator<KeyValuePair<string, IEnumerable<string>>> GetEnumerator() => dictionary.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => dictionary.GetEnumerator();
        public override string ToString() => string.Join("\r\n", dictionary.Select(kvp => $"{kvp.Key}: {string.Join(", ", kvp.Value)}"));
        #endregion
    }
}
