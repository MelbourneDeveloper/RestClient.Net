using RestClientDotNet.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

#pragma warning disable CA2000

namespace RestClientDotNet
{
    public sealed class RestClient : IRestClient
    {
        #region Public Properties
        public IHttpClientFactory HttpClientFactory { get; }
        public IZip Zip { get; }
        public IRestHeaders DefaultRequestHeaders { get; }
        public TimeSpan Timeout { get; set; }
        public ISerializationAdapter SerializationAdapter { get; }
        public ITracer Tracer { get; }
        public bool ThrowExceptionOnFailure { get; set; } = true;
        public Uri BaseUri { get; }
        public string Name { get; }
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
              new DefaultHttpClientFactory(),
              null,
              baseUri,
              timeout,
              null)
        {
        }

        public RestClient(
            ISerializationAdapter serializationAdapter,
            Uri baseUri,
            ITracer tracer)
        : this(
          serializationAdapter,
          new DefaultHttpClientFactory(),
          tracer,
          baseUri,
          default,
          null)
        {
        }

        public RestClient(
            ISerializationAdapter serializationAdapter,
            IHttpClientFactory httpClientFactory)
        : this(
          serializationAdapter,
          httpClientFactory,
          null,
          null,
          default,
          null)
        {
        }

        public RestClient(
        ISerializationAdapter serializationAdapter,
        IHttpClientFactory httpClientFactory,
        ITracer tracer,
        Uri baseUri,
        TimeSpan timeout,
        string name)
        {
            SerializationAdapter = serializationAdapter;
            HttpClientFactory = httpClientFactory;
            Tracer = tracer;
            BaseUri = baseUri;
            Timeout = timeout;
            DefaultRequestHeaders = new RestRequestHeaders();
            Name = name ?? nameof(RestClient);
        }

        #endregion

        #region Implementation
        async Task<RestResponseBase<TResponseBody>> IRestClient.SendAsync<TResponseBody, TRequestBody>(RestRequest<TRequestBody> restRequest)
        {
            var httpClient = HttpClientFactory.CreateClient(Name);

            //Note: if HttpClient naming is not handled properly, this may alter the HttpClient of another RestClient
            if (httpClient.Timeout != Timeout && Timeout != default) httpClient.Timeout = Timeout;
            if (BaseUri != null) httpClient.BaseAddress = BaseUri;

            HttpMethod httpMethod;
            switch (restRequest.HttpVerb)
            {
                case HttpVerb.Get:
                    httpMethod = HttpMethod.Get;
                    break;
                case HttpVerb.Post:
                    httpMethod = HttpMethod.Post;
                    break;
                case HttpVerb.Put:
                    httpMethod = HttpMethod.Put;
                    break;
                case HttpVerb.Delete:
                    httpMethod = HttpMethod.Delete;
                    break;
                case HttpVerb.Patch:
                    httpMethod = new HttpMethod("PATCH");
                    break;
                default:
                    throw new NotImplementedException();
            }

            using (var httpRequestMessage = new HttpRequestMessage
            {
                Method = httpMethod,
                RequestUri = restRequest.Resource
            })
            {
                byte[] requestBodyData = null;
                ByteArrayContent httpContent = null;
                if (new List<HttpVerb> { HttpVerb.Put, HttpVerb.Post, HttpVerb.Patch }.Contains(restRequest.HttpVerb))
                {
                    requestBodyData = SerializationAdapter.Serialize(restRequest.Body, restRequest.Headers);
                    httpContent = new ByteArrayContent(requestBodyData);
                    httpRequestMessage.Content = httpContent;
                }

                foreach (var headerName in restRequest.Headers.Names)
                {
                    if (string.Compare(headerName, "Content-Type", StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        //Note: not sure why this is necessary...
                        //The HttpClient class seems to differentiate between content headers and request message headers, but this dictinctionary doesn't exist in the real world...
                        if (httpContent != null)
                        {
                            httpContent.Headers.Add("Content-Type", restRequest.Headers[headerName]);
                        }
                    }
                    else
                    {
                        httpRequestMessage.Headers.Add(headerName, restRequest.Headers[headerName]);
                    }
                }

                Tracer?.Trace(restRequest.HttpVerb, httpClient.BaseAddress, restRequest.Resource, requestBodyData, TraceType.Request, null, restRequest.Headers);

                var httpResponseMessage = await httpClient.SendAsync(httpRequestMessage, restRequest.CancellationToken);

                return await ProcessResponseAsync<TResponseBody, TRequestBody>(restRequest, httpClient, httpResponseMessage);
            }
        }

        private async Task<RestResponseBase<TResponseBody>> ProcessResponseAsync<TResponseBody, TRequestBody>(RestRequest<TRequestBody> restRequest, HttpClient httpClient, HttpResponseMessage httpResponseMessage)
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

            var responseBody = default(TResponseBody);
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
                httpClient.BaseAddress,
                restRequest.Resource,
                restRequest.HttpVerb,
                responseData,
                responseBody,
                httpResponseMessage
            );

            Tracer?.Trace(
                restRequest.HttpVerb,
                httpClient.BaseAddress,
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

#pragma warning restore CA2000