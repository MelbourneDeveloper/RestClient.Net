using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
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
        private async Task<T> Call<T>(Uri queryString, HttpVerb httpVerb, string contentType, object body, CancellationToken cancellationToken)
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

                    result = await _HttpClient.PostAsync(queryString, stringContent, cancellationToken);
                    break;

                case HttpVerb.Get:
                    result = await _HttpClient.GetAsync(queryString, cancellationToken);
                    break;
                case HttpVerb.Delete:
                    result = await _HttpClient.DeleteAsync(queryString, cancellationToken);
                    break;

                case HttpVerb.Put:
                case HttpVerb.Patch:

                    data = await SerializationAdapter.SerializeAsync(body);
                    bodyString = SerializationAdapter.Encoding.GetString(data);
                    var length = bodyString.Length;
                    stringContent = new StringContent(bodyString, SerializationAdapter.Encoding, contentType);

                    if (httpVerb == HttpVerb.Put)
                    {
                        result = await _HttpClient.PutAsync(queryString, stringContent, cancellationToken);
                    }
                    else
                    {
                        var method = new HttpMethod("PATCH");
                        var request = new HttpRequestMessage(method, queryString)
                        {
                            Content = stringContent
                        };
                        result = await _HttpClient.SendAsync(request, cancellationToken);
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

            throw new HttpStatusException($"{result.StatusCode}.\r\nBase Uri: {_HttpClient.BaseAddress}. Querystring: {queryString}", result);
        }
        #endregion

        #region Public Methods

        #region Get
        public Task<T> GetAsync<T>()
        {
            return Call<T>(null, HttpVerb.Get, DefaultContentType, null, default);
        }

        public Task<T> GetAsync<T>(string queryString)
        {
            return GetAsync<T>(new Uri(queryString, UriKind.Relative));
        }

        public Task<T> GetAsync<T>(Uri queryString)
        {
            return GetAsync<T>(queryString, DefaultContentType);
        }

        public Task<T> GetAsync<T>(Uri queryString, string contentType)
        {
            return GetAsync<T>(queryString, contentType, default);
        }

        public Task<T> GetAsync<T>(Uri queryString, CancellationToken cancellationToken)
        {
            return GetAsync<T>(queryString, DefaultContentType, cancellationToken);
        }

        public Task<T> GetAsync<T>(Uri queryString, string contentType, CancellationToken cancellationToken)
        {
            return Call<T>(queryString, HttpVerb.Get, contentType, null, cancellationToken);
        }
        #endregion

        #region Post
        public Task<TReturn> PostAsync<TReturn, TBody>(TBody body, string queryString)
        {
            return PostAsync<TReturn, TBody>(body, new Uri(queryString, UriKind.Relative));
        }

        public Task<TReturn> PostAsync<TReturn, TBody>(TBody body, Uri queryString)
        {
            return PostAsync<TReturn, TBody>(body, queryString, default);
        }

        public Task<TReturn> PostAsync<TReturn, TBody>(TBody body, Uri queryString, CancellationToken cancellationToken)
        {
            return PostAsync<TReturn, TBody>(body, queryString, DefaultContentType, cancellationToken);
        }

        public Task<TReturn> PostAsync<TReturn, TBody>(TBody body, Uri queryString, string contentType, CancellationToken cancellationToken)
        {
            return Call<TReturn>(queryString, HttpVerb.Post, contentType, body, cancellationToken);
        }
        #endregion

        #region Put
        public Task<TReturn> PutAsync<TReturn, TBody>(TBody body, string queryString)
        {
            return PutAsync<TReturn, TBody>(body, new Uri(queryString, UriKind.Relative));
        }

        public Task<TReturn> PutAsync<TReturn, TBody>(TBody body, Uri queryString)
        {
            return PutAsync<TReturn, TBody>(body, queryString, DefaultContentType, default);
        }

        public Task<TReturn> PutAsync<TReturn, TBody>(TBody body, Uri queryString, CancellationToken cancellationToken)
        {
            return PutAsync<TReturn, TBody>(body, queryString, DefaultContentType, cancellationToken);
        }

        public Task<TReturn> PutAsync<TReturn, TBody>(TBody body, Uri queryString, string contentType, CancellationToken cancellationToken)
        {
            return Call<TReturn>(queryString, HttpVerb.Put, contentType, body, cancellationToken);
        }
        #endregion

        #region Delete
        public Task DeleteAsync(string queryString)
        {
            return DeleteAsync(new Uri(queryString, UriKind.Relative));
        }

        public Task DeleteAsync(Uri queryString)
        {
            return DeleteAsync(queryString, default);
        }

        public Task DeleteAsync(Uri queryString, CancellationToken cancellationToken)
        {
            return DeleteAsync(queryString, DefaultContentType, cancellationToken);
        }

        public Task DeleteAsync(Uri queryString, string contentType, CancellationToken cancellationToken)
        {
            return Call<object>(queryString, HttpVerb.Delete, contentType, null, cancellationToken);
        }
        #endregion

        #region Patch
        public Task<TReturn> PatchAsync<TReturn, TBody>(TBody body, string queryString)
        {
            return PatchAsync<TReturn, TBody>(body, new Uri(queryString, UriKind.Relative));
        }

        public Task<TReturn> PatchAsync<TReturn, TBody>(TBody body, Uri queryString)
        {
            return PatchAsync<TReturn, TBody>(body, queryString, DefaultContentType, default);
        }

        public Task<TReturn> PatchAsync<TReturn, TBody>(TBody body, Uri queryString, CancellationToken cancellationToken)
        {
            return PatchAsync<TReturn, TBody>(body, queryString, DefaultContentType, cancellationToken);
        }

        public Task<TReturn> PatchAsync<TReturn, TBody>(TBody body, Uri queryString, string contentType, CancellationToken cancellationToken)
        {
            return Call<TReturn>(queryString, HttpVerb.Patch, contentType, body, cancellationToken);
        }
        #endregion

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
