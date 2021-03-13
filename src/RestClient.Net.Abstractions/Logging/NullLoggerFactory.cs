namespace Microsoft.Extensions.Logging.Abstractions
{
    public class NullLoggerFactory : ILoggerFactory
    {
        public static NullLoggerFactory Instance { get; } = new();

        public ILogger CreateLogger(string name) => NullLogger.Instance;
    }
}


