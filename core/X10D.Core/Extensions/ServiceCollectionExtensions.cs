using ExtCore.WebApplication.Extensions;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using System.Globalization;
using System.IO;
using System.Linq;
using X10D.Core.Data;
using X10D.Core.Services;
using X10D.Core.StartupFilters;
using X10D.Infrastructure;

namespace X10D.Core.Extensions
{
    internal static class ServiceCollectionExtensions
    {
        internal static IServiceCollection AddKernelServices(this IServiceCollection services)
        {
            var configuration = services.BuildServiceProvider().GetService<IConfiguration>();

            services
                .AddLogging(configure =>
                {
                    configure
                        .AddConsole(c =>
                        {
                            c.TimestampFormat = "[dd.MM.yyyy HH:mm:ss.ffffff]\r\n      ";
                        })
                        .AddDebug();
                })

                .AddEntityFrameworkSqlite()
                .AddDbContext<StoredCacheContext>(o => o.UseSqlite("Filename=cache.db"), ServiceLifetime.Singleton)
                .AddSingleton<IStartupFilter, KernelStartupFilter>()
                .AddHttpContextAccessor()
                .AddAuthentication(options =>
                {
                    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                })
                .AddCookie(options =>
                {
                    options.Cookie.SecurePolicy = CookieSecurePolicy.None;
                    options.Cookie.Name = "x10d.core";
                    options.LoginPath = "/account/login";
                    options.AccessDeniedPath = "/denied";
                });

            services
                .AddAuthorization()
                .AddControllersWithViews();

            services
                .TryAddExtCore();
            return services;
        }

        private static IServiceCollection AddPrototypedServices(this IServiceCollection services)
        {
            var tempServices = new ServiceCollection()
                .Add(services)
                .AddSingleton<IActivator, Activator>();

            var activator = tempServices
                .BuildServiceProvider()
                .GetService<IActivator>();

            (activator as IServicePrototype).Flush().Wait();
            (activator as IServicePrototype).Prepare().Wait();

            var classTypes = activator.GetTypes(type =>
            {
                return new[]
                {
                    type.GetInterfaces().Contains(typeof(IServicePrototype)),
                    !type.IsAbstract,
                    !type.IsInterface,
                }.All(flag => flag);
            });
            
            foreach (var classType in classTypes)
            {
                var interfaceType = classType.GetInterfaces().OrderBy(type => type.GetInterfaces().Length).Last();
                tempServices.Add(new ServiceDescriptor(interfaceType, classType, ServiceLifetime.Singleton));
            }
            
            var tempResolver = tempServices.BuildServiceProvider();
            
            foreach (var classType in classTypes)
            {
                var interfaceType = classType.GetInterfaces().OrderBy(type => type.GetInterfaces().Length).Last();
                var prototype = tempResolver.GetService(interfaceType) as IServicePrototype;
                services.Add(new ServiceDescriptor(interfaceType, classType, prototype.ServiceLifetime));
                
                // adding all services as prototypes
                if (interfaceType != typeof(IServicePrototype))
                {
                    services.Add(new ServiceDescriptor(typeof(IServicePrototype), serviceProvider => serviceProvider.GetService(interfaceType), prototype.ServiceLifetime));
                }
            }
            
            return services;
        }

        private static IServiceCollection TryAddExtCore(this IServiceCollection services)
        {
            var tempRresolver = new ServiceCollection().Add(services).AddPrototypedServices().BuildServiceProvider();
            var kernel = tempRresolver.GetService<IKernel>();
            if (kernel != null)
            {
                // add facades
                services.AddSingleton(serviceProvider => (IKernelFacade)serviceProvider.GetService<IKernel>());
                services.AddSingleton(serviceProvider => (IDebuggerFacade)serviceProvider.GetService<IDebugger>());
                services.AddScoped(serviceProvider => (IDebuggerSessionFacade)serviceProvider.GetService<IDebuggerSession>());

                var cache = tempRresolver.GetService<IStoredCache>();
                services.AddExtCore(
                    Path.Combine(Directory.GetCurrentDirectory(), cache["modules.path"] ?? "modules"),
                    (cache["modules.recursive"] ?? "true").ToLower(new CultureInfo("en-US")) == "true");
                services.AddPrototypedServices();
                return services;
            }
            else
            {
                services.AddPrototypedServices();
            }
            return services;
        }
    }
}
