using System.Collections.Generic;

namespace RestClientDotNet
{
    /// <summary>
    /// Abstraction for storing and enumerating Http Request headers
    /// </summary>
    public interface IRestHeadersCollection : IEnumerable<KeyValuePair<string, IEnumerable<string>>>
    {
        void Add(string name, string value);
        void Add(string name, IEnumerable<string> values);
        void Clear();

        IEnumerable<string> this[string name]
        {
            get;
        }

        bool ContainsName(string name);
    }
}
