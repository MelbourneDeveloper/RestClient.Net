using RestClientDotNet.Abstractions;
using System;
using System.Text;

namespace RestClientDotNet.UnitTests
{
    public class ConsoleTracer : ITracer
    {
        public void Trace(HttpVerb httpVerb, Uri baseUri, Uri resource, byte[] body, TraceType traceType, int? httpStatusCode, IRestHeadersCollection restHeadersCollection)
        {
            Console.WriteLine($"{traceType} {baseUri} {resource}\r\n{Encoding.UTF8.GetString(body)}\r\nStatus Code: {httpStatusCode}");

            foreach (var header in restHeadersCollection)
            {
                Console.WriteLine($"{traceType} Header: {header.Key} {string.Join(", ", header.Value)}");
            }
        }
    }
}
