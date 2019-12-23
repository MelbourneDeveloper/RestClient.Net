using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RestClientDotNet.Abstractions
{
    public interface IRestClient
    {
        //Task<RestResponse<TReturn>> SendAsync<TReturn, TBody>(Uri resource, HttpVerb httpVerb, string contentType, TBody body, CancellationToken cancellationToken);
        Task<RestResponse<TReturn>> SendAsync<TReturn, TBody>(RestRequest<TBody> restRequest);
        IRestHeadersCollection DefaultRequestHeaders { get; }
        string DefaultContentType { get; }
        TimeSpan Timeout { get; set; }
    }

    public class RestRequest<TBody>
    {

        public RestRequest()
        {

        }

        public RestRequest(IRestClient client) : this()
        {
            if (client == null) throw new ArgumentNullException(nameof(client));

            var contentType = client.DefaultContentType;
            if (!string.IsNullOrEmpty(contentType))
            {
                ContentType = contentType;
            }
            var headers = client.DefaultRequestHeaders;
            if (headers != null)
            {
                foreach (var header in headers)
                {
                    Headers.Add(header.Key, header.Value);
                }
            }
        }

        public IRestHeadersCollection Headers { get; } = new RestRequestHeadersCollection();

        public Uri Resource { get; set; }
        public HttpVerb HttpVerb { get; set; } = HttpVerb.Get;
        public string ContentType { get; set; } = "application/json";
        public TBody Body { get; set; }


    }

    public class RestRequestHeadersCollection : IRestHeadersCollection
    {
#pragma warning disable CA1065 // Do not raise exceptions in unexpected locations
        public IEnumerable<string> this[string name] => throw new NotImplementedException();
#pragma warning restore CA1065 // Do not raise exceptions in unexpected locations

        public void Add(string name, string value)
        {
            throw new NotImplementedException();
        }

        public void Add(string name, IEnumerable<string> values)
        {
            throw new NotImplementedException();
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }

        public bool Contains(string name)
        {
            throw new NotImplementedException();
        }

        public IEnumerator<KeyValuePair<string, IEnumerable<string>>> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
}