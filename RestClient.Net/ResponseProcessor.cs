using RestClientDotNet.Abstractions;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace RestClientDotNet
{
    public class ResponseProcessor : IResponseProcessor
    {
        private IZip Zip { get; }
        private ISerializationAdapter SerializationAdapter { get; }
        public bool IsSuccess => _httpResponseMessage.IsSuccessStatusCode;
        public int StatusCode => (int)_httpResponseMessage.StatusCode;
        public IRestHeadersCollection Headers { get; }
        public object UnderlyingResponseMessage => _httpResponseMessage;

        private readonly HttpResponseMessage _httpResponseMessage;
        private readonly ITracer Tracer;

        public ResponseProcessor
            (
                IZip zip,
                ISerializationAdapter serializationAdapter,
                HttpResponseMessage httpResponseMessage,
                ITracer tracer
            )
        {
            Zip = zip;
            SerializationAdapter = serializationAdapter;
            _httpResponseMessage = httpResponseMessage ?? throw new ArgumentNullException(nameof(httpResponseMessage));
            Tracer = tracer;
            Headers = new RestResponseHeadersCollection(httpResponseMessage.Headers);
        }

        public async Task<RestResponse<TReturn>> GetRestResponse<TReturn>(Uri baseUri, Uri queryString, HttpVerb httpVerb)
        {
            byte[] responseData = null;

            if (Zip != null)
            {
                //This is for cases where an unzipping utility needs to be used to unzip the content. This is actually a bug in UWP
                var gzipHeader = _httpResponseMessage.Content.Headers.ContentEncoding.FirstOrDefault(h =>
                    !string.IsNullOrEmpty(h) && h.Equals("gzip", StringComparison.OrdinalIgnoreCase));
                if (gzipHeader != null)
                {
                    var bytes = await _httpResponseMessage.Content.ReadAsByteArrayAsync();
                    responseData = Zip.Unzip(bytes);
                }
            }

            if (responseData == null)
            {
                responseData = await _httpResponseMessage.Content.ReadAsByteArrayAsync();
            }

            var bodyObject = await SerializationAdapter.DeserializeAsync<TReturn>(responseData);

            var restHeadersCollection = new RestResponseHeadersCollection(_httpResponseMessage.Headers);

            var restResponse = new RestResponse<TReturn>(
                bodyObject,
                restHeadersCollection,
                (int)_httpResponseMessage.StatusCode,
                _httpResponseMessage,
                this
            );

            Tracer?.Trace(
                httpVerb,
                baseUri,
                queryString,
                responseData,
                TraceType.Response,
                _httpResponseMessage.StatusCode,
                restHeadersCollection);

            return restResponse;
        }
    }
}
