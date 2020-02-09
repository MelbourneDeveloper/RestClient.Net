
#if NET45
using RestClient.Net.Abstractions.Logging;
#else
using Microsoft.Extensions.Logging;
#endif

#if NETCOREAPP3_0
using RestClient.Net.Abstractions.Extensions;
#endif

using RestClient.Net.Abstractions;
using System;


namespace RestClient.Net
{
    public static class LoggingExtensions
    {
        private static readonly Func<Trace, Exception, string> func = new Func<Trace, Exception, string>((trace, exception) => { return trace.Message; });

        public static void LogInfo(this ILogger logger, Trace trace)
        {
            if (logger == null) return;

            logger.Log(LogLevel.Information, new EventId(1), trace, null, func);
        }
    }

}

