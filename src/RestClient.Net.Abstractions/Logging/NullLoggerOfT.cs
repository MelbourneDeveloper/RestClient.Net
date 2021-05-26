using System;

namespace Microsoft.Extensions.Logging.Abstractions
{

    internal class NullLogger<T> : ILogger<T>
    {
        private readonly ILogger _logger;

        public static NullLogger<T> Instance = new(NullLoggerFactory.Instance);

        public NullLogger(ILoggerFactory factory)
        {
            if (factory == null)
            {
                throw new ArgumentNullException(nameof(factory));
            }

            _logger = factory.CreateLogger(typeof(T).Name);
        }

        public IDisposable BeginScope(string messageFormat, params object[] args) => _logger.BeginScope(messageFormat, args);
        public void LogDebug(string message, params object[] args) => _logger.LogDebug(message, args);
        public void LogError(EventId eventId, Exception exception, string message, params object[] args) => _logger.LogError(eventId, exception, message, args);
        public void LogError(Exception exception, string message, params object[] args) => _logger.LogError(exception, message, args);
        public void LogInformation(string message, params object[] args) => _logger.LogInformation(message, args);
        public void LogTrace(string message, params object[] args) { }
        public void LogWarning(string message, params object[] args) => _logger.LogWarning(message, args);
    }
}
