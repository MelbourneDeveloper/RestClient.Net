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

#pragma warning disable CA1000 // Do not declare static members on generic types
        public static ILoggerFactory GetLoggerFactory(Action<object?> callback)
#pragma warning restore CA1000 // Do not declare static members on generic types
        {
            var callbackLogger = new CallbackLogger<Client>(callback);

            var callbackLoggerFactory = new LoggerFactory();
#pragma warning disable CA2000 // Dispose objects before losing scope
            var provider = new LoggerProvider(callbackLogger);
#pragma warning restore CA2000 // Dispose objects before losing scope
            callbackLoggerFactory.AddProvider(provider);

            return callbackLoggerFactory;
        }
    }
}

#endif