using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;
using X10D.Infrastructure;

namespace X10D.Core.Services
{
    internal interface IActivatorCache : IServicePrototype
    {
        ConcurrentDictionary<Type, List<Type>> Implementations { get; }
        ConcurrentDictionary<Type, List<Type>> Inheritors { get; }
        List<Assembly> Assemblies { get; }
    }
}