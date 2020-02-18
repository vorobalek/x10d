using System;
using System.Linq;
using X10D.Core.Services;

namespace X10D.Core.Components.Debugger
{
    internal sealed class KernelStateDebuggerComponent
    {
        public string Key => "kernel_state";
        public string Description => "Show KernelService state. Example: ?debug=kernel_state";

        public void Invoke(IDebuggerSession session, IKernel kernel)
        {
            var kernelServices = new[] { kernel }.Concat(kernel.Services);
            foreach (var service in kernelServices)
            {
                session.AddDebugInfo(Key, $"{service.GetType().GetFullName()}\t{service.ServiceLifetime}\t{service.State}");
            }
        }
    }
}
