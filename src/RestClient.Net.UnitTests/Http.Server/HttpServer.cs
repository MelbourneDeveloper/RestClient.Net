using System;
using System.Net;
using System.Threading.Tasks;
using Urls;

#pragma warning disable CA1816 // Dispose methods should call SuppressFinalize
#pragma warning disable CA1063 // Implement IDisposable Correctly

//Thanks! https://gist.github.com/yetanotherchris/fb50071bced8bf0849ecd2cbbc3e9dce

namespace Http.Server
{
    public class HttpServer : IDisposable
    {
        internal readonly HttpListener listener = new();
        internal readonly Func<HttpListenerContext, Task> func;
        public AbsoluteUrl AbsoluteUrl { get; }

        public HttpServer(AbsoluteUrl url, Func<HttpListenerContext, Task> func)
        {
            AbsoluteUrl = url ?? throw new ArgumentNullException(nameof(url));
            this.func = func ?? throw new ArgumentNullException(nameof(func));

            listener.Prefixes.Add(url.ToString() + "/");
            listener.Start();

            _ = Task.Run(() =>
               func(listener.GetContext()));
        }

        public void Dispose()
        {
            listener.Stop();
            ((IDisposable)listener).Dispose();
        }
    }
}


