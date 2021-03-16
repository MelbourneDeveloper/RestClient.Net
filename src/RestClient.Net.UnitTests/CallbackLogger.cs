#if !NET45

using Microsoft.Extensions.Logging;
using System;

namespace RestClient.Net.UnitTests
{
    public class CallbackLogger<T> : ILogger<T>
    {
        private readonly Action<object?> callback;

        public CallbackLogger(Action<object?> callback) => this.callback = callback;

        public IDisposable BeginScope<TState>(TState state) => throw new NotImplementedException();
        public bool IsEnabled(LogLevel logLevel) => true;
        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
            => callback(state);
    }
}

#endif