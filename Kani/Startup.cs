using Kani.Services;
using Microsoft.AspNetCore.Components.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Kani
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddSingleton<IAssemblyRegistry, AssemlbyRegistry>()
                .AddSingleton<IDocumentHistoryService, DocumentHistoryService>()
                .AddSingleton<IDocumentService, DocumentService>();
        }

        public void Configure(IComponentsApplicationBuilder app)
        {
            app.AddComponent<App>("app");
        }
    }
}
