#pragma warning disable CA1063 // Implement IDisposable Correctly
#pragma warning disable CA1816 // Dispose methods should call SuppressFinalize

namespace Microsoft.Extensions.Logging.Abstractions
{
    public class NullLoggerFactory : ILoggerFactory
    {
        public static NullLoggerFactory Instance { get; } = new();

        public ILogger CreateLogger(string name) => NullLogger.Instance;
        public void Dispose() { }
    }
}


