
#if NET45
using RestClient.Net.Abstractions.Logging;
#else
using Microsoft.Extensions.Logging;
#endif

using RestClient.Net.Abstractions;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

#pragma warning disable CA2000

namespace RestClient.Net
{

    public sealed class Client : IClient
    {
        #region Public Properties
        public IHttpClientFactory HttpClientFactory { get; }
        public IZip Zip { get; set; }
        public IHeadersCollection DefaultRequestHeaders { get; }
        public TimeSpan Timeout { get; set; }
        public ISerializationAdapter SerializationAdapter { get; }
        public ILogger Logger { get; }
        public bool ThrowExceptionOnFailure { get; set; } = true;
        public Uri BaseUri { get; set; }
        public string Name { get; }
        public IRequestConverter RequestConverter { get; }
        public Func<HttpClient, Func<HttpRequestMessage>, CancellationToken, Task<HttpResponseMessage>> SendHttpRequestFunc { get; }
        #endregion

        #region Func
        private static readonly Func<HttpClient, Func<HttpRequestMessage>, CancellationToken, Task<HttpResponseMessage>> DefaultSendHttpRequestMessageFunc = (httpClient, httpRequestMessageFunc, cancellationToken) =>
        {
            var httpRequestMessage = httpRequestMessageFunc.Invoke();
            return httpClient.SendAsync(httpRequestMessage, cancellationToken);
        };
        #endregion

        #region Constructors
        public Client(
            ISerializationAdapter serializationAdapter)
        : this(
            serializationAdapter,
            default)
        {
        }

        public Client(
            ISerializationAdapter serializationAdapter,
            Uri baseUri)
        : this(
            serializationAdapter,
            null,
            baseUri)
        {
        }

        public Client(
            ISerializationAdapter serializationAdapter,
            ILogger logger,
            IHttpClientFactory httpClientFactory)
        : this(
            serializationAdapter,
            null,
            null,
            logger: logger,
            httpClientFactory: httpClientFactory)
        {
        }

        public Client(
            ISerializationAdapter serializationAdapter,
            string name = null,
            Uri baseUri = null,
            IHeadersCollection defaultRequestHeaders = null,
            ILogger logger = null,
            IHttpClientFactory httpClientFactory = null,
            Func<HttpClient, Func<HttpRequestMessage>, CancellationToken, Task<HttpResponseMessage>> sendHttpRequestFunc = null,
            IRequestConverter requestConverter = null)
        {
            SerializationAdapter = serializationAdapter ?? throw new ArgumentNullException(nameof(serializationAdapter));
            Logger = logger;
            BaseUri = baseUri;
            Name = name ?? "RestClient";
            DefaultRequestHeaders = defaultRequestHeaders ?? new RequestHeadersCollection();
            RequestConverter = requestConverter ?? new DefaultRequestConverter();
            HttpClientFactory = httpClientFactory ?? new DefaultHttpClientFactory();
            SendHttpRequestFunc = sendHttpRequestFunc ?? DefaultSendHttpRequestMessageFunc;
        }

        #endregion

        #region Implementation
        async Task<Response<TResponseBody>> IClient.SendAsync<TResponseBody, TRequestBody>(Request<TRequestBody> request)
        {
            var httpClient = HttpClientFactory.CreateClient(Name);

            //Note: if HttpClient naming is not handled properly, this may alter the HttpClient of another RestClient
            if (httpClient.Timeout != Timeout && Timeout != default) httpClient.Timeout = Timeout;
            if (BaseUri != null) httpClient.BaseAddress = BaseUri;

            byte[] requestBodyData = null;

            if (DefaultRequestConverter.UpdateHttpRequestMethods.Contains(request.HttpRequestMethod))
            {
                requestBodyData = SerializationAdapter.Serialize(request.Body, request.Headers);
            }

            HttpResponseMessage httpResponseMessage = null;
            try
            {
                httpResponseMessage = await SendHttpRequestFunc.Invoke(
                    httpClient,
                    () => RequestConverter.GetHttpRequestMessage(request, requestBodyData),
                    request.CancellationToken
                    );
            }
            catch (TaskCanceledException tce)
            {
                Log(LogLevel.Error, null, tce);
                throw;
            }
            catch (OperationCanceledException oce)
            {
                Log(LogLevel.Error, null, oce);
                throw;
            }
            //TODO: Does this need to be handled like this?
            catch (Exception ex)
            {
                var exception = new SendException<TRequestBody>("HttpClient Send Exception", request, ex);
                Log(LogLevel.Error, null, exception);
                throw exception;
            }

            Log(LogLevel.Trace, new RestTrace
                (
                 request.HttpRequestMethod,
                 httpResponseMessage.RequestMessage.RequestUri,
                 requestBodyData,
                 RestEvent.Request,
                 null,
                 request.Headers
                ), null);

            return await ProcessResponseAsync<TResponseBody, TRequestBody>(request, httpResponseMessage);
        }

        private async Task<Response<TResponseBody>> ProcessResponseAsync<TResponseBody, TRequestBody>(Request<TRequestBody> request, HttpResponseMessage httpResponseMessage)
        {
            byte[] responseData = null;

            if (Zip != null)
            {
                //This is for cases where an unzipping utility needs to be used to unzip the content. This is actually a bug in UWP
                var gzipHeader = httpResponseMessage.Content.Headers.ContentEncoding.FirstOrDefault(h =>
                    !string.IsNullOrEmpty(h) && h.Equals("gzip", StringComparison.OrdinalIgnoreCase));
                if (gzipHeader != null)
                {
                    var bytes = await httpResponseMessage.Content.ReadAsByteArrayAsync();
                    responseData = Zip.Unzip(bytes);
                }
            }

            if (responseData == null)
            {
                responseData = await httpResponseMessage.Content.ReadAsByteArrayAsync();
            }

            var httpResponseHeadersCollection = new HttpResponseHeadersCollection(httpResponseMessage.Headers);

            TResponseBody responseBody;
            try
            {
                responseBody = SerializationAdapter.Deserialize<TResponseBody>(responseData, httpResponseHeadersCollection);
            }
            catch (Exception ex)
            {
                throw new DeserializationException(Messages.ErrorMessageDeserialization, responseData, this, ex);
            }

            var httpResponseMessageResponse = new HttpResponseMessageResponse<TResponseBody>
            (
                httpResponseHeadersCollection,
                (int)httpResponseMessage.StatusCode,
                request.HttpRequestMethod,
                responseData,
                responseBody,
                httpResponseMessage
            );

            Log(LogLevel.Trace, new RestTrace
            (
             request.HttpRequestMethod,
             httpResponseMessage.RequestMessage.RequestUri,
             responseData,
             RestEvent.Response,
             (int)httpResponseMessage.StatusCode,
             httpResponseHeadersCollection
            ), null);

            if (httpResponseMessageResponse.IsSuccess || !ThrowExceptionOnFailure)
            {
                return httpResponseMessageResponse;
            }

            throw new HttpStatusException($"Non successful Http Status Code: {httpResponseMessageResponse.StatusCode}.\r\nRequest Uri: {httpResponseMessage.RequestMessage.RequestUri}", httpResponseMessageResponse, this);
        }
        #endregion

        #region Private Methods
        private void Log(LogLevel loglevel, RestTrace restTrace, Exception exception)
        {
            Logger?.Log(loglevel,
                restTrace != null ?
                new EventId((int)restTrace.RestEvent, restTrace.RestEvent.ToString()) :
                new EventId((int)RestEvent.Error, RestEvent.Error.ToString()),
                restTrace, exception, null);
        }
        #endregion
    }
}

#pragma warning restore CA2000