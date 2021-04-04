using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Urls;

namespace Http.Server
{
    public static class ServerExtensions
    {
        public static Task ServeAsync(this HttpServer listener, Func<HttpListenerContext, Task> func)
        {
            return Task.Run<Task>(() =>
           {
               var context = listener.listener.GetContext();
               return func(context);
           });
        }

        public static AbsoluteUrl GetLocalhostAddress()
        {
            var listener = new TcpListener(IPAddress.Loopback, 0);
            listener.Start();
            var port = ((IPEndPoint)listener.LocalEndpoint).Port;
            listener.Stop();

            return $"http://localhost:{port}/".ToAbsoluteUrl();
        }
    }
}


