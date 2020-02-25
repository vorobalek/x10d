using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using X10D.Infrastructure;

namespace X10D.Core.Services
{
    internal sealed class Activator : ServicePrototype, IActivator
    {
        private static ConcurrentDictionary<Type, IEnumerable<Type>> implementations = new ConcurrentDictionary<Type, IEnumerable<Type>>();
        private static ConcurrentDictionary<Type, IEnumerable<Type>> inheritors = new ConcurrentDictionary<Type, IEnumerable<Type>>();
        private static IList<Assembly> assemblies  = new List<Assembly>();

        protected override void FlushService()
        {
            implementations = new ConcurrentDictionary<Type, IEnumerable<Type>>();
            inheritors = new ConcurrentDictionary<Type, IEnumerable<Type>>();
            assemblies = new List<Assembly>();

            base.FlushService();
        }

        protected override void PrepareService()
        {
            implementations = new ConcurrentDictionary<Type, IEnumerable<Type>>();
            inheritors = new ConcurrentDictionary<Type, IEnumerable<Type>>();
            assemblies = ExtCore.Infrastructure.ExtensionManager.Assemblies?.ToList() ?? new List<Assembly>(new[] { Assembly.GetExecutingAssembly() });

            base.PrepareService();
        }

        public override ServiceLifetime ServiceLifetime => ServiceLifetime.Transient;
        private IServiceProvider ServiceProvider { get; }
        public Activator(IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;
            Idle();
        }

        public IList<Assembly> Assemblies => new List<Assembly>(assemblies);

        public IList<Type> GetTypes(Func<Type, bool> predicate = null)
        {
            if (predicate == null)
                return Assemblies.SelectMany(assembly => assembly.GetTypes()).ToList();

            return Assemblies.SelectMany(assembly => assembly.GetTypes()).Where(predicate).ToList();
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

        public IEnumerable<Type> GetInheritors<T>(bool useCaching = true)
        {
            return GetInheritors<T>(null, useCaching);
        }

        public IEnumerable<Type> GetInheritors<T>(Func<Type, bool> predicate, bool useCaching = true)
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

        public IEnumerable<Type> GetInheritors(Type targetType, bool useCaching = true)
        {
            return GetInheritors(targetType, null, useCaching);
        }

        public IEnumerable<Type> GetInheritors(Type targetType, Func<Type, bool> predicate, bool useCaching = true)
        {
            if (useCaching && inheritors.ContainsKey(targetType))
                return inheritors[targetType];

            List<Type> typeInheritors = new List<Type>();

            foreach (Type type in GetTypes(predicate))
                if (targetType.GetTypeInfo().IsAssignableFrom(type))
                    typeInheritors.Add(type);

            if (useCaching)
                inheritors[targetType] = typeInheritors;

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

        public IEnumerable<Type> GetImplementations<T>(bool useCaching = true)
        {
            return GetImplementations<T>(null, useCaching);
        }

        public IEnumerable<Type> GetImplementations<T>(Func<Type, bool> predicate, bool useCaching = true)
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

        public IEnumerable<Type> GetImplementations(Type targetType, bool useCaching = true)
        {
            return GetImplementations(targetType, null, useCaching);
        }

        public IEnumerable<Type> GetImplementations(Type targetType, Func<Type, bool> predicate, bool useCaching = true)
        {
            if (useCaching && implementations.ContainsKey(targetType))
                return implementations[targetType];

            List<Type> typeImplementations = new List<Type>();

            foreach (Type type in GetTypes(predicate))
                if (targetType.GetTypeInfo().IsAssignableFrom(type) && type.GetTypeInfo().IsClass)
                    typeImplementations.Add(type);

            if (useCaching)
                implementations[targetType] = typeImplementations;

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

        public IEnumerable<T> GetInstances<T>(bool useCaching = true, params object[] args)
        {
            return GetInstances<T>(null, useCaching, args);
        }

        public IEnumerable<T> GetInstances<T>(Func<Type, bool> predicate, bool useCaching = true, params object[] args)
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

        public IEnumerable<object> GetInstances(Type targetType, bool useCaching = true, params object[] args)
        {
            return GetInstances(targetType, null, useCaching, args);
        }

        public IEnumerable<object> GetInstances(Type targetType, Func<Type, bool> predicate, bool useCaching = true, params object[] args)
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

        public T CreateInstance<T>(params object[] args)
        {
            return ActivatorUtilities.CreateInstance<T>(ServiceProvider, args);
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
