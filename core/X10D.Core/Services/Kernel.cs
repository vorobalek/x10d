using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using X10D.Infrastructure;

namespace X10D.Core.Services
{
    internal sealed class Kernel : ServicePrototype, IKernel
    {
        public override ServiceLifetime ServiceLifetime => ServiceLifetime.Singleton;
        
        private IServiceProvider ServiceProvider { get; }
        
        private ILogger Logger { get; }
        
        public IList<IServicePrototype> Services { get; private set; } = new List<IServicePrototype>();

        private Guid token = Guid.NewGuid();
        
        public Kernel(IServiceProvider serviceProvider, ILogger<Kernel> logger)
        {
            ServiceProvider = serviceProvider;
            Logger = logger;

            Idle();
        }

        public void LogToken()
        {
            Logger.LogCritical(@"
------------------------------------------
                 ВНИМАНИЕ                 
------------------------------------------

" + $"     {token.ToString("N", new CultureInfo("en-US")).ToUpperInvariant()}     " + @"

------------------------------------------
    ЭТО КЛЮЧ БЕЗОПАСНОСТИ ЯДРА СИСТЕМЫ    
    ОН ИЗМЕНЯЕТСЯ ПРИ КАЖДОМ ЗАПУСКЕ ЯДРА 
------------------------------------------");
        }

        public bool ValidateToken(string candidate)
        {
            return Guid.TryParse(candidate, out var guid_candidate) && guid_candidate == token;
        }

        protected override void FlushService()
        {
            var tasks = Services.Select(service => service.Flush());
            Task.WaitAll(tasks.ToArray());

            Services = new List<IServicePrototype>();
            base.FlushService();
        }

        protected override void PrepareService()
        {
            var services = ServiceProvider.GetServices<IServicePrototype>().Where(service => !service.GetType().GetInterfaces().Contains(typeof(IKernelFacade)));
            Services = services.Select(service =>
                service
                    .OnBeforeStateChange(state =>
                    {
                        Logger.LogWarning($"Сервис {service.GetType().GetFullName()} переходит из состояния {state}");
                    }, state =>
                    {
                        Logger.LogWarning($"Сервис {service.GetType().GetFullName()} переходит в состояние {state}");
                    })
                    .OnAfterStateChange(state =>
                    {
                        Logger.LogWarning($"Сервис {service.GetType().GetFullName()} перешел из состояния {state}");
                    }, state =>
                    {
                        Logger.LogWarning($"Сервис {service.GetType().GetFullName()} перешел в состояние {state}");
                    })).ToList();


            var tasks = Services.Select(service => service.Prepare());
            Task.WaitAll(tasks.ToArray());

            base.PrepareService();
        }

        protected override void StartService()
        {
            var tasks = Services.Select(service => service.Start());
            Task.WaitAll(tasks.ToArray());

            base.StartService();
            LogToken();
        }

        protected override void StopService()
        {
            var tasks = Services.Select(service => service.Stop());
            Task.WaitAll(tasks.ToArray());

            base.StopService();
        }

        public override string Log
        {
            get
            {
                var services = new[] { this }.Concat(Services);
                return string.Join("\r\n", services.Select(service => $"{DateTime.Now}\t{service.GetType().GetFullName()}\t{service.ServiceLifetime}\t{service.State}"));
            }
        }

        public void Dispose()
        {
            StopService();
            FlushService();
        }
    }
}
