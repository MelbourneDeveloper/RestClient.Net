
#pragma warning disable 

using System;

namespace RestClient.Net.Abstractions.Logging
{
    public class NullLoggerFactory : ILoggerFactory
    {
        public static NullLoggerFactory Instance { get; } = new NullLoggerFactory();

        public ILogger CreateLogger(string categoryName) => new NullLogger();
    }

    public class DummyDisposable : IDisposable
    {
        public void Dispose()
        {
        }
    }

    public class NullLogger : ILogger
    {
        public static ILogger Instance = new NullLogger();

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
        }
    }
}
#pragma warning restore 

