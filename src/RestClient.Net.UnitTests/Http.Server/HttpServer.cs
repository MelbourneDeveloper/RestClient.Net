using System;
using System.Net;

#pragma warning disable CA1054 // URI-like parameters should not be strings
#pragma warning disable CA1816 // Dispose methods should call SuppressFinalize
#pragma warning disable CA1063 // Implement IDisposable Correctly

//Thanks! https://gist.github.com/yetanotherchris/fb50071bced8bf0849ecd2cbbc3e9dce

namespace Http.Server
{
    public class HttpServer : IDisposable
    {
        internal readonly HttpListener listener = new();

        public HttpServer(string url)
        {
            listener.Prefixes.Add(url);
            listener.Start();
        }

        public void Dispose()
        {
            listener.Stop();
            ((IDisposable)listener).Dispose();
        }
    }
}


