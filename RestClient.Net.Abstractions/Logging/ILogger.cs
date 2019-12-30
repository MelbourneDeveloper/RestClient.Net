using System;

namespace RestClientDotNet.Abstractions.Logging
{
    public interface ILogger
    {
        void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter);
    }


#pragma warning disable CA1815 // Override equals and operator equals on value types
}
