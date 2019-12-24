using System;

namespace RestClientDotNet.Abstractions
{
    public interface ITracer
    {
        void Trace(HttpVerb httpVerb, Uri baseUri, Uri resource, byte[] body, TraceType traceType, int? httpStatusCode, IRestHeaders restHeadersCollection);
    }
}
