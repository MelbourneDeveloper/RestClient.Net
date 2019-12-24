using System.Collections.Generic;

namespace RestClientDotNet.Abstractions
{
#pragma warning disable CA1711 // Identifiers should not have incorrect suffix
    /// <summary>
    /// Abstraction for storing and enumerating Http Request headers
    /// </summary>
    public interface IRestHeadersCollection
#pragma warning restore CA1711 // Identifiers should not have incorrect suffix
    {
        void Add(string name, string value);
        void Add(string name, IEnumerable<string> values);
        void Clear();

        IEnumerable<string> this[string name]
        {
            get;
        }

        bool Contains(string name);
        IEnumerable<string> Names { get; }
    }
}
