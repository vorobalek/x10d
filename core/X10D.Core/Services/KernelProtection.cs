using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using X10D.Infrastructure;

namespace X10D.Core.Services
{
    internal sealed class KernelProtection : ServicePrototype, IKernelProtection
    {
        public override ServiceLifetime ServiceLifetime => ServiceLifetime.Singleton;

        public string SafeRedirectUrl 
        {
            get
            {
                return safeRedirectUrl;
            }
            set
            {
                if (!SafeUrlPrefixes.Any(prefix => value.StartsWith(prefix, StringComparison.InvariantCultureIgnoreCase)))
                {
                    SafeUrlPrefixes.Add(value);
                }
                safeRedirectUrl = value;
            } 
        }

        public IList<string> SafeUrlPrefixes { get; private set; } = new List<string>(new[] { "/kernel/api/log" });

        public bool IsReady => State == ServiceState.InProgress;

        private IActivator Activator { get; }
        
        private IKernel Kernel { get; }

        public KernelProtection(IActivator activator, IKernel kernel)
        {
            Activator = activator;
            Kernel = kernel;
            Idle();
        }

        public override string Log => base.Log;

        protected override void FlushService()
        {
            components = new SortedList<int, TypedObject>();
            SafeUrlPrefixes = new List<string>();
            SafeRedirectUrl = null;

            base.FlushService();
        }

        protected override void PrepareService()
        {
            components = new SortedList<int, TypedObject>();
            SafeUrlPrefixes = new List<string>();
            SafeRedirectUrl = "/kernel/api/log";

            var types = Activator.GetTypes(type =>
                type.Name.Length > nameof(ProtectionComponent).Length
                && type.Name.IndexOf(nameof(ProtectionComponent), StringComparison.InvariantCultureIgnoreCase) == type.Name.Length - nameof(ProtectionComponent).Length
                && type.GetProperty(nameof(ProtectionComponent.Priority)) != null
                && type.GetProperty(nameof(ProtectionComponent.Priority)).PropertyType == typeof(int)
                && type.GetMethod(nameof(ProtectionComponent.Invoke)) != null
                && type.GetMethod(nameof(ProtectionComponent.Invoke)).ReturnType == typeof(bool));

            foreach (var type in types)
            {
                var componentObj = new TypedObject(type, Activator.CreateInstance(type));
                var componentKey = type.GetProperty(nameof(ProtectionComponent.Priority)).GetValue(componentObj.Object) as int? ?? int.MinValue;
                components.Add(componentKey, componentObj);
            }

            base.PrepareService();
        }

        protected override void StartService()
        {
            foreach (var component in components)
            {
                var result = component.Value.Type.GetMethod(nameof(ProtectionComponent.Invoke)).Invoke(component.Value.Object, null);
                if (result is bool flag && !flag)
                {
                    (this as IServicePrototype).Block().Wait();
                    Kernel.Block().Wait();
                    break;
                }
            }

            base.StartService();
        }

        private sealed class ProtectionComponent
        {
            internal int Priority => int.MinValue;
            internal bool Invoke() { return true; }
        }
        private string safeRedirectUrl;
        private SortedList<int, TypedObject> components = new SortedList<int, TypedObject>();
    }
}
