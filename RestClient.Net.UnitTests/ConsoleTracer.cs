
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
    public class ConsoleLogger : ILogger
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

            if (trace != null)
            {
                Console.WriteLine($"{trace?.RestEvent} {trace?.RequestUri}\r\nStatus Code: {trace?.HttpStatusCode}");

                if(trace.BodyData!=null)
                {
                    Console.WriteLine($"Body: {Encoding.UTF8.GetString(trace?.BodyData)}\r\n");
                }

                if (trace.RestHeadersCollection == null) return;

                foreach (var kvp in trace?.RestHeadersCollection)
                {
                    Console.WriteLine($"Header: {kvp.Key} {string.Join(", ", kvp.Value)}");
                }
            }

            Console.WriteLine($"Exception: {exception}");

            Console.WriteLine();
        }

    }
}
