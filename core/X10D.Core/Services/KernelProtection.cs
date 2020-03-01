using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using X10D.Infrastructure;

namespace X10D.Core.Services
{
    internal sealed class KernelProtection : ServicePrototype<IKernelProtection>, IKernelProtection
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

        public IList<string> SafeUrlPrefixes { get; private set; } = defaultSafeUrlPrefixes;

        public bool IsReady => State == ServiceState.InProgress;

        private IActivator Activator { get; }
        
        private IKernel Kernel { get; }

        private ILogger Logger { get; }

        public KernelProtection(IActivator activator, IKernel kernel, ILogger<IKernelProtection> logger)
        {
            Activator = activator;
            Kernel = kernel;
            Logger = logger;
            Idle();
        }

        public override string Log => base.Log;

        protected override void FlushService()
        {
            components = new SortedList<int, Type>();
            SafeUrlPrefixes = new List<string>();
            SafeRedirectUrl = null;

            base.FlushService();
        }

        protected override void PrepareService()
        {
            components = new SortedList<int, Type>();
            SafeUrlPrefixes = defaultSafeUrlPrefixes;
            SafeRedirectUrl = "/kernel/api/log";

            var types = Activator.GetTypes(type =>
                type.Name.Length > nameof(ProtectionComponent).Length
                && type.Name.IndexOf(nameof(ProtectionComponent), StringComparison.InvariantCultureIgnoreCase) == type.Name.Length - nameof(ProtectionComponent).Length
                && type.GetProperty(nameof(ProtectionComponent.Priority)) != null
                && type.GetProperty(nameof(ProtectionComponent.Priority)).PropertyType == typeof(int)
                && type.GetMethod(nameof(ProtectionComponent.Invoke)) != null
                && type.GetMethod(nameof(ProtectionComponent.Invoke)).ReturnType == typeof(bool));

            foreach (var componentType in types)
            {
                var componentObj = Activator.CreateEmpty(componentType);
                var componentKey = componentType.GetProperty(nameof(ProtectionComponent.Priority)).GetValue(componentObj) as int? ?? int.MinValue;
                components.Add(componentKey, componentType);
            }

            base.PrepareService();
        }

        protected override void StartService()
        {
            foreach (var componentType in components.Values)
            {
                var componentObj = Activator.GetServiceOrCreateInstance(componentType);
                if ((componentType.GetProperty(nameof(ProtectionComponent.IsRelevant))?.GetValue(componentObj) ?? true as object) is bool isRelevant && 
                    isRelevant)
                {
                    using var scope = Activator.GetService<IServiceProvider>().CreateScope();
                    var activator = scope.ServiceProvider.GetService<IActivator>();
                    var method = componentType.GetMethod(nameof(ProtectionComponent.Invoke));

                    if (method != null)
                    {
                        var args = method.GetParameters()
                            .Select(p => p.ParameterType)
                            .Select(type => activator.GetServiceOrCreateInstance(type));
                        
                        var invokeResultObj = method.Invoke(componentObj, args.ToArray());

                        if (invokeResultObj is bool invokeResult &&
                            !invokeResult)
                        {
                            Logger.LogCritical($"Kernel protection point {componentType.GetFullName()}:\tBLOCK");
                            
                            Block().Wait();
                            Kernel.Block().Wait();
                            
                            break;
                        }
                        Logger.LogInformation($"Kernel protection point {componentType.GetFullName()}:\tOK");
                    }
                }
            }

            base.StartService();
        }

        private sealed class ProtectionComponent
        {
            internal bool IsRelevant => false;
            internal int Priority => int.MinValue;
            internal bool Invoke() { return true; }
        }
        private string safeRedirectUrl;
        private SortedList<int, Type> components = new SortedList<int, Type>();
        private static IList<string> defaultSafeUrlPrefixes = new List<string>(new[]
        {
            "/api/kernel",
        });
    }
}
