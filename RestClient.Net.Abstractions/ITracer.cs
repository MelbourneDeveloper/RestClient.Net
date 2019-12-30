using System;

namespace RestClientDotNet.Abstractions
{
    public class RestTrace
    {
        public HttpVerb HttpVerb { get; }
        public Uri RequestUri { get; }
        public byte[] BodyData { get; }
        public TraceType TraceType { get; }
        public int? HttpStatusCode { get; }
        public IRestHeadersCollection RestHeadersCollection { get; }

        public RestTrace(
            HttpVerb httpVerb,
            Uri requestUri,
            byte[] bodyData,
            TraceType traceType,
            int? httpStatusCode,
            IRestHeadersCollection restHeadersCollection)
        {
            HttpVerb = httpVerb;
            RequestUri = requestUri;
            BodyData = bodyData;
            TraceType = traceType;
            HttpStatusCode = httpStatusCode;
            RestHeadersCollection = restHeadersCollection;
        }
    }
}
