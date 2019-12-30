using System.Collections.Generic;

namespace RestClientDotNet.Abstractions
{
    /// <summary>
    /// Abstraction for storing and enumerating Http Request headers
    /// </summary>
    public interface IRestHeadersCollection : IEnumerable<KeyValuePair<string, IEnumerable<string>>>
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
