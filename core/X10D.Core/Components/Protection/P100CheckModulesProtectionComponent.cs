using X10D.Core.Services;
using X10D.Infrastructure;

namespace X10D.Core.Components.Protection
{
    internal sealed class P100CheckModulesProtectionComponent
    {
        IKernelProtection KernelProtection { get; }
        IStoredCache StoredCache { get; }

        public P100CheckModulesProtectionComponent(IKernelProtection kernelProtection, IStoredCache storedCache)
        {
            KernelProtection = kernelProtection;
            StoredCache = storedCache;
        }

        public int Priority => 100;
        public bool Invoke()
        {
            if (StoredCache["modules.path"] == null)
            {
                KernelProtection.SafeRedirectUrl = "/kernel/install/modules";
                return false;
            }
            else if (StoredCache["modules.recursive"] == null)
            {
                KernelProtection.SafeRedirectUrl = "/kernel/install/modules";
                return false;
            }

            return true;
        }
    }
}
