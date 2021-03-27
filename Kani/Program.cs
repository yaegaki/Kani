using System;
using System.Net.Http;
using System.Threading.Tasks;
using Kani.Services;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace Kani
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            await CreateHostBuilder(args).Build().RunAsync();
        }

        public static WebAssemblyHostBuilder CreateHostBuilder(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("app");
            builder.Services
                .AddSingleton<IAssemblyRegistry, AssemlbyRegistry>()
                .AddSingleton<IDocumentHistoryService, DocumentHistoryService>()
                .AddSingleton<IDocumentService, DocumentService>()
                .AddSingleton(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

            return builder;
        }
    }
}
