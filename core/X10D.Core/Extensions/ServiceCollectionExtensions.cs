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
using System.Reflection;
using X10D.Core.Data;
using X10D.Core.Services;
using X10D.Core.StartupFilters;
using X10D.Infrastructure;
using X10D.Infrastructure.Attributes;

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
                    var dt = DateTime.Now;
                    var dtString = $"{dt.Year:0000}{dt.Month:00}{dt.Day:00}/{dt.Hour:00}{dt.Minute:00}{dt.Second:00}";
                    var outputTemplate = "{Timestamp:o} {RequestId,13} [{Level:u4}] {Message} ({EventId:x8}){NewLine}{Exception}";
                    configure
                        .AddConsole(c =>
                        {
                            c.TimestampFormat = "[dd.MM.yyyy HH:mm:ss.ffffff]\r\n      ";
                        })
                        .AddDebug()
                        .AddFile($"logs/{dtString}/VERB.txt", minimumLevel: LogLevel.Trace, outputTemplate: outputTemplate)
                        .AddFile($"logs/{dtString}/DBUG.txt", minimumLevel: LogLevel.Debug, outputTemplate: outputTemplate)
                        .AddFile($"logs/{dtString}/INFO.txt", minimumLevel: LogLevel.Information, outputTemplate: outputTemplate)
                        .AddFile($"logs/{dtString}/WARN.txt", minimumLevel: LogLevel.Warning, outputTemplate: outputTemplate)
                        .AddFile($"logs/{dtString}/EROR.txt", minimumLevel: LogLevel.Error, outputTemplate: outputTemplate)
                        .AddFile($"logs/{dtString}/FATL.txt", minimumLevel: LogLevel.Critical, outputTemplate: outputTemplate)

                        .AddFile($"wwwroot/logs/{dtString}/VERB.json", minimumLevel: LogLevel.Trace, outputTemplate: outputTemplate, isJson: true)
                        .AddFile($"wwwroot/logs/{dtString}/DBUG.json", minimumLevel: LogLevel.Debug, outputTemplate: outputTemplate, isJson: true)
                        .AddFile($"wwwroot/logs/{dtString}/INFO.json", minimumLevel: LogLevel.Information, outputTemplate: outputTemplate, isJson: true)
                        .AddFile($"wwwroot/logs/{dtString}/WARN.json", minimumLevel: LogLevel.Warning, outputTemplate: outputTemplate, isJson: true)
                        .AddFile($"wwwroot/logs/{dtString}/EROR.json", minimumLevel: LogLevel.Error, outputTemplate: outputTemplate, isJson: true)
                        .AddFile($"wwwroot/logs/{dtString}/FATL.json", minimumLevel: LogLevel.Critical, outputTemplate: outputTemplate, isJson: true);
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
                .AddAuthorization()
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

            var logger = tempResolver.GetService<ILogger<Startup>>();

            foreach (var serviceType in serviceTypes)
            {
                Type interfaceType = default;
                Func<IServiceProvider, object> customFactory = default;
                ServiceLifetime serviceLifetime = default;

                if (!serviceType.IsGenericType)
                {
                    var serviceEmpty = activator.CreateEmpty<IServicePrototype>(serviceType);
                    interfaceType = serviceEmpty.InterfaceType;
                    customFactory = serviceEmpty.CustomFactory;
                    serviceLifetime = serviceEmpty.ServiceLifetime;
                }
                else
                {
                    if (serviceType.GetCustomAttribute<ServiceAttribute>() is ServiceAttribute attribute)
                    {
                        interfaceType = attribute.InterfaceType;
                        customFactory = attribute.CustomFactory;
                        serviceLifetime = attribute.ServiceLifetime;
                    }
                }
                services.RegisterService(serviceType, interfaceType, customFactory, serviceLifetime, logger, serviceType.IsGenericType);
            }

            return services;
        }
        private static void RegisterService(this IServiceCollection services, Type serviceType, Type interfaceType, Func<IServiceProvider, object> customFactory,  ServiceLifetime serviceLifetime, ILogger logger, bool isGeneric)
        {
            if (serviceType != default)
            {
                if (interfaceType != default)
                {
                    if (customFactory != default)
                    {
                        services.Add(new ServiceDescriptor(interfaceType, customFactory, serviceLifetime));
                        logger.LogWarning($"The \"{interfaceType.GetFullName()}\" service is registered as {serviceLifetime} with custom factory.");
                    }
                    else
                    {
                        services.Add(new ServiceDescriptor(interfaceType, serviceType, serviceLifetime));
                        logger.LogWarning($"The \"{interfaceType.GetFullName()}\" service is registered as {serviceLifetime} with {serviceType.GetFullName()} implementation.");
                    }
                }
                if (!isGeneric)
                {
                    // adding all services as prototypes
                    if (interfaceType != default &&
                        interfaceType != typeof(IServicePrototype))
                    {
                        services.Add(new ServiceDescriptor(typeof(IServicePrototype), serviceProvider => serviceProvider.GetService(interfaceType), serviceLifetime));
                        logger.LogWarning($"The \"{typeof(IServicePrototype).GetFullName()}\" service is registered as {serviceLifetime} with standart factory (=> {interfaceType.GetFullName()}).");
                    }
                    else
                    {
                        services.Add(new ServiceDescriptor(typeof(IServicePrototype), serviceType, serviceLifetime));
                        logger.LogWarning($"The \"{typeof(IServicePrototype).GetFullName()}\" service is registered as {serviceLifetime} with {serviceType.GetFullName()} implementation.");
                    }
                }
            }
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
