using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using System;
using System.Threading.Tasks;
using X10D.Core.Services;

namespace X10D.Core.StartupFilters
{
    internal sealed class KernelStartupFilter : IStartupFilter
    {
        IKernel Kernel { get; }
        public KernelStartupFilter(IKernel kernel)
        {
            Kernel = kernel;
        }
        public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next)
        {
            Task.Run(() =>
            {
                Kernel.Flush().Wait();
                Kernel.Prepare().Wait();
                Kernel.Start().Wait();
            });
            return next;
        }
    }
}
