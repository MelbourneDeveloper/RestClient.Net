using RestClientDotNet.Abstractions;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http.Headers;

namespace RestClientDotNet
{
    public class RestResponseHeadersCollection : IRestHeadersCollection
    {
        #region Public Properties
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
        public void Add(string name, string value)
        {
            throw new System.NotImplementedException();
        }

        public void Add(string name, IEnumerable<string> values)
        {
            throw new System.NotImplementedException();
        }

        public void Clear()
        {
            throw new System.NotImplementedException();
        }

        public IEnumerator<KeyValuePair<string, IEnumerable<string>>> GetEnumerator()
        {
            return HttpResponseHeaders.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return HttpResponseHeaders.GetEnumerator();
        }

        public bool Contains(string name)
        {
            return HttpResponseHeaders.Contains(name);
        }
        #endregion
    }
}
