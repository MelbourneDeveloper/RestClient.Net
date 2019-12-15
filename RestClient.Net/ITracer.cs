using System;

namespace RestClientDotNet
{
    public interface ITracer
    {
        void Trace(HttpVerb httpVerb, Uri baseUri, string queryString, byte[] body, TraceType traceType);
    }
}
