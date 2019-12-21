using System.Collections;
using System.Collections.Generic;
using System.Net.Http.Headers;

namespace RestClientDotNet
{
    /// <summary>
    /// A wrapper for HttpRequestHeaders
    /// </summary>
    public class RestRequestHeadersCollection : IRestHeadersCollection
    {
        #region Public Properties
        public HttpRequestHeaders HttpRequestHeaders { get; }
        public IEnumerable<string> this[string name] => HttpRequestHeaders.GetValues(name);
        #endregion

        #region Implementation
        public RestRequestHeadersCollection(HttpRequestHeaders httpRequestHeaders)
        {
            HttpRequestHeaders = httpRequestHeaders;
        }

        public void Add(string name, string value)
        {
            HttpRequestHeaders.Add(name, value);
        }

        public void Add(string name, IEnumerable<string> values)
        {
            HttpRequestHeaders.Add(name, values);
        }

        public IEnumerator<KeyValuePair<string, IEnumerable<string>>> GetEnumerator()
        {
            return HttpRequestHeaders.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return HttpRequestHeaders.GetEnumerator();
        }

        public void Clear()
        {
            HttpRequestHeaders.Clear();
        }

        public bool Contains(string name)
        {
            return HttpRequestHeaders.Contains(name);
        }
        #endregion
    }
}
