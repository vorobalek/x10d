using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using X10D.Core.Extensions;

namespace X10D.Core
{
    internal sealed class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddKernelServices();
        }

        public void Configure(IApplicationBuilder application, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                application.UseDebugger();
                application.UseDeveloperExceptionPage();
            }

            application
                .UseKernelServices();
        }
    }
}
