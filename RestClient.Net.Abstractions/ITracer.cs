using System;

namespace RestClientDotNet.Abstractions
{
    public class RestTrace
    {
        public HttpRequestMethod HttpRequestMethod { get; }
        public Uri RequestUri { get; }
        public byte[] BodyData { get; }
        public RestEvent RestEvent { get; }
        public int? HttpStatusCode { get; }
        public IRestHeadersCollection RestHeadersCollection { get; }

        public RestTrace(
            HttpRequestMethod httpRequestMethod,
            Uri requestUri,
            byte[] bodyData,
            RestEvent traceType,
            int? httpStatusCode,
            IRestHeadersCollection restHeadersCollection)
        {
            HttpRequestMethod = httpRequestMethod;
            RequestUri = requestUri;
            BodyData = bodyData;
            RestEvent = traceType;
            HttpStatusCode = httpStatusCode;
            RestHeadersCollection = restHeadersCollection;
        }
    }
}
