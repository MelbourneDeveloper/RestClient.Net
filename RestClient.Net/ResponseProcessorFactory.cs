using RestClientDotNet.Abstractions;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace RestClientDotNet
{
    public class ResponseProcessorFactory : IResponseProcessorFactory
    {
        #region Public Properties
        public IZip Zip { get; }
        public ISerializationAdapter SerializationAdapter { get; }
        public ITracer Tracer { get; }
        public IHttpClientFactory HttpClientFactory { get; }
        public TimeSpan Timeout { get => HttpClientFactory.Timeout; set => HttpClientFactory.Timeout = value; }
        public Uri BaseAddress => HttpClientFactory.BaseUri;
        public IRestHeadersCollection DefaultRequestHeaders => HttpClientFactory.DefaultRequestHeaders;
        #endregion

        #region Constructor
        public ResponseProcessorFactory(
            ISerializationAdapter serializationAdapter,
            ITracer tracer,
            IHttpClientFactory httpClientFactory,
            IZip zip
            )
        {

            SerializationAdapter = serializationAdapter ?? throw new ArgumentNullException(nameof(serializationAdapter));
            HttpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
            Zip = zip;
            Tracer = tracer;
        }
        #endregion

        #region Implementation
        public async Task<IResponseProcessor> CreateResponseProcessorAsync<TBody>(HttpVerb httpVerb, Uri baseUri, Uri queryString, TBody body, string contentType, CancellationToken cancellationToken)
        {
            var httpClient = HttpClientFactory.CreateHttpClient();

            HttpResponseMessage httpResponseMessage;

            switch (httpVerb)
            {
                case HttpVerb.Get:
                    Tracer?.Trace(httpVerb, baseUri, queryString, null, TraceType.Request, null, DefaultRequestHeaders);
                    httpResponseMessage = await httpClient.GetAsync(queryString, cancellationToken);
                    break;
                case HttpVerb.Delete:
                    Tracer?.Trace(httpVerb, baseUri, queryString, null, TraceType.Request, null, DefaultRequestHeaders);
                    httpResponseMessage = await httpClient.DeleteAsync(queryString, cancellationToken);
                    break;

                case HttpVerb.Post:
                case HttpVerb.Put:
                case HttpVerb.Patch:

                    var bodyData = await SerializationAdapter.SerializeAsync(body);

                    using (var httpContent = new ByteArrayContent(bodyData))
                    {
                        //Why do we have to set the content type only in cases where there is a request body, and headers?
                        httpContent.Headers.Add("Content-Type", contentType);

                        Tracer?.Trace(httpVerb, baseUri, queryString, bodyData, TraceType.Request, null, DefaultRequestHeaders);

                        if (httpVerb == HttpVerb.Put)
                        {
                            httpResponseMessage = await httpClient.PutAsync(queryString, httpContent, cancellationToken);
                        }
                        else if (httpVerb == HttpVerb.Post)
                        {
                            httpResponseMessage = await httpClient.PostAsync(queryString, httpContent, cancellationToken);
                        }
                        else
                        {
                            var method = new HttpMethod("PATCH");
                            using (var request = new HttpRequestMessage(method, queryString)
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

            var responseProcessor = new ResponseProcessor
            (
                Zip,
                SerializationAdapter,
                httpResponseMessage,
                Tracer
            );

            return responseProcessor;
        }

        public void Dispose()
        {
        }
        #endregion
    }

}
