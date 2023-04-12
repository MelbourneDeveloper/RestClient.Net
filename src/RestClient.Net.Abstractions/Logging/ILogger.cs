#pragma warning disable IDE0130 // Namespace does not match folder structure

using System;

namespace Microsoft.Extensions.Logging
{
    public interface ILogger
    {
        IDisposable BeginScope(string messageFormat, params object[] args);
        void LogError(EventId eventId, Exception exception, string message, params object[] args);
        void LogError(Exception exception, string message, params object[] args);
        void LogInformation(string message, params object[] args);
        void LogWarning(string message, params object[] args);
        void LogDebug(string message, params object[] args);
        void LogTrace(string message, params object[] args);
    }

    public interface ILogger<T> : ILogger
    {

    }
}