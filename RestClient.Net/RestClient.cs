using RestClientDotNet.Abstractions;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace RestClientDotNet
{
    public sealed class RestClient : IRestClient
    {
        #region Public Properties
        public IHttpClientFactory HttpClientFactory { get; }
        public IZip Zip { get; }
        public IRestHeaders DefaultRequestHeaders => HttpClientFactory.DefaultRequestHeaders;
        public TimeSpan Timeout { get => HttpClientFactory.Timeout; set => HttpClientFactory.Timeout = value; }
        public ISerializationAdapter SerializationAdapter { get; }
        public ITracer Tracer { get; }
        public bool ThrowExceptionOnFailure { get; set; } = true;
        public string DefaultContentType { get; set; } = "application/json";

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
        : this(
            serializationAdapter,
            new SingletonHttpClientFactory(timeout, baseUri))
        {
        }

        public RestClient(
            ISerializationAdapter serializationAdapter,
            Uri baseUri,
            ITracer tracer)
        : this(
          serializationAdapter,
          new SingletonHttpClientFactory(default, baseUri),
          tracer)
        {
        }

        public RestClient(
            ISerializationAdapter serializationAdapter,
            IHttpClientFactory httpClientFactory)
        : this(
          serializationAdapter,
          httpClientFactory,
          null)
        {
        }

        public RestClient(
       ISerializationAdapter serializationAdapter,
       IHttpClientFactory httpClientFactory,
       ITracer tracer)
        {
            SerializationAdapter = serializationAdapter;
            HttpClientFactory = httpClientFactory;
            Tracer = tracer;
        }

        #endregion

        #region Implementation
        async Task<RestResponseBase<TResponseBody>> IRestClient.SendAsync<TResponseBody, TRequestBody>(RestRequest<TRequestBody> restRequest)
        {
            var httpClient = HttpClientFactory.CreateHttpClient();

            HttpResponseMessage httpResponseMessage;

            switch (restRequest.HttpVerb)
            {
                case HttpVerb.Get:
                    Tracer?.Trace(restRequest.HttpVerb, HttpClientFactory.BaseUri, restRequest.Resource, null, TraceType.Request, null, HttpClientFactory.DefaultRequestHeaders);
                    httpResponseMessage = await httpClient.GetAsync(restRequest.Resource, restRequest.CancellationToken);
                    break;
                case HttpVerb.Delete:
                    Tracer?.Trace(restRequest.HttpVerb, HttpClientFactory.BaseUri, restRequest.Resource, null, TraceType.Request, null, HttpClientFactory.DefaultRequestHeaders);
                    httpResponseMessage = await httpClient.DeleteAsync(restRequest.Resource, restRequest.CancellationToken);
                    break;

                case HttpVerb.Post:
                case HttpVerb.Put:
                case HttpVerb.Patch:

                    var bodyData = await SerializationAdapter.SerializeAsync(restRequest.Body);

                    using (var httpContent = new ByteArrayContent(bodyData))
                    {
                        //Why do we have to set the content type only in cases where there is a request restRequest.Body, and headers?
                        httpContent.Headers.Add("Content-Type", restRequest.ContentType);

                        Tracer?.Trace(restRequest.HttpVerb, HttpClientFactory.BaseUri, restRequest.Resource, bodyData, TraceType.Request, null, HttpClientFactory.DefaultRequestHeaders);

                        if (restRequest.HttpVerb == HttpVerb.Put)
                        {
                            httpResponseMessage = await httpClient.PutAsync(restRequest.Resource, httpContent, restRequest.CancellationToken);
                        }
                        else if (restRequest.HttpVerb == HttpVerb.Post)
                        {
                            httpResponseMessage = await httpClient.PostAsync(restRequest.Resource, httpContent, restRequest.CancellationToken);
                        }
                        else
                        {
                            var method = new HttpMethod("PATCH");
                            using (var request = new HttpRequestMessage(method, restRequest.Resource)
                            {
                                Content = httpContent
                            })
                            {
                                httpResponseMessage = await httpClient.SendAsync(request, restRequest.CancellationToken);
                            }
                        }
                    }

                    break;
                default:
                    throw new NotImplementedException();
            }

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

            var responseBody = await SerializationAdapter.DeserializeAsync<TResponseBody>(responseData);

            var restHeadersCollection = new RestResponseHeaders(httpResponseMessage.Headers);

            var restResponse = new RestResponse<TResponseBody>
            (
                restHeadersCollection,
                (int)httpResponseMessage.StatusCode,
                HttpClientFactory.BaseUri,
                restRequest.Resource,
                restRequest.HttpVerb,
                responseData,
                responseBody,
                httpResponseMessage
            );

            Tracer?.Trace(
                restRequest.HttpVerb,
                HttpClientFactory.BaseUri,
                restRequest.Resource,
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
