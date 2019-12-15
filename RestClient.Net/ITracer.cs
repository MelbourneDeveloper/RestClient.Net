using System;

namespace RestClientDotNet
{
    public interface ITracer
    {
        void Trace(HttpVerb httpVerb, Uri baseUri, Uri queryString, byte[] body, TraceType traceType);
    }
}
