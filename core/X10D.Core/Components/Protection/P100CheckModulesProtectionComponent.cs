using X10D.Core.Services;
using X10D.Infrastructure;

namespace X10D.Core.Components.Protection
{
    internal sealed class P100CheckModulesProtectionComponent
    {
        public int Priority => 100;
        public bool Invoke(IKernelProtection kernelProtection, IStoredCache storedCache)
        {
            if (storedCache["modules.path"] == null ||
                storedCache["modules.recursive"] == null)
            {
                kernelProtection.SafeRedirectUrl = "/kernel/install";
                return false;
            }

            return true;
        }
    }
}
