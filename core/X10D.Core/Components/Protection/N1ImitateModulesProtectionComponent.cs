using System.Globalization;
using X10D.Infrastructure;

namespace X10D.Core.Components.Protection
{
    internal sealed class N1ImitateModulesProtectionComponent
    {
        public int Priority => -1;
        //public bool IsRelevant => false;
        public bool Invoke(IStoredCache storedCache)
        {
            storedCache["modules.path"] = "modules";
            storedCache["modules.recursive"] = true.ToString(new CultureInfo("en-US"));

            return true;
        }
    }
}
