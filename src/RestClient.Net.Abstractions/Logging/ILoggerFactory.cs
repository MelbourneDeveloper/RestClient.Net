#pragma warning disable IDE0130 // Namespace does not match folder structure

using System;

namespace Microsoft.Extensions.Logging
{
    public interface ILoggerFactory : IDisposable
    {
        ILogger CreateLogger(string name);
    }
}

