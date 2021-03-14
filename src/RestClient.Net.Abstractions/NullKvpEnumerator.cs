using System.Collections;
using System.Collections.Generic;

#pragma warning disable CA1063 // Implement IDisposable Correctly
#pragma warning disable CA1816 // Dispose methods should call SuppressFinalize

namespace RestClient.Net.Abstractions
{
    public class NullKvpEnumerator<T, T2> : IEnumerator<KeyValuePair<T, T2>>
    {
        public KeyValuePair<T, T2> Current => new KeyValuePair<T, T2>();

        object IEnumerator.Current => new KeyValuePair<T, T2>();

        public void Dispose() { }
        public bool MoveNext() => false;
        public void Reset() { }
    }
}
