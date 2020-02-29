using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using X10D.Infrastructure;

namespace X10D.Core.Services
{
    internal sealed class Activator : ServicePrototype<IActivator>, IActivator
    {
        private ConcurrentDictionary<Type, List<Type>> Implementations => ActivatorCache.Implementations;
        private ConcurrentDictionary<Type, List<Type>> Inheritors => ActivatorCache.Inheritors;

        public override ServiceLifetime ServiceLifetime => ServiceLifetime.Transient;
        private IServiceProvider ServiceProvider { get; }
        private IActivatorCache ActivatorCache { get; }
        public Activator(IServiceProvider serviceProvider, IActivatorCache activatorCache)
        {
            ServiceProvider = serviceProvider;
            ActivatorCache = activatorCache;
            Idle();
        }

        public IReadOnlyList<Assembly> Assemblies => ActivatorCache.Assemblies.AsReadOnly();

        public IReadOnlyList<Type> GetTypes(Func<Type, bool> predicate = null)
        {
            if (predicate == null)
                return Assemblies.SelectMany(assembly => assembly.GetTypes()).ToList().AsReadOnly();

            return Assemblies.SelectMany(assembly => assembly.GetTypes()).Where(predicate).ToList().AsReadOnly();
        }

        #region Inheritor

        #region Template

        public Type GetInheritor<T>(bool useCaching = true)
        {
            return GetInheritors<T>(useCaching).FirstOrDefault();
        }

        public Type GetInheritor<T>(Func<Type, bool> predicate, bool useCaching = true)
        {
            return GetInheritors<T>(predicate, useCaching).FirstOrDefault();
        }

        public IReadOnlyList<Type> GetInheritors<T>(bool useCaching = true)
        {
            return GetInheritors<T>(null, useCaching);
        }

        public IReadOnlyList<Type> GetInheritors<T>(Func<Type, bool> predicate, bool useCaching = true)
        {
            return GetInheritors(typeof(T), predicate, useCaching);
        }

        #endregion

        #region Object

        public Type GetInheritor(Type targetType, bool useCaching = true)
        {
            return GetInheritors(targetType, useCaching).FirstOrDefault();
        }

        public Type GetInheritor(Type targetType, Func<Type, bool> predicate, bool useCaching = true)
        {
            return GetInheritors(targetType, predicate, useCaching).FirstOrDefault();
        }

        public IReadOnlyList<Type> GetInheritors(Type targetType, bool useCaching = true)
        {
            return GetInheritors(targetType, null, useCaching);
        }

        public IReadOnlyList<Type> GetInheritors(Type targetType, Func<Type, bool> predicate, bool useCaching = true)
        {
            if (useCaching && Inheritors.ContainsKey(targetType))
                return Inheritors[targetType].AsReadOnly();

            List<Type> typeInheritors = new List<Type>();

            foreach (Type type in GetTypes(predicate))
                if (targetType.GetTypeInfo().IsAssignableFrom(type))
                    typeInheritors.Add(type);

            if (useCaching)
                Inheritors[targetType] = typeInheritors;

            return typeInheritors;
        }

        #endregion

        #endregion

        #region Implementation

        #region Template

        public Type GetImplementation<T>(bool useCaching = true)
        {
            return GetImplementations<T>(useCaching).FirstOrDefault();
        }

        public Type GetImplementation<T>(Func<Type, bool> predicate, bool useCaching = true)
        {
            return GetImplementations<T>(predicate, useCaching).FirstOrDefault();
        }

        public IReadOnlyList<Type> GetImplementations<T>(bool useCaching = true)
        {
            return GetImplementations<T>(null, useCaching);
        }

        public IReadOnlyList<Type> GetImplementations<T>(Func<Type, bool> predicate, bool useCaching = true)
        {
            return GetImplementations(typeof(T), predicate, useCaching);
        }

        #endregion

        #region Object

        public Type GetImplementation(Type targetType, bool useCaching = true)
        {
            return GetImplementations(targetType, useCaching).FirstOrDefault();
        }

        public Type GetImplementation(Type targetType, Func<Type, bool> predicate, bool useCaching = true)
        {
            return GetImplementations(targetType, predicate, useCaching).FirstOrDefault();
        }

        public IReadOnlyList<Type> GetImplementations(Type targetType, bool useCaching = true)
        {
            return GetImplementations(targetType, null, useCaching);
        }

        public IReadOnlyList<Type> GetImplementations(Type targetType, Func<Type, bool> predicate, bool useCaching = true)
        {
            if (useCaching && Implementations.ContainsKey(targetType))
                return Implementations[targetType].AsReadOnly();

            List<Type> typeImplementations = new List<Type>();

            foreach (Type type in GetTypes(predicate))
                if (targetType.GetTypeInfo().IsAssignableFrom(type) && type.GetTypeInfo().IsClass)
                    typeImplementations.Add(type);

            if (useCaching)
                Implementations[targetType] = typeImplementations;

            return typeImplementations;
        }

        #endregion

        #endregion

        #region Instance

        #region Template

        public T GetInstance<T>(bool useCaching = true, params object[] args)
        {
            return GetInstance<T>(null, useCaching, args);
        }

        public T GetInstance<T>(Func<Type, bool> predicate, bool useCaching = true, params object[] args)
        {
            return GetInstances<T>(predicate, useCaching, args).FirstOrDefault();
        }

        public IReadOnlyList<T> GetInstances<T>(bool useCaching = true, params object[] args)
        {
            return GetInstances<T>(null, useCaching, args);
        }

        public IReadOnlyList<T> GetInstances<T>(Func<Type, bool> predicate, bool useCaching = true, params object[] args)
        {
            List<T> instances = new List<T>();

            foreach (Type implementation in GetImplementations<T>(predicate, useCaching))
            {
                if (!implementation.GetTypeInfo().IsAbstract)
                {
                    T instance = GetServiceOrCreateInstance<T>(implementation, args);

                    instances.Add(instance);
                }
            }

            return instances;
        }

        #endregion

        #region Object

        public object GetInstance(Type targetType, bool useCaching = true, params object[] args)
        {
            return GetInstance(targetType, null, useCaching, args);
        }

        public object GetInstance(Type targetType, Func<Type, bool> predicate, bool useCaching = true, params object[] args)
        {
            return GetInstances(targetType, predicate, useCaching, args).FirstOrDefault();
        }

        public IReadOnlyList<object> GetInstances(Type targetType, bool useCaching = true, params object[] args)
        {
            return GetInstances(targetType, null, useCaching, args);
        }

        public IReadOnlyList<object> GetInstances(Type targetType, Func<Type, bool> predicate, bool useCaching = true, params object[] args)
        {
            List<object> instances = new List<object>();

            foreach (Type implementation in GetImplementations(targetType, predicate, useCaching))
            {
                if (!implementation.GetTypeInfo().IsAbstract)
                {
                    var instance = GetServiceOrCreateInstance(implementation, args);

                    instances.Add(instance);
                }
            }

            return instances;
        }

        #endregion

        #endregion

        #region Service

        #region Template

        public T GetService<T>()
        {
            return ServiceProvider.GetService<T>();
        }

        public T CreateEmpty<T>()
        {
            return (T)CreateEmpty(typeof(T));
        }

        public T CreateEmpty<T>(Type type)
        {
            return (T)CreateEmpty(type);
        }

        public T CreateInstance<T>(params object[] args)
        {
            return CreateInstance<T>(typeof(T), args);
        }

        public T CreateInstance<T>(Type type, params object[] args)
        {
            return (T)CreateInstance(type, args);
        }

        public T GetServiceOrCreateInstance<T>(params object[] args)
        {
            return GetServiceOrCreateInstance<T>(typeof(T), args);
        }

        public T GetServiceOrCreateInstance<T>(Type type, params object[] args)
        {
            return GetService<T>() ?? (T)CreateInstance(type, args);
        }

        #endregion

        #region Object

        public object GetService(Type type)
        {
            return ServiceProvider.GetService(type);
        }

        public object CreateEmpty(Type type)
        {
            return FormatterServices.GetUninitializedObject(type);
        }

        public object CreateInstance(Type type, params object[] args)
        {
            return ActivatorUtilities.CreateInstance(ServiceProvider, type, args);
        }

        public object GetServiceOrCreateInstance(Type type, params object[] args)
        {
            return GetService(type) ?? CreateInstance(type, args);
        }

        #endregion

        #endregion

    }
}
