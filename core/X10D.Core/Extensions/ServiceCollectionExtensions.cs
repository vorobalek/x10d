using ExtCore.WebApplication.Extensions;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using System;
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
                .AddDbContext<StoredCacheContext>(o =>
                {
                    o.UseSqlite("Filename=cache.db");
                    o.UseInternalServiceProvider(services.BuildServiceProvider());
                }, ServiceLifetime.Singleton)
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
                .AddAuthorization(opts =>
                {
                    opts.AddPolicy(Constants.TokenSidePolicy, (builder) =>
                    {
                        builder.RequireClaim(Constants.TokenSidePolicy);
                    });
                })
                .AddControllersWithViews();

            services
                .AddSingleton<IAssemblyProvider, AssemblyProvider>()
                .TryAddExtCore();
            return services;
        }

        private static IServiceCollection AddPrototypedServices(this IServiceCollection services)
        {
            var tempServices = new ServiceCollection()
                .Add(services)
                .AddTransient<IActivator, Services.Activator>()
                .AddSingleton<IActivatorCache, ActivatorCache>();

            var tempResolver = tempServices
                .BuildServiceProvider();

            var activatorCache = tempResolver
                .GetService<IActivatorCache>();

            var activator = tempResolver
                .GetService<IActivator>();

            activatorCache.Flush().Wait();
            activatorCache.Prepare().Wait();

            var serviceTypes = activator.GetTypes(type =>
            {
                return new[]
                {
                    type.GetInterfaces().Contains(typeof(IServicePrototype)),
                    !type.IsAbstract,
                    !type.IsInterface,
                }.All(flag => flag);
            });

            foreach (var serviceType in serviceTypes)
            {
                var serviceEmpty = activator.CreateEmpty<IServicePrototype>(serviceType);
                if (serviceEmpty.InterfaceType != null)
                {
                    if (serviceEmpty.CustomFactory != null)
                    {
                        services.Add(new ServiceDescriptor(serviceEmpty.InterfaceType, serviceEmpty.CustomFactory, serviceEmpty.ServiceLifetime));
                    }
                    else
                    {
                        services.Add(new ServiceDescriptor(serviceEmpty.InterfaceType, serviceType, serviceEmpty.ServiceLifetime));
                    }
                }

                // adding all services as prototypes
                if (serviceEmpty.InterfaceType != null && 
                    serviceEmpty.InterfaceType != typeof(IServicePrototype))
                {
                    services.Add(new ServiceDescriptor(typeof(IServicePrototype), serviceProvider => serviceProvider.GetService(serviceEmpty.InterfaceType), serviceEmpty.ServiceLifetime));
                }
                else
                {
                    services.Add(new ServiceDescriptor(typeof(IServicePrototype), serviceType, serviceEmpty.ServiceLifetime));
                }
            }

            return services;
        }

        private static IServiceCollection TryAddExtCore(this IServiceCollection services)
        {
            var tempServices = new ServiceCollection().Add(services).AddPrototypedServices();
            var tempRresolver = tempServices.BuildServiceProvider();
            var kernel = tempRresolver.GetService<IKernel>();
            if (kernel != null)
            {
                // add facades
                services.AddSingleton(serviceProvider => (IKernelFacade)serviceProvider.GetService<IKernel>());
                services.AddSingleton(serviceProvider => (IDebuggerFacade)serviceProvider.GetService<IDebugger>());
                services.AddScoped(serviceProvider => (IDebuggerSessionFacade)serviceProvider.GetService<IDebuggerSession>());

                var cache = tempRresolver.GetService<IStoredCache>();
                var assemblyProvider = tempRresolver.GetService<IAssemblyProvider>();
                services
                    .AddExtCore(
                    Path.Combine(Directory.GetCurrentDirectory(), cache["modules.path"] ?? "modules"),
                    (cache["modules.recursive"] ?? "true").ToLower(new CultureInfo("en-US")) == "true",
                    assemblyProvider);
                services
                    .AddPrototypedServices()
                    .AddSingleton(assemblyProvider);
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
