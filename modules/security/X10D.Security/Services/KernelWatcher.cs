using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Threading;
using X10D.Infrastructure;

namespace X10D.Security.Services
{
    internal sealed class KernelWatcher : ServicePrototype
    {
        public override ServiceLifetime ServiceLifetime => ServiceLifetime.Singleton;

        IKernelFacade Kernel { get; }
        ILogger Logger { get; }
        public KernelWatcher(IKernelFacade kernel, ILogger<KernelWatcher> logger)
        {
            Kernel = kernel;
            Logger = logger;
        }

        protected override ThreadStart MainProcess =>
            () =>
            {
                CriticalWhile(() => true, () =>
                {
                    Logger.LogInformation($"Kernel state: {Kernel.State}");
                    Thread.Sleep(1000);
                });
            };
    }
}
