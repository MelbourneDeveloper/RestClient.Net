using System;
using System.Net;
using System.Text;

namespace RestClientDotNet.UnitTests
{
    public class ConsoleTracer : ITracer
    {
        public void Trace(HttpVerb httpVerb, Uri baseUri, Uri queryString, byte[] body, TraceType traceType, HttpStatusCode? httpStatusCode, IRestHeadersCollection restHeadersCollection)
        {
            Console.WriteLine($"{traceType} {baseUri} {queryString}\r\n{Encoding.UTF8.GetString(body)}\r\nStatus Code: {httpStatusCode}");

            foreach (var header in restHeadersCollection)
            {
                Console.WriteLine($"Header: {header.Key} {string.Join(", ", header.Value)}");
            }
        }
    }
}
