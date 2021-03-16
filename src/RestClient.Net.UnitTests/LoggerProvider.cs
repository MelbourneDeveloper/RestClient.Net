#if !NET45

#pragma warning disable CA1063 // Implement IDisposable Correctly
#pragma warning disable CA1816 // Dispose methods should call SuppressFinalize

using Microsoft.Extensions.Logging;

namespace RestClient.Net.UnitTests
{
    public class LoggerProvider : ILoggerProvider
    {
        private readonly ILogger logger;

        public LoggerProvider(ILogger logger) => this.logger = logger;

        public ILogger CreateLogger(string categoryName) => logger;
        public void Dispose() { }
    }
}
#endif