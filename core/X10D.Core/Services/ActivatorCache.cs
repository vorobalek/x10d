using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using X10D.Infrastructure;

namespace X10D.Core.Services
{
    internal sealed class ActivatorCache : ServicePrototype<IActivatorCache>, IActivatorCache
    {
        public override ServiceLifetime ServiceLifetime => ServiceLifetime.Singleton;
     
        public ConcurrentDictionary<Type, List<Type>> Implementations { get; private set; }

        public ConcurrentDictionary<Type, List<Type>> Inheritors { get; private set; }

        public List<Assembly> Assemblies { get; private set; }

        public override int? LoadPriority => int.MinValue;

        protected override void FlushService()
        {
            Implementations = new ConcurrentDictionary<Type, List<Type>>();
            Inheritors = new ConcurrentDictionary<Type, List<Type>>();
            Assemblies = new List<Assembly>();

            base.FlushService();
        }

        protected override void PrepareService()
        {
            Implementations = new ConcurrentDictionary<Type, List<Type>>();
            Inheritors = new ConcurrentDictionary<Type, List<Type>>();
            Assemblies = ExtCore.Infrastructure.ExtensionManager.Assemblies?.ToList() ?? new List<Assembly>(new[] { Assembly.GetExecutingAssembly() });

            base.PrepareService();
        }
    }
}
