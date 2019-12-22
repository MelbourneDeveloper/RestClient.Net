using System;

namespace RestClientDotNet.Abstractions
{
    public interface ITracer
    {
        void Trace(HttpVerb httpVerb, Uri baseUri, Uri queryString, byte[] body, TraceType traceType, int? httpStatusCode, IRestHeadersCollection restHeadersCollection);
    }
}
