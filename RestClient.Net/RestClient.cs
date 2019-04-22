using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace RestClientDotNet
{
    public class RestClient : IDisposable
    {
        #region Fields
        private const string DefaultContentType = "application/json";
        private readonly HttpClient _HttpClient = new HttpClient();
        private bool disposed;
        #endregion

        #region Public Properties
        public Uri BaseUri => _HttpClient.BaseAddress;
        public Dictionary<string, string> Headers { get; private set; } = new Dictionary<string, string>();
        public AuthenticationHeaderValue Authorization { get; set; }
        public Dictionary<HttpStatusCode, Func<byte[], object>> HttpStatusCodeFuncs { get; } = new Dictionary<HttpStatusCode, Func<byte[], object>>();
        public IZip Zip { get; set; }
        public ISerializationAdapter SerializationAdapter { get; }
        #endregion

        #region Constructor
        public RestClient(ISerializationAdapter serializationAdapter, Uri baseUri)
        {
            _HttpClient.BaseAddress = baseUri;
            _HttpClient.Timeout = new TimeSpan(0, 3, 0);
            SerializationAdapter = serializationAdapter;
        }
        #endregion

        #region Private Methods
        private async Task<T> Call<T>(Uri queryString, HttpVerb httpVerb, string contentType, object body = null)
        {
            _HttpClient.DefaultRequestHeaders.Clear();

            if (Authorization != null)
            {
                _HttpClient.DefaultRequestHeaders.Authorization = Authorization;
            }

            HttpResponseMessage result = null;
            var isPost = httpVerb == HttpVerb.Post;
            if (!isPost)
            {
                _HttpClient.DefaultRequestHeaders.Clear();
                foreach (var key in Headers.Keys)
                {
                    _HttpClient.DefaultRequestHeaders.Add(key, Headers[key]);
                }
            }

            string bodyString = null;
            StringContent stringContent = null;
            byte[] data = null;

            switch (httpVerb)
            {
                case HttpVerb.Post:

                    if (body is string bodyAsString)
                    {
                        bodyString = bodyAsString;
                    }
                    else
                    {
                        if (body != null)
                        {
                            data = await SerializationAdapter.SerializeAsync(body);
                            bodyString = SerializationAdapter.Encoding.GetString(data);
                        }
                        else
                        {
                            bodyString = string.Empty;
                        }
                    }

                    stringContent = new StringContent(bodyString, Encoding.UTF8, contentType);

                    //Don't know why but this has to be set again, otherwise more text is added on to the Content-Type header...
                    stringContent.Headers.ContentType = new MediaTypeHeaderValue(contentType);

                    stringContent.Headers.ContentLength = bodyString.Length;

                    foreach (var key in Headers.Keys)
                    {
                        stringContent.Headers.Add(key, Headers[key]);
                    }

                    result = await _HttpClient.PostAsync(queryString, stringContent);
                    break;

                case HttpVerb.Get:
                    result = await _HttpClient.GetAsync(queryString);
                    break;
                case HttpVerb.Delete:
                    result = await _HttpClient.DeleteAsync(queryString);
                    break;

                case HttpVerb.Put:
                case HttpVerb.Patch:

                    data = await SerializationAdapter.SerializeAsync(body);
                    bodyString = SerializationAdapter.Encoding.GetString(data);
                    var length = bodyString.Length;
                    stringContent = new StringContent(bodyString, SerializationAdapter.Encoding, contentType);

                    if (httpVerb == HttpVerb.Put)
                    {
                        result = await _HttpClient.PutAsync(queryString, stringContent);
                    }
                    else
                    {
                        var method = new HttpMethod("PATCH");
                        var request = new HttpRequestMessage(method, queryString)
                        {
                            Content = stringContent
                        };
                        result = await _HttpClient.SendAsync(request);
                    }
                    break;
                default:
                    throw new NotImplementedException();
            }

            if (result.IsSuccessStatusCode)
            {
                var gzipHeader = result.Content.Headers.ContentEncoding.FirstOrDefault(h => !string.IsNullOrEmpty(h) && h.Equals("gzip", StringComparison.OrdinalIgnoreCase));
                if (gzipHeader != null && Zip != null)
                {
                    var bytes = await result.Content.ReadAsByteArrayAsync();
                    data = Zip.Unzip(bytes);
                }
                else
                {
                    data = await result.Content.ReadAsByteArrayAsync();
                }

                return await SerializationAdapter.DeserializeAsync<T>(data);
            }

            var errorData = await result.Content.ReadAsByteArrayAsync();

            if (HttpStatusCodeFuncs.ContainsKey(result.StatusCode))
            {
                return (T)HttpStatusCodeFuncs[result.StatusCode].Invoke(errorData);
            }

            throw new HttpStatusException($"{result.StatusCode}.\r\nBase Uri: {_HttpClient.BaseAddress}. Full Uri: {new Uri(_HttpClient.BaseAddress, queryString)}", result.StatusCode, errorData);
        }
        #endregion

        #region Public Methods
        public async Task<T> GetAsync<T>(string contentType = DefaultContentType)
        {
            return await Call<T>(null, HttpVerb.Get, contentType);
        }

        public async Task<T> GetAsync<T>(string queryString, string contentType = DefaultContentType)
        {
            return await Call<T>(new Uri(queryString, UriKind.Relative), HttpVerb.Get, contentType);
        }

        public async Task<T> GetAsync<T>(Uri queryString, string contentType = DefaultContentType)
        {
            return await Call<T>(queryString, HttpVerb.Get, contentType);
        }

        public async Task<TReturn> PostAsync<TReturn, TBody>(TBody body, string queryString, string contentType = DefaultContentType)
        {
            return await Call<TReturn>(new Uri(queryString, UriKind.Relative), HttpVerb.Post, contentType, body);
        }

        public async Task<TReturn> PostAsync<TReturn, TBody>(TBody body, Uri queryString, string contentType = DefaultContentType)
        {
            return await Call<TReturn>(queryString, HttpVerb.Post, contentType, body);
        }

        public async Task<TReturn> PutAsync<TReturn, TBody>(TBody body, string queryString, string contentType = DefaultContentType)
        {
            return await Call<TReturn>(new Uri(queryString, UriKind.Relative), HttpVerb.Put, contentType, body);
        }

        public async Task<TReturn> PutAsync<TReturn, TBody>(TBody body, Uri queryString, string contentType = DefaultContentType)
        {
            return await Call<TReturn>(queryString, HttpVerb.Put, contentType, body);
        }

        public Task DeleteAsync(string queryString, string contentType = DefaultContentType)
        {
            return Call<object>(new Uri(queryString, UriKind.Relative), HttpVerb.Delete, contentType, null);
        }

        public Task DeleteAsync(Uri queryString, string contentType = DefaultContentType)
        {
            return Call<object>(queryString, HttpVerb.Delete, contentType, null);
        }

        public async Task<TReturn> PatchAsync<TReturn, TBody>(TBody body, string queryString, string contentType = DefaultContentType)
        {
            return await Call<TReturn>(new Uri(queryString, UriKind.Relative), HttpVerb.Patch, contentType, body);
        }

        public async Task<TReturn> PatchAsync<TReturn, TBody>(TBody body, Uri queryString, string contentType = DefaultContentType)
        {
            return await Call<TReturn>(queryString, HttpVerb.Patch, contentType, body);
        }

        public void Dispose()
        {
            if (disposed) return;
            disposed = true;

            GC.SuppressFinalize(this);

            _HttpClient.Dispose();
        }

        #endregion
    }
}
