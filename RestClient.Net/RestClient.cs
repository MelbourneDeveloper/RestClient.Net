using RestClientDotNet.Abstractions;
using System;
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
        public IResponseProcessorFactory ResponseProcessorFactory { get; } = new ResponseProcessorFactory();
        public bool ThrowExceptionOnFailure { get; set; } = true;
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
            var responseProcessor = await ResponseProcessorFactory.GetResponseProcessor
                (
                httpVerb,
                BaseUri,
                queryString,
                body,
                contentType,
                DefaultRequestHeaders,
                cancellationToken);

            if (responseProcessor.IsSuccess)
            {
                return await responseProcessor.GetRestResponse<TReturn>(BaseUri, queryString, httpVerb);
            }

            var errorResponse = new RestResponse<TReturn>(
                default,
                responseProcessor.Headers,
                responseProcessor.StatusCode,
                responseProcessor.UnderlyingResponseMessage,
                responseProcessor
                );

            if (ThrowExceptionOnFailure)
            {
                throw new HttpStatusException(
                    $"{responseProcessor.StatusCode}.\r\nBase Uri: {HttpClient.BaseAddress}. Querystring: {queryString}", errorResponse);
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

    //public class Afasdasd
    //{
    //    private ITracer Tracer;
    //    private ISerializationAdapter SerializationAdapter;
    //    private IZip Zip;


    //}

}
