using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using X10D.Infrastructure;

namespace X10D.Core.Services
{
    internal sealed class DebuggerComponent
    {
        internal string Key => "template";
        internal string Description => "template";
        internal void Invoke(IDebuggerSessionFacade session) { session.AddDebugInfo(Key, "template"); }
    }

    internal sealed class DebuggerSession : IDebuggerSession
    {
        private IDebugger Debugger { get; }

        private IActivator Activator { get; }

        public DebuggerSession(IDebugger debugger, IActivator activator)
        {
            Debugger = debugger;
            Activator = activator;
        }

        void IDebuggerSession.ProcessDebug(string key)
        {
            if (Debugger.Components.TryGetValue(key, out var componentType))
            {
                var component = Activator.CreateInstance(componentType);
                var componentMethod = component?.GetType().GetMethod(nameof(DebuggerComponent.Invoke));
                var @params = componentMethod.GetParameters();
                var args = @params
                    .Select(p => p.ParameterType)
                    .Select(type => type == typeof(IDebuggerSessionFacade) || type.GetInterfaces().Contains(typeof(IDebuggerSessionFacade))
                    ? this
                    : Activator.GetServiceOrCreateInstance(type));

                componentMethod.Invoke(component, args.ToArray());
            }
        }

        IDictionary<string, IList<string>> IDebuggerSessionFacade.DebugInfo
        {
            get
            {
                return new Dictionary<string, IList<string>>(debugInfo);
            }
        }

        void IDebuggerSessionFacade.AddDebugInfo(string key, string value)
        {
            if (debugInfo.ContainsKey(key))
            {
                if (debugInfo[key] is IList<string> list)
                {
                    list.Add(value);
                }
                else
                {
                    debugInfo[key] = new List<string>(new[] { value });
                }
            }
            else
            {
                debugInfo.Add(key, new List<string>(new[] { value }));
            }
        }

        private IDictionary<string, IList<string>> debugInfo = new Dictionary<string, IList<string>>();
    }

    internal sealed class Debugger : ServicePrototype, IDebugger
    {
        public override ServiceLifetime ServiceLifetime => ServiceLifetime.Singleton;
        
        private IActivator Activator { get; }

        public Debugger(IActivator activator)
        {
            Activator = activator;
            Idle();
        }

        IDictionary<string, Type> IDebugger.Components
        {
            get
            {
                return new Dictionary<string, Type>(components);
            }
        }
        IList<IDebuggerCompomnent> IDebugger.ComponentsInfo
        {
            get
            {
                return new List<IDebuggerCompomnent>(componentsInfo);
            }
        }

        IDebuggerSession IDebugger.CreateSession()
        {
            return Activator.CreateInstance<DebuggerSession>();
        }

        IDebuggerSessionFacade IDebuggerFacade.CreateSession()
        {
            return (this as IDebugger).CreateSession();
        }

        protected override void FlushService()
        {
            components = new Dictionary<string, Type>();
            componentsInfo = new List<IDebuggerCompomnent>();
            base.FlushService();
        }

        protected override void PrepareService()
        {
            components = new Dictionary<string, Type>();
            componentsInfo = new List<IDebuggerCompomnent>();
            var types = Activator.GetTypes(type =>
                type.Name.Length > nameof(DebuggerComponent).Length
                && type.Name.IndexOf(nameof(DebuggerComponent), StringComparison.InvariantCultureIgnoreCase) == type.Name.Length - nameof(DebuggerComponent).Length
                && type.GetProperty(nameof(DebuggerComponent.Key)) != null
                && type.GetProperty(nameof(DebuggerComponent.Key)).PropertyType == typeof(string)
                && type.GetMethod(nameof(DebuggerComponent.Invoke)) != null
                && type.GetMethod(nameof(DebuggerComponent.Invoke)).GetParameters().Length > 0
                && (type.GetMethod(nameof(DebuggerComponent.Invoke)).GetParameters()[0].ParameterType == typeof(IDebuggerSessionFacade)
                    || type.GetMethod(nameof(DebuggerComponent.Invoke)).GetParameters()[0].ParameterType.GetInterfaces().Contains(typeof(IDebuggerSessionFacade))));

            foreach (var type in types)
            {
                var componentObj = Activator.CreateInstance(type);
                var componentKey = type.GetProperty(nameof(DebuggerComponent.Key))?.GetValue(componentObj) as string;
                var componentDescription = type.GetProperty(nameof(DebuggerComponent.Description))?.GetValue(componentObj) as string ?? "without description";

                components.Add(componentKey, type);
                componentsInfo.Add(IDebuggerCompomnent.Create(componentKey, componentDescription));
            }
            base.PrepareService();
        }

        private IDictionary<string, Type> components;
        private IList<IDebuggerCompomnent> componentsInfo;
    }
}
