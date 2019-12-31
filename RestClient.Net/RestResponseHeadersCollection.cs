using RestClientDotNet.Abstractions;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;

namespace RestClientDotNet
{
    public class RestResponseHeadersCollection : IHeadersCollection
    {
        #region Public Properties
        public IEnumerable<string> Names => HttpResponseHeaders.ToList().Select(l => l.Key);
        public HttpResponseHeaders HttpResponseHeaders { get; }
        public IEnumerable<string> this[string name] => HttpResponseHeaders.GetValues(name);
        #endregion

        #region Constructor
        public RestResponseHeadersCollection(HttpResponseHeaders httpResponseHeaders)
        {
            HttpResponseHeaders = httpResponseHeaders;
        }
        #endregion

        #region Implementation
        public void Clear()
        {
            throw new System.NotImplementedException();
        }

        public bool Contains(string name)
        {
            return HttpResponseHeaders.Contains(name);
        }

        public IEnumerator<KeyValuePair<string, IEnumerable<string>>> GetEnumerator()
        {
            return HttpResponseHeaders.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return HttpResponseHeaders.GetEnumerator();
        }

        public void Add(KeyValuePair<string, IEnumerable<string>> keyValuePair)
        {
            throw new System.NotImplementedException();
        }
        #endregion
    }
}
