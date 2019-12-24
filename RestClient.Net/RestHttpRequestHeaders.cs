using RestClientDotNet.Abstractions;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;

namespace RestClientDotNet
{
    /// <summary>
    /// A wrapper for HttpRequestHeaders
    /// </summary>
    public class RestHttpRequestHeaders : IRestHeadersCollection
    {
        #region Public Properties
        public HttpRequestHeaders HttpRequestHeaders { get; }
        public IEnumerable<string> Names => HttpRequestHeaders.ToList().Select(kvp => kvp.Key);
        public IEnumerable<string> this[string name] => HttpRequestHeaders.GetValues(name);
        #endregion

        #region Implementation
        public RestHttpRequestHeaders(HttpRequestHeaders httpRequestHeaders)
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
