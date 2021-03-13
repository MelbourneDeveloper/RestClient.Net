using System;
using System.Collections;
using System.Collections.Generic;

#pragma warning disable CA1063 // Implement IDisposable Correctly
#pragma warning disable CA1816 // Dispose methods should call SuppressFinalize

namespace RestClient.Net.Abstractions
{
    public class NullHeadersCollection : IHeadersCollection, IDisposable
    {
        private readonly NullKvpEnumerator<string, IEnumerable<string>> nullEnumerator = new();

        public static NullHeadersCollection Instance { get; } = new();

        #region Public Properties
        public IEnumerable<string> Names { get; } = new List<string>();
        public IEnumerable<string> this[string name] => new List<string>();
        #endregion


        #region Implementation
        public void Clear() { }

        public bool Contains(string name) => false;

        public IEnumerator<KeyValuePair<string, IEnumerable<string>>> GetEnumerator() => nullEnumerator;

        IEnumerator IEnumerable.GetEnumerator() => nullEnumerator;

        public void Add(KeyValuePair<string, IEnumerable<string>> keyValuePair) { }

        public void Dispose() => nullEnumerator.Dispose();
        #endregion
    }
}
