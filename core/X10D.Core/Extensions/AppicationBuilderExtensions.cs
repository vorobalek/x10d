﻿using ExtCore.WebApplication.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using X10D.Core.Middleware;
using X10D.Infrastructure;

namespace X10D.Core.Extensions
{
    internal static class AppicationBuilderExtensions
    {
        internal static IApplicationBuilder UseKernelServices(this IApplicationBuilder application)
        {
            return application
                .UseRequestTimestamp()
                .UseKernelProtection()
                .UseKernelMvc()
                .TryUseExtCore();
        }

        private static IApplicationBuilder UseKernelProtection(this IApplicationBuilder application)
        {
            return application
                .UseMiddleware<KernelProtectionMiddleware>();
        }

        private static IApplicationBuilder UseRequestTimestamp(this IApplicationBuilder application)
        {
            return application
                .UseMiddleware<RequestTimestampMiddleware>();
        }

        private static IApplicationBuilder UseKernelMvc(this IApplicationBuilder application)
        {
            return application
                .UseRouting()
                .UseAuthentication()
                .UseAuthorization()
                .UseStatusCodePagesWithReExecute("/error/{0}")
                .UseExceptionHandler("/fail")
                .UseEndpoints(endpoints =>
                {
                    endpoints.MapAreaControllerRoute(
                        name: "kernel",
                        areaName: "Kernel",
                        pattern: "kernel/{controller=home}/{action=index}",
                        defaults: new { area = "kernel", controller = "home", action="index" },
                        constraints: new { area = "kernel" });
                });
        }

        private static IApplicationBuilder TryUseExtCore(this IApplicationBuilder application)
        {
            var kernel = application.ApplicationServices.GetService<IKernelFacade>();
            if (kernel?.State == ServiceState.InProgress)
            {
                application.UseExtCore();
            }
            return application;
        }

        internal static IApplicationBuilder UseDebugger(this IApplicationBuilder application)
        {
            return application
                  .UseMiddleware<DebuggerMiddleware>();
        }
    }
}
