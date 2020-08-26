namespace RestClient.Net.Abstractions.Logging
{
    public interface ILoggerFactory
    {
        ILogger CreateLogger<T>();
    }
}
