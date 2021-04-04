using System;
using System.Net;
using Urls;

#pragma warning disable CA1816 // Dispose methods should call SuppressFinalize
#pragma warning disable CA1063 // Implement IDisposable Correctly

//Thanks! https://gist.github.com/yetanotherchris/fb50071bced8bf0849ecd2cbbc3e9dce

namespace Http.Server
{
    public class HttpServer : IDisposable
    {
        internal readonly HttpListener listener = new();

        public AbsoluteUrl AbsoluteUrl { get; }

        public HttpServer(AbsoluteUrl url)
        {
            AbsoluteUrl = url ?? throw new ArgumentNullException(nameof(url));

            listener.Prefixes.Add(url.ToString() + "/");
            listener.Start();
        }

        public void Dispose()
        {
            listener.Stop();
            ((IDisposable)listener).Dispose();
        }
    }
}


