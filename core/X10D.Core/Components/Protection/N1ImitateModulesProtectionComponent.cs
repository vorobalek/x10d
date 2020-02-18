using System.Globalization;
using X10D.Core.Services;

namespace X10D.Core.Components.Protection
{
    internal sealed class N1ImitateModulesProtectionComponent
    {
        IStoredCache StoredCache { get; }

        public N1ImitateModulesProtectionComponent(IStoredCache storedCache)
        {
            StoredCache = storedCache;
        }

        public int Priority => -1;
        public bool Invoke()
        {
            StoredCache["modules.path"] = "modules";
            StoredCache["modules.recursive"] = true.ToString(new CultureInfo("en-US"));

            return true;
        }
    }
}
