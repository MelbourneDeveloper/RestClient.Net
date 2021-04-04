using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Urls;

#pragma warning disable CA1835 // Prefer the 'Memory'-based overloads for 'ReadAsync' and 'WriteAsync'

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

        public static Task WriteContentAndCloseAsync(this HttpListenerContext context, string responseContent)
            => responseContent == null
                ? throw new ArgumentNullException(nameof(responseContent))
                : WriteContentAndCloseAsync(context, Encoding.UTF8.GetBytes(responseContent));


        public static async Task WriteContentAndCloseAsync(this HttpListenerContext context, byte[] responseContent)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            if (responseContent == null) throw new ArgumentNullException(nameof(responseContent));

            await context.Response.OutputStream.WriteAsync(responseContent, 0, responseContent.Length).ConfigureAwait(false);
            context.Response.OutputStream.Close();
        }

        public static HttpServer GetSingleRequestServer(this AbsoluteUrl url, string responseContent)
        {
            var server = new HttpServer(url);

            var task = server.ServeAsync(async (context) =>
            {
                await context.WriteContentAndCloseAsync(responseContent).ConfigureAwait(false);
            });

            return server;
        }
    }
}


