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
    internal sealed class Kernel : ServicePrototype<IKernel>, IKernel
    {
        public override ServiceLifetime ServiceLifetime => ServiceLifetime.Singleton;
        private IServiceProvider ServiceProvider { get; }
        private ILogger Logger { get; }
        private IAssemblyProvider AssemblyProvider { get; }
        private IServiceScope Scope { get; }
        public List<IServicePrototype> Services { get; } = new List<IServicePrototype>();
        public Kernel(IServiceProvider serviceProvider, ILogger<Kernel> logger, IAssemblyProvider assemblyProvider)
        {
            ServiceProvider = serviceProvider;
            Scope = serviceProvider.CreateScope();
            Logger = logger;
            AssemblyProvider = assemblyProvider;
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
            var tasks = Services.Select(service => service.Flush()).ToArray();
            Task.WaitAll(tasks);

            Services.ForEach(service => service.RemoveOnStateChange(OnServiceStateChange));
            Services.Clear();

            base.FlushService();
        }

        protected override void PrepareService()
        {
            Services.AddRange(Scope.ServiceProvider.GetServices<IServicePrototype>().Where(service => !service.GetType().GetInterfaces().Contains(typeof(IKernelFacade))));
            var tasks = Services.Select(service => service.AddOnStateChange(OnServiceStateChange).Prepare()).ToArray();
            Task.WaitAll(tasks);

            base.PrepareService();
        }

        protected override void StartService()
        {
            var tasks = Services.Select(service => service.Start()).ToArray();
            Task.WaitAll(tasks);
            
            LogToken();

            base.StartService();
        }

        protected override void StopService()
        {
            var tasks = Services.Select(service => service.Stop()).ToArray();
            Task.WaitAll(tasks);

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

        public override void Dispose()
        {
            base.Dispose();
            Scope.Dispose();
        }
        
        private void OnServiceStateChange(IServicePrototype service, ServiceStateChangeEventArgs args)
        {
            Logger.LogWarning($"{service.GetType().GetFullName()} {(args.StateChanged ? "changed" : "changing")} [{args.OldValue}] => [{args.NewValue}]");
        }
    }
}
