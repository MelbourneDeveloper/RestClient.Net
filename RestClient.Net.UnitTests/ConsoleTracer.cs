
#if NET45
using RestClientDotNet.Abstractions.Logging;
#else
using Microsoft.Extensions.Logging;
#endif

using RestClientDotNet.Abstractions;
using System;
using System.Text;

namespace RestClientDotNet.UnitTests
{
    public class ConsoleTracer : ILogger
    {
        public IDisposable BeginScope<TState>(TState state)
        {
            throw new NotImplementedException();
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            throw new NotImplementedException();
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            var trace = (RestTrace)(object)state;

            Console.WriteLine($"{trace.TraceType} {trace.RequestUri}\r\n{Encoding.UTF8.GetString(trace.BodyData)}\r\nStatus Code: {trace.HttpStatusCode}");

            if (trace.RestHeadersCollection == null) return;

            foreach (var kvp in trace.RestHeadersCollection)
            {
                Console.WriteLine($"Header: {kvp.Key} {string.Join(", ", kvp.Value)}");
            }

            Console.WriteLine();
        }

    }
}
