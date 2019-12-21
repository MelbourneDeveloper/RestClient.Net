using System;
using System.Net;

namespace RestClientDotNet
{
    public interface ITracer
    {
        void Trace(HttpVerb httpVerb, Uri baseUri, Uri queryString, byte[] body, TraceType traceType, HttpStatusCode? httpStatusCode, IRestHeadersCollection restHeadersCollection);
    }
}
