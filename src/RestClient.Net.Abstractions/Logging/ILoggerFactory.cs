namespace RestClient.Net.Abstractions.Logging
{
    public interface ILoggerFactory
    {
        ILogger<T> CreateLogger<T>();
    }
}
