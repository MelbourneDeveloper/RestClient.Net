using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace CF.RESTClientDotNet
{
    public class RESTClient
    {
        #region Fields
        private readonly HttpClient _HttpClient = new HttpClient { Timeout = new TimeSpan(0, 3, 0) };
        #endregion

        #region Public Properties
        public Uri BaseUri => _HttpClient.BaseAddress;
        public Dictionary<string, string> Headers { get; } = new Dictionary<string, string>();
        public Dictionary<HttpStatusCode, Func<string, object>> HttpStatusCodeFuncs = new Dictionary<HttpStatusCode, Func<string, object>>();
        public IZip Zip;
        public ISerializationAdapter SerializationAdapter { get; }
        public TimeSpan Timeout
        {
            get => _HttpClient.Timeout;
            set => _HttpClient.Timeout = value;
        }
        public ITracer Tracer { get; set; }
        public Type ErrorType { get; set; }
        #endregion

        #region Constructor
        public RESTClient(ISerializationAdapter serializationAdapter, Uri baseUri)
        {
            _HttpClient.BaseAddress = baseUri;
            SerializationAdapter = serializationAdapter;
        }
        #endregion

        #region Private Methods
        private async Task<TReturn> Call<TReturn, TBody>(string queryString, HttpVerb httpVerb, string contentType, TBody body = default(TBody))
        {
            _HttpClient.DefaultRequestHeaders.Clear();

            HttpResponseMessage result = null;
            if (httpVerb != HttpVerb.Post)
            {
                _HttpClient.DefaultRequestHeaders.Clear();
                foreach (var key in Headers.Keys)
                {
                    _HttpClient.DefaultRequestHeaders.Add(key, Headers[key]);
                }

                switch (httpVerb)
                {
                    case HttpVerb.Get:
                        result = await _HttpClient.GetAsync(queryString);
                        break;
                    case HttpVerb.Put:

                        var data = await SerializationAdapter.SerializeAsync(body);
                        var bodyString = SerializationAdapter.Encoding.GetString(data);
                        var length = bodyString.Length;
                        var stringContent = new StringContent(bodyString, SerializationAdapter.Encoding, contentType);

                        result = await _HttpClient.PutAsync(queryString, stringContent);
                        break;
                }

                result = await _HttpClient.GetAsync(queryString);
            }
            else
            {
                StringContent stringContent = null;
                long length = 0;

                var bodyAsString = body as string;
                if (bodyAsString != null)
                {
                    stringContent = new StringContent(bodyAsString, SerializationAdapter.Encoding, contentType);
                    length = bodyAsString.Length;
                }
                else
                {
                    var data = await SerializationAdapter.SerializeAsync(body);
                    var bodyString = SerializationAdapter.Encoding.GetString(data);
                    length = bodyString.Length;
                    stringContent = new StringContent(bodyString, SerializationAdapter.Encoding, contentType);
                }

                //Don't know why but this has to be set again, otherwise more text is added on to the Content-Type header...
                stringContent.Headers.ContentType = new MediaTypeHeaderValue(contentType);

                stringContent.Headers.ContentLength = length;

                foreach (var key in Headers.Keys)
                {
                    stringContent.Headers.Add(key, Headers[key]);
                }

                result = await _HttpClient.PostAsync(queryString, stringContent);

                System.Diagnostics.Debug.WriteLine($"{_HttpClient.BaseAddress} OK");
            }

            if (result.IsSuccessStatusCode)
            {
                var gzipHeader = result.Content.Headers.ContentEncoding.FirstOrDefault(h => !string.IsNullOrEmpty(h) && h.Equals("gzip", StringComparison.InvariantCultureIgnoreCase));
                byte[] data;
                if (gzipHeader != null && Zip != null)
                {
                    var bytes = await result.Content.ReadAsByteArrayAsync();
                    var jsonUnzipped = Zip.Unzip(bytes);

                    data = jsonUnzipped;
                }
                else
                {
                    data = await result.Content.ReadAsByteArrayAsync();
                }

                return await SerializationAdapter.DeserializeAsync<TReturn>(data);
            }

            if (HttpStatusCodeFuncs.ContainsKey(result.StatusCode))
            {
                var text = await result.Content.ReadAsStringAsync();
                return (TReturn)HttpStatusCodeFuncs[result.StatusCode].Invoke(text);
            }
            else
            {
                var responseData = await result.Content.ReadAsByteArrayAsync();
                if (ErrorType != null)
                {
                    var error = await SerializationAdapter.DeserializeAsync(responseData, ErrorType);
                    throw new RESTException(error, responseData, "An error occurred. Please see Error property of this exception", result.StatusCode);
                }
                else
                {
                    throw new Exception($"An error occurred\r\n\r\n{SerializationAdapter.Encoding.GetString(responseData)}");
                }
            }
        }
        #endregion

        #region Public Methods
        public async Task<T> GetAsync<T>()
        {
            return await GetAsync<T>(null);
        }

        public async Task<TReturn> GetAsync<TReturn>(string queryString, string contentType = "application/json")
        {
            return await Call<TReturn, object>(queryString, HttpVerb.Get, contentType);
        }

        public async Task<TReturn> PostAsync<TReturn, TBody>(TBody body, string queryString, string contentType = "application/json")
        {
            return await Call<TReturn, TBody>(queryString, HttpVerb.Post, contentType, body);
        }

        public async Task<TReturn> PutAsync<TReturn, TBody>(TBody body, string queryString, string contentType = "application/json")
        {
            return await Call<TReturn, TBody>(queryString, HttpVerb.Put, contentType, body);
        }
        #endregion
    }
}
