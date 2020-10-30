using System;

namespace RestClient.Net.Abstractions.Logging
{
    public interface ILogger
    {
        void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter);
    }

    public interface ILogger<T> : ILogger { }

#pragma warning disable CA1815 // Override equals and operator equals on value types
}
