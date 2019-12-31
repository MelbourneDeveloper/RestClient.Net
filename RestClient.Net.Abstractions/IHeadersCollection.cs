using System.Collections.Generic;

namespace RestClient.Net.Abstractions
{
    /// <summary>
    /// Abstraction for storing and enumerating Http Request headers
    /// </summary>
    public interface IHeadersCollection : IEnumerable<KeyValuePair<string, IEnumerable<string>>>
    {
        void Add(KeyValuePair<string, IEnumerable<string>> keyValuePair);
        void Clear();

        IEnumerable<string> this[string name]
        {
            get;
        }

        bool Contains(string name);
        IEnumerable<string> Names { get; }
    }
}
