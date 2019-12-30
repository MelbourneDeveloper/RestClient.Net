
#if NET45
using RestClientDotNet.Abstractions.Logging;
#else
using Microsoft.Extensions.Logging;
#endif

using RestClientDotNet.Abstractions;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

#pragma warning disable CA2000

namespace RestClientDotNet
{

    public sealed class RestClient : IRestClient
    {
        #region Public Properties
        public IHttpClientFactory HttpClientFactory { get; }
        public IZip Zip { get; set; }
        public IRestHeadersCollection DefaultRequestHeaders { get; }
        public TimeSpan Timeout { get; set; }
        public ISerializationAdapter SerializationAdapter { get; }
        public ILogger Logger { get; }
        public bool ThrowExceptionOnFailure { get; set; } = true;
        public Uri BaseUri { get; }
        public string Name { get; }
        public IRestRequestConverter RestRequestConverter { get; }
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
        public RestClient(
            ISerializationAdapter serializationAdapter)
        : this(
            serializationAdapter,
            default)
        {
        }

        public RestClient(
            ISerializationAdapter serializationAdapter,
            Uri baseUri)
        : this(
            serializationAdapter,
            baseUri,
            null)
        {
        }

        public RestClient(
            ISerializationAdapter serializationAdapter,
            Uri baseUri,
            ILogger logger)
        : this(
              serializationAdapter,
            baseUri,
            logger,
            null,
            null)
        {
        }

        public RestClient(
            ISerializationAdapter serializationAdapter,
            ILogger logger,
            IHttpClientFactory httpClientFactory)
        : this(
            serializationAdapter,
            null,
            logger,
            httpClientFactory,
            null,
            default,
            null,
            null)
        {
        }

        public RestClient(
            ISerializationAdapter serializationAdapter,
            Uri baseUri,
            ILogger logger,
            string name,
            IHttpClientFactory httpClientFactory)
        : this(
              serializationAdapter,
            baseUri,
            logger,
            httpClientFactory,
            name,
            default,
            null,
            null)
        {
        }

        public RestClient(ISerializationAdapter serializationAdapter,
            Uri baseUri,
            ILogger logger,
            IHttpClientFactory httpClientFactory,
            string name,
            Func<HttpClient, Func<HttpRequestMessage>, CancellationToken, Task<HttpResponseMessage>> sendHttpRequestFunc = null,
            IRestRequestConverter restRequestConverter = null,
            IRestHeadersCollection defaultRequestHeaders = null)
        {
            SerializationAdapter = serializationAdapter ?? throw new ArgumentNullException(nameof(serializationAdapter));
            Logger = logger;
            BaseUri = baseUri;
            Name = name ?? nameof(RestClient);
            DefaultRequestHeaders = defaultRequestHeaders ?? new RestRequestHeadersCollection();
            RestRequestConverter = restRequestConverter ?? new DefaultRestRequestConverter();
            HttpClientFactory = httpClientFactory ?? new DefaultHttpClientFactory();
            SendHttpRequestFunc = sendHttpRequestFunc ?? DefaultSendHttpRequestMessageFunc;
        }

        #endregion

        #region Implementation
        async Task<RestResponseBase<TResponseBody>> IRestClient.SendAsync<TResponseBody, TRequestBody>(RestRequest<TRequestBody> restRequest)
        {
            var httpClient = HttpClientFactory.CreateClient(Name);

            //Note: if HttpClient naming is not handled properly, this may alter the HttpClient of another RestClient
            if (httpClient.Timeout != Timeout && Timeout != default) httpClient.Timeout = Timeout;
            if (BaseUri != null) httpClient.BaseAddress = BaseUri;

            byte[] requestBodyData = null;

            if (DefaultRestRequestConverter.UpdateVerbs.Contains(restRequest.HttpVerb))
            {
                requestBodyData = SerializationAdapter.Serialize(restRequest.Body, restRequest.Headers);
            }

            var httpResponseMessage = await SendHttpRequestFunc.Invoke(
                httpClient,
                () => RestRequestConverter.GetHttpRequestMessage(restRequest, requestBodyData),
                restRequest.CancellationToken
                );

            Log(LogLevel.Trace, new RestTrace
                (
                 restRequest.HttpVerb,
                 httpResponseMessage.RequestMessage.RequestUri,
                 requestBodyData,
                 TraceType.Request,
                 null,
                 restRequest.Headers
                ), null);

            return await ProcessResponseAsync<TResponseBody, TRequestBody>(restRequest, httpResponseMessage);
        }

        private async Task<RestResponseBase<TResponseBody>> ProcessResponseAsync<TResponseBody, TRequestBody>(RestRequest<TRequestBody> restRequest, HttpResponseMessage httpResponseMessage)
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

            var restHeadersCollection = new RestResponseHeadersCollection(httpResponseMessage.Headers);

            TResponseBody responseBody;
            try
            {
                responseBody = SerializationAdapter.Deserialize<TResponseBody>(responseData, restHeadersCollection);
            }
            catch (Exception ex)
            {
                throw new DeserializationException(Messages.ErrorMessageDeserialization, responseData, this, ex);
            }

            var restResponse = new RestResponse<TResponseBody>
            (
                restHeadersCollection,
                (int)httpResponseMessage.StatusCode,
                restRequest.HttpVerb,
                responseData,
                responseBody,
                httpResponseMessage
            );

            Log(LogLevel.Trace, new RestTrace
            (
             restRequest.HttpVerb,
             httpResponseMessage.RequestMessage.RequestUri,
             responseData,
             TraceType.Response,
             (int)httpResponseMessage.StatusCode,
             restHeadersCollection
            ), null);

            if (restResponse.IsSuccess || !ThrowExceptionOnFailure)
            {
                return restResponse;
            }

            throw new HttpStatusException($"{restResponse.StatusCode}.\r\nrestRequest.Resource: {restRequest.Resource}", restResponse, this);
        }
        #endregion

        #region Private Methods
        private void Log(LogLevel loglevel, RestTrace restTrace, Exception exception)
        {
            Logger?.Log(loglevel, new EventId((int)restTrace.TraceType, restTrace.TraceType.ToString()), restTrace, exception, null);
        }
        #endregion
    }
}

#pragma warning restore CA2000