using RestClientDotNet.Abstractions;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace RestClientDotNet
{
    public sealed class RestClient : IRestClient
    {
        #region Public Properties
        public IHttpClientFactory HttpClientFactory { get; }
        public IZip Zip { get; }
        public IRestHeadersCollection DefaultRequestHeaders => HttpClientFactory.DefaultRequestHeaders;
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
        async Task<RestResponseBase<TReturn>> IRestClient.SendAsync<TReturn, TBody>(Uri resource, HttpVerb httpVerb, string contentType, TBody body, CancellationToken cancellationToken)
        {
            var httpClient = HttpClientFactory.CreateHttpClient();

            HttpResponseMessage httpResponseMessage;

            switch (httpVerb)
            {
                case HttpVerb.Get:
                    Tracer?.Trace(httpVerb, HttpClientFactory.BaseUri, resource, null, TraceType.Request, null, HttpClientFactory.DefaultRequestHeaders);
                    httpResponseMessage = await httpClient.GetAsync(resource, cancellationToken);
                    break;
                case HttpVerb.Delete:
                    Tracer?.Trace(httpVerb, HttpClientFactory.BaseUri, resource, null, TraceType.Request, null, HttpClientFactory.DefaultRequestHeaders);
                    httpResponseMessage = await httpClient.DeleteAsync(resource, cancellationToken);
                    break;

                case HttpVerb.Post:
                case HttpVerb.Put:
                case HttpVerb.Patch:

                    var bodyData = await SerializationAdapter.SerializeAsync(body);

                    using (var httpContent = new ByteArrayContent(bodyData))
                    {
                        //Why do we have to set the content type only in cases where there is a request body, and headers?
                        httpContent.Headers.Add("Content-Type", contentType);

                        Tracer?.Trace(httpVerb, HttpClientFactory.BaseUri, resource, bodyData, TraceType.Request, null, HttpClientFactory.DefaultRequestHeaders);

                        if (httpVerb == HttpVerb.Put)
                        {
                            httpResponseMessage = await httpClient.PutAsync(resource, httpContent, cancellationToken);
                        }
                        else if (httpVerb == HttpVerb.Post)
                        {
                            httpResponseMessage = await httpClient.PostAsync(resource, httpContent, cancellationToken);
                        }
                        else
                        {
                            var method = new HttpMethod("PATCH");
                            using (var request = new HttpRequestMessage(method, resource)
                            {
                                Content = httpContent
                            })
                            {
                                httpResponseMessage = await httpClient.SendAsync(request, cancellationToken);
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

            var bodyObject = await SerializationAdapter.DeserializeAsync<TReturn>(responseData);

            var restHeadersCollection = new RestResponseHeadersCollection(httpResponseMessage.Headers);

            var restResponse = new RestResponse<TReturn>
            (
                restHeadersCollection,
                (int)httpResponseMessage.StatusCode,
                HttpClientFactory.BaseUri,
                resource,
                httpVerb,
                responseData,
                bodyObject,
                httpResponseMessage
            );

            Tracer?.Trace(
                httpVerb,
                HttpClientFactory.BaseUri,
                resource,
                responseData,
                TraceType.Response,
                (int)httpResponseMessage.StatusCode,
                restHeadersCollection);

            if (restResponse.IsSuccess || !ThrowExceptionOnFailure)
            {
                return restResponse;
            }

            throw new HttpStatusException($"{restResponse.StatusCode}.\r\nResource: {resource}", restResponse);
        }
        #endregion
    }
}
