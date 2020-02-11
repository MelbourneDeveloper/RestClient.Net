using Microsoft.AspNetCore.Blazor.Hosting;
using RestClient.Net.Samples.Blazor;
using System.Threading.Tasks;

namespace BlazorApp1
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("app");

            await builder.Build().RunAsync();
        }

    }
}
