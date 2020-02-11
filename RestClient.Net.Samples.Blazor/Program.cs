using Microsoft.AspNetCore.Blazor.Hosting;
using RestClient.Net.Samples.Blazor;
using System.Threading.Tasks;

namespace BlazorApp1
{
    public class Program
    {
        //This is for server side rendering
#if (NETCOREAPP3_1)
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
#endif

        //Client side Blazor rendering
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("app");

            await builder.Build().RunAsync();
        }
    }
}
