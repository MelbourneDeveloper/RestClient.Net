using RestClientDotNet.Abstractions;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace RestClientDotNet
{
    public class ResponseProcessor : IResponseProcessor
    {
        #region Public Properties
        public IZip Zip { get; }
        public ISerializationAdapter SerializationAdapter { get; }
        public bool IsSuccess => HttpResponseMessage.IsSuccessStatusCode;
        public int StatusCode => (int)HttpResponseMessage.StatusCode;
        public IRestHeadersCollection Headers { get; }
        public object UnderlyingResponse => HttpResponseMessage;
        public ITracer Tracer { get; }
        public HttpResponseMessage HttpResponseMessage { get; }
        #endregion

        #region Constructor
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
            HttpResponseMessage = httpResponseMessage ?? throw new ArgumentNullException(nameof(httpResponseMessage));
            Tracer = tracer;
            Headers = new RestResponseHeadersCollection(httpResponseMessage.Headers);
        }
        #endregion

        #region Implementation
        public async Task<RestResponse<TReturn>> ProcessRestResponseAsync<TReturn>(Uri baseUri, Uri queryString, HttpVerb httpVerb)
        {
            byte[] responseData = null;

            if (Zip != null)
            {
                //This is for cases where an unzipping utility needs to be used to unzip the content. This is actually a bug in UWP
                var gzipHeader = HttpResponseMessage.Content.Headers.ContentEncoding.FirstOrDefault(h =>
                    !string.IsNullOrEmpty(h) && h.Equals("gzip", StringComparison.OrdinalIgnoreCase));
                if (gzipHeader != null)
                {
                    var bytes = await HttpResponseMessage.Content.ReadAsByteArrayAsync();
                    responseData = Zip.Unzip(bytes);
                }
            }

            if (responseData == null)
            {
                responseData = await HttpResponseMessage.Content.ReadAsByteArrayAsync();
            }

            var bodyObject = await SerializationAdapter.DeserializeAsync<TReturn>(responseData);

            var restHeadersCollection = new RestResponseHeadersCollection(HttpResponseMessage.Headers);

            var restResponse = new RestResponse<TReturn>(
                bodyObject,
                restHeadersCollection,
                (int)HttpResponseMessage.StatusCode,
                this,
                baseUri,
                queryString,
                httpVerb
            );

            Tracer?.Trace(
                httpVerb,
                baseUri,
                queryString,
                responseData,
                TraceType.Response,
                (int)HttpResponseMessage.StatusCode,
                restHeadersCollection);

            return restResponse;
        }
        #endregion
    }
}
