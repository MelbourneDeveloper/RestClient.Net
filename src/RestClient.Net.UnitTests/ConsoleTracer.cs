
#if NET45
using RestClient.Net.Abstractions.Logging;
#else
using Microsoft.Extensions.Logging;
#endif

using RestClient.Net.Abstractions;
using System;
using System.Text;

namespace RestClient.Net.UnitTests
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
            var trace = (Trace)(object)state;

            if (trace != null)
            {
                Console.WriteLine($"{trace?.RestEvent} {trace?.RequestUri}\r\nStatus Code: {trace?.HttpStatusCode}");

                if (trace.BodyData != null)
                {
                    Console.WriteLine($"Body: {Encoding.UTF8.GetString(trace?.BodyData)}\r\n");
                }

                if (trace.HeadersCollection == null) return;

                foreach (var kvp in trace?.HeadersCollection)
                {
                    Console.WriteLine($"Header: {kvp.Key} {string.Join(", ", kvp.Value)}");
                }
            }

            Console.WriteLine($"Exception: {exception}");

            Console.WriteLine();
        }

    }
}
