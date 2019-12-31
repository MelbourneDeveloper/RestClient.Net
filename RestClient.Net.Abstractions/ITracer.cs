using System;

namespace RestClientDotNet.Abstractions
{
    public class RestTrace
    {
        public HttpRequestMethod HttpRequestMethod { get; }
        public Uri RequestUri { get; }
        public byte[] BodyData { get; }
        public TraceType TraceType { get; }
        public int? HttpStatusCode { get; }
        public IRestHeadersCollection RestHeadersCollection { get; }

        public RestTrace(
            HttpRequestMethod httpRequestMethod,
            Uri requestUri,
            byte[] bodyData,
            TraceType traceType,
            int? httpStatusCode,
            IRestHeadersCollection restHeadersCollection)
        {
            HttpRequestMethod = httpRequestMethod;
            RequestUri = requestUri;
            BodyData = bodyData;
            TraceType = traceType;
            HttpStatusCode = httpStatusCode;
            RestHeadersCollection = restHeadersCollection;
        }
    }
}
