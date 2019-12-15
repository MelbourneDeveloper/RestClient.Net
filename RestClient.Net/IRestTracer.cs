using System;

namespace RestClientDotNet
{
    public interface IRestTracer
    {
        void Trace(HttpVerb httpVerb, Uri baseUri, string queryString, byte[] body, TraceType traceType);
    }
}
