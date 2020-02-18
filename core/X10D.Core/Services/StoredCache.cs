using Microsoft.Extensions.DependencyInjection;
using System.Globalization;
using X10D.Core.Data;
using X10D.Infrastructure;

namespace X10D.Core.Services
{
    internal sealed class StoredCache : ServicePrototype, IStoredCache
    {
        public override ServiceLifetime ServiceLifetime => ServiceLifetime.Singleton;
        private StoredCacheContext Cache { get; }
        public StoredCache(StoredCacheContext cache)
        {
            Cache = cache;
            
            Cache["modules.path"] = "modules";
            Cache["modules.recursive"] = true.ToString(new CultureInfo("en-US"));

            Idle();
        }
        public string this[string key] { get => Cache[key]; set => Cache[key] = value; }
    }
}
