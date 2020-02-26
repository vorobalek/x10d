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
        private ILogger Logger { get; }
        private IServiceScope Scope { get; }
        public List<IServicePrototype> Services { get; } = new List<IServicePrototype>();
        public Kernel(IServiceProvider serviceProvider, ILogger<Kernel> logger)
        {
            Scope = serviceProvider.CreateScope();
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

        private Guid token = Guid.NewGuid();
        public bool ValidateToken(string candidate)
        {
            return Guid.TryParse(candidate, out var guid_candidate) && guid_candidate == token;
        }

        protected override void FlushService()
        {
            var tasks = Services.Select(service => service.Flush());
            Task.WaitAll(tasks.ToArray());

            Services.ForEach(service => service.RemoveOnStateChange(OnServiceStateChange));
            Services.Clear();
            base.FlushService();
        }

        protected override void PrepareService()
        {
            Services.AddRange(Scope.ServiceProvider.GetServices<IServicePrototype>().Where(service => !service.GetType().GetInterfaces().Contains(typeof(IKernelFacade))));
            var tasks = Services.Select(service => service.AddOnStateChange(OnServiceStateChange).Prepare());
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
            (this as IServicePrototype).Stop().Wait();
            (this as IServicePrototype).Flush().Wait();
        }
        
        private void OnServiceStateChange(object sender, ServiceStateChangeEventArgs args)
        {
            var service = sender as IServicePrototype;
            Logger.LogWarning($"{service.GetType().GetFullName()} {(args.StateChanged ? "changed" : "changing")} [{args.OldValue}] => [{args.NewValue}]");
        }
    }
}
