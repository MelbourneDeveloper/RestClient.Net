using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace CF.RESTClientDotNet
{
    public class RESTClient
    {
        #region Fields
        private readonly HttpClient _HttpClient = new HttpClient();
        #endregion

        #region Public Properties
        public Uri BaseUri => _HttpClient.BaseAddress;
        public Dictionary<string, string> Headers { get; private set; } = new Dictionary<string, string>();
        public AuthenticationHeaderValue Authorization { get; set; }
        public Dictionary<HttpStatusCode, Func<string, object>> HttpStatusCodeFuncs = new Dictionary<HttpStatusCode, Func<string, object>>();
        public IZip Zip;
        public ISerializationAdapter SerializationAdapter { get; set; }
        public Encoding Encoding = Encoding.UTF8;
        #endregion

        #region Constructor
        public RESTClient(Uri baseUri)
        {
            _HttpClient.BaseAddress = baseUri;
            _HttpClient.Timeout = new TimeSpan(0, 3, 0);
        }

        public RESTClient(Uri baseUri, Encoding encoding) : this(baseUri)
        {
            Encoding = encoding;
        }

        #endregion

        #region Private Methods
        private async Task<TReturn> Call<TReturn, TBody>(string queryString, bool isPost, string contentType, TBody body = default(TBody))
        {
            _HttpClient.DefaultRequestHeaders.Clear();

            if (Authorization != null)
            {
                _HttpClient.DefaultRequestHeaders.Authorization = Authorization;
            }

            HttpResponseMessage result;
            if (!isPost)
            {
                _HttpClient.DefaultRequestHeaders.Clear();
                foreach (var key in Headers.Keys)
                {
                    _HttpClient.DefaultRequestHeaders.Add(key, Headers[key]);
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
                    stringContent = new StringContent(bodyAsString, Encoding, contentType);
                    length = bodyAsString.Length;
                }
                else
                {
                    var data = await SerializationAdapter.SerializeAsync<TBody>(body);
                    var bodyString = Encoding.GetString(data);
                    length = bodyString.Length;
                    stringContent = new StringContent(bodyString, Encoding, contentType);
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

            var text = await result.Content.ReadAsStringAsync();

            if (HttpStatusCodeFuncs.ContainsKey(result.StatusCode))
            {
                return (TReturn)HttpStatusCodeFuncs[result.StatusCode].Invoke(text);
            }

            throw new HttpStatusException($"Nope {result.StatusCode}.\r\nBase Uri: {_HttpClient.BaseAddress}. Full Uri: {_HttpClient.BaseAddress + queryString}\r\nError:\r\n{text}", result.StatusCode);
        }
        #endregion

        #region Public Methods
        public async Task<TReturn> GetAsync<TReturn>(string queryString, string contentType = "application/json")
        {
            return await Call<TReturn, object>(queryString, false, contentType);
        }

        public async Task<TReturn> PostAsync<TReturn, TBody>(TBody body, string queryString, string contentType = "application/json")
        {
            return await Call<TReturn, TBody>(queryString, true, contentType, body);
        }
        #endregion
    }
}
