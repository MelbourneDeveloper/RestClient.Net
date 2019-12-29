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
        public IRestHeaders DefaultRequestHeaders { get; }
        public TimeSpan Timeout { get; set; }
        public ISerializationAdapter SerializationAdapter { get; }
        public ITracer Tracer { get; }
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
            default(Uri))
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
            TimeSpan timeout)
        : this(null,
              baseUri,
              serializationAdapter,
              null,
              timeout,
              null,
              null,
              null,
              default)
        {
        }

        public RestClient(
            ISerializationAdapter serializationAdapter,
            Uri baseUri,
            ITracer tracer)
        : this(
              null,
              baseUri,
              serializationAdapter,
              tracer,
              default,
              null,
              null,
              null,
              default)
        {
        }

        public RestClient(
            ISerializationAdapter serializationAdapter,
            IHttpClientFactory httpClientFactory)
        : this(
              null,
              null,
              serializationAdapter,
              null,
              default,
              httpClientFactory,
              null,
              null,
              default)
        {
        }

        public RestClient(
            ISerializationAdapter serializationAdapter,
            ITracer tracer,
            Uri baseUri,
            TimeSpan timeout,
            string name)
            : this(
                  name,
                baseUri,
                serializationAdapter,
                tracer,
                timeout,
                null,
                null,
                null,
                null)
        {
        }

        public RestClient(string name,
            Uri baseUri,
            ISerializationAdapter serializationAdapter,
            ITracer tracer,
            TimeSpan timeout,
            IHttpClientFactory httpClientFactory,
            IRestRequestConverter restRequestConverter,
            IRestHeaders defaultRequestHeaders,
            Func<HttpClient, Func<HttpRequestMessage>, CancellationToken, Task<HttpResponseMessage>> sendHttpRequestFunc)
        {
            SerializationAdapter = serializationAdapter;
            Tracer = tracer;
            BaseUri = baseUri;
            Timeout = timeout;
            DefaultRequestHeaders = defaultRequestHeaders ?? new RestRequestHeaders();
            Name = name ?? nameof(RestClient);
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

            Tracer?.Trace(restRequest.HttpVerb, httpResponseMessage.RequestMessage.RequestUri, requestBodyData, TraceType.Request, null, restRequest.Headers);

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

            var restHeadersCollection = new RestResponseHeaders(httpResponseMessage.Headers);

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

            Tracer?.Trace(
                restRequest.HttpVerb,
                httpResponseMessage.RequestMessage.RequestUri,
                responseData,
                TraceType.Response,
                (int)httpResponseMessage.StatusCode,
                restHeadersCollection);

            if (restResponse.IsSuccess || !ThrowExceptionOnFailure)
            {
                return restResponse;
            }

            throw new HttpStatusException($"{restResponse.StatusCode}.\r\nrestRequest.Resource: {restRequest.Resource}", restResponse, this);
        }
        #endregion
    }
}

#pragma warning restore CA2000