using RestClient.Net.Abstractions;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;

namespace RestClient.Net
{
    public class HttpResponseHeadersCollection : IHeadersCollection
    {
        #region Public Properties
        public IEnumerable<string> Names => HttpResponseHeaders.ToList().Select(l => l.Key);
        public HttpResponseHeaders HttpResponseHeaders { get; }
        public IEnumerable<string> this[string name] => HttpResponseHeaders.GetValues(name);
        #endregion

        #region Constructor
        public HttpResponseHeadersCollection(HttpResponseHeaders httpResponseHeaders) => HttpResponseHeaders = httpResponseHeaders;
        #endregion

        #region Implementation
        public void Clear() => throw new System.NotImplementedException();

        public bool Contains(string name) => HttpResponseHeaders.Contains(name);

        public IEnumerator<KeyValuePair<string, IEnumerable<string>>> GetEnumerator() => HttpResponseHeaders.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => HttpResponseHeaders.GetEnumerator();

        public void Add(KeyValuePair<string, IEnumerable<string>> keyValuePair) => throw new System.NotImplementedException();
        #endregion
    }
}
