using System;
using System.Linq;
using System.Net.Http;
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
        public bool ThrowExceptionOnFailure { get; } = true;
        public HttpClient HttpClient { get; }
        public string DefaultContentType { get; set; } = "application/json";
        public Uri BaseUri => HttpClient.BaseAddress;
        public IRestHeadersCollection DefaultRequestHeaders { get; }
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

            if (HttpClient.BaseAddress == null && baseUri != null)
            {
                HttpClient.BaseAddress = baseUri;
            }

            if (timeout != default)
            {
                HttpClient.Timeout = timeout;
            }

            DefaultRequestHeaders = new RestRequestHeadersCollection(HttpClient.DefaultRequestHeaders);

            SerializationAdapter = serializationAdapter;

            Tracer = tracer;
        }
        #endregion

        #region Private Methods
        private async Task<RestResponse<TReturn>> Call<TReturn, TBody>(Uri queryString, HttpVerb httpVerb, string contentType, TBody body, CancellationToken cancellationToken)
        {
            HttpResponseMessage result = null;

            switch (httpVerb)
            {
                case HttpVerb.Get:
                    Tracer?.Trace(httpVerb, BaseUri, queryString, null, TraceType.Request, null, DefaultRequestHeaders);
                    result = await HttpClient.GetAsync(queryString, cancellationToken);
                    break;
                case HttpVerb.Delete:
                    Tracer?.Trace(httpVerb, BaseUri, queryString, null, TraceType.Request, null, DefaultRequestHeaders);
                    result = await HttpClient.DeleteAsync(queryString, cancellationToken);
                    break;

                case HttpVerb.Post:
                case HttpVerb.Put:
                case HttpVerb.Patch:

                    var bodyData = await SerializationAdapter.SerializeAsync(body);

                    using (var httpContent = new ByteArrayContent(bodyData))
                    {
                        //Why do we have to set the content type only in cases where there is a request body, and headers?
                        httpContent.Headers.Add("Content-Type", contentType);

                        Tracer?.Trace(httpVerb, BaseUri, queryString, bodyData, TraceType.Request, null, DefaultRequestHeaders);

                        if (httpVerb == HttpVerb.Put)
                        {
                            result = await HttpClient.PutAsync(queryString, httpContent, cancellationToken);
                        }
                        else if (httpVerb == HttpVerb.Post)
                        {
                            result = await HttpClient.PostAsync(queryString, httpContent, cancellationToken);
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
                if (Zip != null)
                {
                    //This is for cases where an unzipping utility needs to be used to unzip the content. This is actually a bug in UWP
                    var gzipHeader = result.Content.Headers.ContentEncoding.FirstOrDefault(h => !string.IsNullOrEmpty(h) && h.Equals("gzip", StringComparison.OrdinalIgnoreCase));
                    if (gzipHeader != null)
                    {
                        var bytes = await result.Content.ReadAsByteArrayAsync();
                        responseData = Zip.Unzip(bytes);
                    }
                }

                if (responseData == null)
                {
                    responseData = await result.Content.ReadAsByteArrayAsync();
                }


                var bodyObject = await SerializationAdapter.DeserializeAsync<TReturn>(responseData);

                var restHeadersCollection = new RestResponseHeadersCollection(result.Headers);

                var restResponse = new RestResponse<TReturn>(bodyObject, restHeadersCollection, (int)result.StatusCode, result);

                Tracer?.Trace(httpVerb, BaseUri, queryString, responseData, TraceType.Response, result.StatusCode, restHeadersCollection);

                return restResponse;
            }

            var errorResponse = new RestResponse<TReturn>(default, new RestResponseHeadersCollection(result.Headers), (int)result.StatusCode, result);

            //TODO: It's possible to deserialize errors here. What's the best way?

            if (ThrowExceptionOnFailure)
            {
                throw new HttpStatusException(
                    $"{result.StatusCode}.\r\nBase Uri: {HttpClient.BaseAddress}. Querystring: {queryString}", errorResponse);
            }
            else
            {
                return errorResponse;
            }
        }
        #endregion

        #region Public Methods

        #region Get
        public Task<RestResponse<T>> GetAsync<T>()
        {
            return Call<T, object>(null, HttpVerb.Get, DefaultContentType, null, default);
        }

        public Task<RestResponse<T>> GetAsync<T>(string queryString)
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

        public Task<RestResponse<T>> GetAsync<T>(Uri queryString)
        {
            return GetAsync<T>(queryString, DefaultContentType);
        }

        public Task<RestResponse<T>> GetAsync<T>(Uri queryString, string contentType)
        {
            return GetAsync<T>(queryString, contentType, default);
        }

        public Task<RestResponse<T>> GetAsync<T>(Uri queryString, CancellationToken cancellationToken)
        {
            return GetAsync<T>(queryString, DefaultContentType, cancellationToken);
        }

        public Task<RestResponse<T>> GetAsync<T>(Uri queryString, string contentType, CancellationToken cancellationToken)
        {
            return Call<T, object>(queryString, HttpVerb.Get, contentType, null, cancellationToken);
        }
        #endregion

        #region Post
        public Task<RestResponse<TReturn>> PostAsync<TReturn, TBody>(TBody body)
        {
            return PostAsync<TReturn, TBody>(body, default(Uri));
        }

        public Task<RestResponse<TReturn>> PostAsync<TReturn, TBody>(TBody body, string queryString)
        {
            return PostAsync<TReturn, TBody>(body, new Uri(queryString, UriKind.Relative));
        }

        public Task<RestResponse<TReturn>> PostAsync<TReturn, TBody>(TBody body, Uri queryString)
        {
            return PostAsync<TReturn, TBody>(body, queryString, default);
        }

        public Task<RestResponse<TReturn>> PostAsync<TReturn, TBody>(TBody body, Uri queryString, CancellationToken cancellationToken)
        {
            return PostAsync<TReturn, TBody>(body, queryString, DefaultContentType, cancellationToken);
        }

        public Task<RestResponse<TReturn>> PostAsync<TReturn, TBody>(TBody body, Uri queryString, string contentType, CancellationToken cancellationToken)
        {
            return Call<TReturn, TBody>(queryString, HttpVerb.Post, contentType, body, cancellationToken);
        }
        #endregion

        #region Put
        public Task<RestResponse<TReturn>> PutAsync<TReturn, TBody>(TBody body, string queryString)
        {
            return PutAsync<TReturn, TBody>(body, new Uri(queryString, UriKind.Relative));
        }

        public Task<RestResponse<TReturn>> PutAsync<TReturn, TBody>(TBody body, Uri queryString)
        {
            return PutAsync<TReturn, TBody>(body, queryString, DefaultContentType, default);
        }

        public Task<RestResponse<TReturn>> PutAsync<TReturn, TBody>(TBody body, Uri queryString, CancellationToken cancellationToken)
        {
            return PutAsync<TReturn, TBody>(body, queryString, DefaultContentType, cancellationToken);
        }

        public Task<RestResponse<TReturn>> PutAsync<TReturn, TBody>(TBody body, Uri queryString, string contentType, CancellationToken cancellationToken)
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
        public Task<RestResponse<TReturn>> PatchAsync<TReturn, TBody>(TBody body, string queryString)
        {
            return PatchAsync<TReturn, TBody>(body, new Uri(queryString, UriKind.Relative));
        }

        public Task<RestResponse<TReturn>> PatchAsync<TReturn, TBody>(TBody body, Uri queryString)
        {
            return PatchAsync<TReturn, TBody>(body, queryString, DefaultContentType, default);
        }

        public Task<RestResponse<TReturn>> PatchAsync<TReturn, TBody>(TBody body, Uri queryString, CancellationToken cancellationToken)
        {
            return PatchAsync<TReturn, TBody>(body, queryString, DefaultContentType, cancellationToken);
        }

        public Task<RestResponse<TReturn>> PatchAsync<TReturn, TBody>(TBody body, Uri queryString, string contentType, CancellationToken cancellationToken)
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
