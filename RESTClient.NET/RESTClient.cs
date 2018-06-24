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
        public ISerializationAdapter SerializationAdapter { get; }
        #endregion

        #region Constructor
        public RESTClient(ISerializationAdapter serializationAdapter, Uri baseUri)
        {
            _HttpClient.BaseAddress = baseUri;
            _HttpClient.Timeout = new TimeSpan(0, 3, 0);
            SerializationAdapter = serializationAdapter;
        }
        #endregion

        #region Private Methods
        private async Task<T> Call<T>(string queryString, bool isPost, string contentType, object body = null)
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
                string bodyString;

                if (body is string bodyAsString)
                {
                    bodyString = bodyAsString;
                }
                else
                {
                    if (body != null)
                    {
                        var data = await SerializationAdapter.SerializeAsync(body);
                        bodyString = SerializationAdapter.Encoding.GetString(data);
                    }
                    else
                    {
                        bodyString = string.Empty;
                    }
                }

                var stringContent = new StringContent(bodyString, Encoding.UTF8, contentType);

                //Don't know why but this has to be set again, otherwise more text is added on to the Content-Type header...
                stringContent.Headers.ContentType = new MediaTypeHeaderValue(contentType);

                stringContent.Headers.ContentLength = bodyString.Length;

                foreach (var key in Headers.Keys)
                {
                    stringContent.Headers.Add(key, Headers[key]);
                }

                result = await _HttpClient.PostAsync(queryString, stringContent);
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

                return await SerializationAdapter.DeserializeAsync<T>(data);
            }

            var text = await result.Content.ReadAsStringAsync();

            if (HttpStatusCodeFuncs.ContainsKey(result.StatusCode))
            {
                return (T)HttpStatusCodeFuncs[result.StatusCode].Invoke(text);
            }

            throw new HttpStatusException($"Nope {result.StatusCode}.\r\nBase Uri: {_HttpClient.BaseAddress}. Full Uri: {_HttpClient.BaseAddress + queryString}\r\nError:\r\n{text}", result.StatusCode);
        }
        #endregion

        #region Public Methods
        public async Task<T> GetAsync<T>()
        {
            return await GetAsync<T>(null);
        }

        public async Task<T> GetAsync<T>(string queryString, string contentType = "application/json")
        {
            return await Call<T>(queryString, false, contentType);
        }

        public async Task<TReturn> PostAsync<TReturn, TBody>(TBody body, string queryString, string contentType = "application/json")
        {
            return await Call<TReturn>(queryString, true, contentType, body);
        }
        #endregion
    }
}
