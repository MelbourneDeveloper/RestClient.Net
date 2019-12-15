using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace RestClientDotNet
{
    public class RestClient : IDisposable, IRestClient
    {
        #region Fields
        private bool disposed;
        #endregion

        #region Public Properties
        public HttpClient HttpClient { get; }
        public string DefaultContentType { get; set; } = "application/json";
        public Uri BaseUri => HttpClient.BaseAddress;
        public Dictionary<string, string> Headers { get; private set; } = new Dictionary<string, string>();
        public AuthenticationHeaderValue Authorization { get; set; }
        public Dictionary<HttpStatusCode, Func<byte[], object>> HttpStatusCodeFuncs { get; } = new Dictionary<HttpStatusCode, Func<byte[], object>>();
        public IZip Zip { get; set; }
        public ISerializationAdapter SerializationAdapter { get; }
        public TimeSpan Timeout
        {
            get => HttpClient.Timeout;
            set => HttpClient.Timeout = value;
        }
        public ITracer Tracer { get; }
        #endregion

        #region Constructor
        public RestClient(ISerializationAdapter serializationAdapter) : this(serializationAdapter, null)
        {
        }

        public RestClient(ISerializationAdapter serializationAdapter, Uri baseUri, ITracer tracer) : this(serializationAdapter, baseUri, default, null, tracer)
        {
        }

        public RestClient(ISerializationAdapter serializationAdapter, Uri baseUri) : this(serializationAdapter, baseUri, default, null)
        {
        }

        public RestClient(ISerializationAdapter serializationAdapter, Uri baseUri, TimeSpan timeout) : this(serializationAdapter, baseUri, timeout, null)
        {
        }

        public RestClient(ISerializationAdapter serializationAdapter, Uri baseUri, TimeSpan timeout, HttpClient httpClient) : this(serializationAdapter, baseUri, timeout, httpClient, null)
        {
        }

        public RestClient(ISerializationAdapter serializationAdapter, Uri baseUri, TimeSpan timeout, HttpClient httpClient, ITracer tracer)
        {
            HttpClient = httpClient;

            if (HttpClient == null)
            {
                HttpClient = new HttpClient();
            }

            HttpClient.BaseAddress = baseUri;

            if (timeout != default)
            {
                HttpClient.Timeout = timeout;
            }

            SerializationAdapter = serializationAdapter;

            Tracer = tracer;
        }
        #endregion

        #region Private Methods
        private async Task<TReturn> Call<TReturn, TBody>(Uri queryString, HttpVerb httpVerb, string contentType, TBody body, CancellationToken cancellationToken)
        {
            HttpClient.DefaultRequestHeaders.Clear();

            if (Authorization != null)
            {
                HttpClient.DefaultRequestHeaders.Authorization = Authorization;
            }

            HttpResponseMessage result = null;
            var isPost = httpVerb == HttpVerb.Post;
            if (!isPost)
            {
                HttpClient.DefaultRequestHeaders.Clear();
                foreach (var key in Headers.Keys)
                {
                    HttpClient.DefaultRequestHeaders.Add(key, Headers[key]);
                }
            }


            switch (httpVerb)
            {

                case HttpVerb.Get:
                    Tracer?.Trace(httpVerb, BaseUri, queryString, null, TraceType.Request);
                    result = await HttpClient.GetAsync(queryString, cancellationToken);
                    break;
                case HttpVerb.Delete:
                    Tracer?.Trace(httpVerb, BaseUri, queryString, null, TraceType.Request);
                    result = await HttpClient.DeleteAsync(queryString, cancellationToken);
                    break;

                case HttpVerb.Post:
                case HttpVerb.Put:
                case HttpVerb.Patch:

                    var bodyData = await SerializationAdapter.SerializeAsync(body);

                    using (var httpContent = new ByteArrayContent(bodyData))
                    {

                        httpContent.Headers.Add("Content-Type", contentType);

                        Tracer?.Trace(httpVerb, BaseUri, queryString, bodyData, TraceType.Request);

                        if (httpVerb == HttpVerb.Put)
                        {
                            result = await HttpClient.PutAsync(queryString, httpContent, cancellationToken);
                        }
                        else if (httpVerb == HttpVerb.Post)
                        {
                            foreach (var key in Headers.Keys)
                            {
                                httpContent.Headers.Add(key, Headers[key]);
                            }

                            result = await HttpClient.PostAsync(queryString, httpContent, cancellationToken);
                            break;
                        }
                        else
                        {
                            var method = new HttpMethod("PATCH");
                            using (var request = new HttpRequestMessage(method, queryString)
                            {
                                Content = httpContent
                            })
                            {
                                result = await HttpClient.SendAsync(request, cancellationToken);
                            }
                        }
                    }

                    break;
                default:
                    throw new NotImplementedException();
            }

            byte[] responseData = null;
            if (result.IsSuccessStatusCode)
            {
                var gzipHeader = result.Content.Headers.ContentEncoding.FirstOrDefault(h => !string.IsNullOrEmpty(h) && h.Equals("gzip", StringComparison.OrdinalIgnoreCase));
                if (gzipHeader != null && Zip != null)
                {
                    var bytes = await result.Content.ReadAsByteArrayAsync();
                    responseData = Zip.Unzip(bytes);
                }
                else
                {
                    responseData = await result.Content.ReadAsByteArrayAsync();
                }

                Tracer?.Trace(httpVerb, BaseUri, queryString, responseData, TraceType.Response);

                return await SerializationAdapter.DeserializeAsync<TReturn>(responseData);
            }

            var errorData = await result.Content.ReadAsByteArrayAsync();

            if (HttpStatusCodeFuncs.ContainsKey(result.StatusCode))
            {
                return (TReturn)HttpStatusCodeFuncs[result.StatusCode].Invoke(errorData);
            }

            throw new HttpStatusException($"{result.StatusCode}.\r\nBase Uri: {HttpClient.BaseAddress}. Querystring: {queryString}", result);
        }
        #endregion

        #region Public Methods

        #region Get
        public Task<T> GetAsync<T>()
        {
            return Call<T, object>(null, HttpVerb.Get, DefaultContentType, null, default);
        }

        public Task<T> GetAsync<T>(string queryString)
        {
            try
            {
                return GetAsync<T>(new Uri(queryString, UriKind.Relative));
            }
            catch (UriFormatException ufe)
            {
                if (ufe.Message == "A relative URI cannot be created because the 'uriString' parameter represents an absolute URI.")
                {
                    throw new UriFormatException(Messages.ErrorMessageAbsoluteUriAsString, ufe);
                }

                throw;
            }
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
            return Call<T, object>(queryString, HttpVerb.Get, contentType, null, cancellationToken);
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
            return Call<TReturn, TBody>(queryString, HttpVerb.Post, contentType, body, cancellationToken);
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
            return Call<TReturn, TBody>(queryString, HttpVerb.Put, contentType, body, cancellationToken);
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
            return Call<object, object>(queryString, HttpVerb.Delete, contentType, null, cancellationToken);
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
            return Call<TReturn, object>(queryString, HttpVerb.Patch, contentType, body, cancellationToken);
        }
        #endregion

        public void Dispose()
        {
            if (disposed)
            {
                return;
            }

            disposed = true;

            GC.SuppressFinalize(this);

            HttpClient.Dispose();
        }

        #endregion
    }
}
