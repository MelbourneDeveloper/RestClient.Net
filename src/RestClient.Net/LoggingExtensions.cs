
#if NET45
using RestClient.Net.Abstractions.Logging;
#else
using Microsoft.Extensions.Logging;
#endif

using RestClient.Net.Abstractions;
using System;


namespace RestClient.Net
{
    internal static class LoggingExtensions
    {
        private static readonly Func<Trace, Exception, string> func = (trace, exception) => exception != null ? exception.ToString() : trace.Message ?? string.Empty;

        public static void LogInformation(this ILogger logger, Trace trace)
        {
            if (trace == null) throw new ArgumentNullException(nameof(trace));

            logger.Log(LogLevel.Information, new EventId((int)trace.RestEvent, trace.RestEvent.ToString()), trace, null, func);
        }

        public static void LogTrace(this ILogger logger, Trace trace)
        {
            if (trace == null) throw new ArgumentNullException(nameof(trace));

            logger.Log(LogLevel.Trace, new EventId((int)trace.RestEvent, trace.RestEvent.ToString()), trace, null, func);
        }

        public static void LogException(this ILogger logger, Trace trace, Exception exception)
        {
            if (trace == null) throw new ArgumentNullException(nameof(trace));

            logger.Log(LogLevel.Error, new EventId((int)trace.RestEvent, trace.RestEvent.ToString()), trace, exception, func);
        }
    }
}