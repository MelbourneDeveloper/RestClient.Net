using RestClientDotNet.Abstractions;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace RestClientDotNet
{
    public class ResponseProcessor : IResponseProcessor
    {
        #region Public Properties
        public IZip Zip { get; }
        public ISerializationAdapter SerializationAdapter { get; }
        public ITracer Tracer { get; }
        public IHttpClientFactory HttpClientFactory { get; }
        public IRestHeadersCollection DefaultRequestHeaders => HttpClientFactory.DefaultRequestHeaders;
        public TimeSpan Timeout { get => HttpClientFactory.Timeout; set => HttpClientFactory.Timeout = value; }
        #endregion

        #region Constructor
        public ResponseProcessor
            (
                IZip zip,
                ISerializationAdapter serializationAdapter,
                ITracer tracer,
                IHttpClientFactory httpClientFactory
            )
        {
            Zip = zip;
            SerializationAdapter = serializationAdapter;
            Tracer = tracer;
            HttpClientFactory = httpClientFactory;
        }
        #endregion

        #region Implementation
        public async Task<RestResponseBase<TReturn>> SendAsync<TReturn, TBody>(Uri resource, HttpVerb httpVerb, TBody body, string contentType, CancellationToken cancellationToken)
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

            return restResponse;
        }
        #endregion
    }
}
