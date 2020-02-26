using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using X10D.Infrastructure;

namespace X10D.Core.Services
{
    internal sealed class Debugger : ServicePrototype, IDebugger
    {
        public override ServiceLifetime ServiceLifetime => ServiceLifetime.Singleton;

        private IActivator Activator { get; }

        public Debugger(IActivator activator)
        {
            Activator = activator;
            Idle();
        }

        private IDictionary<string, Type> components;
        public IDictionary<string, Type> Components
        {
            get
            {
                return new Dictionary<string, Type>(components);
            }
        }

        private IList<IDebuggerCompomnentInfo> componentsInfo;
        public IList<IDebuggerCompomnentInfo> ComponentsInfo
        {
            get
            {
                return new List<IDebuggerCompomnentInfo>(componentsInfo);
            }
        }

        private IList<IDebuggerSession> sessions;
        public IList<IDebuggerSession> Sessions
        {
            get
            {
                return new List<IDebuggerSession>(sessions);
            }
        }

        public Task<IDebuggerSession> ProcessDebugAsync(string[] keys)
        {
            return Task.Run(() =>
            {
                using var scope = Activator.GetService<IServiceProvider>().CreateScope();
                var activator = scope.ServiceProvider.GetService<IActivator>();
                foreach (var key in keys)
                {
                    if (components.TryGetValue(key, out var componentType))
                    {
                        var component = activator.GetServiceOrCreateInstance(componentType);
                        var method = componentType.GetMethod(nameof(DebuggerComponent.Invoke));

                        if (method != null)
                        {
                            var args = method.GetParameters()
                                .Select(p => p.ParameterType)
                                .Select(type => activator.GetServiceOrCreateInstance(type));

                            method.Invoke(component, args.ToArray());
                        }
                    }
                }
                var session = activator.GetServiceOrCreateInstance<IDebuggerSession>();
                if (!session.IsTemporary)
                {
                    sessions.Add(session);
                }
                return session;
            });
        }

        async Task<IDebuggerSessionFacade> IDebuggerFacade.ProcessDebugAsync(string[] keys)
        {
            return await (this as IDebugger).ProcessDebugAsync(keys).ConfigureAwait(true);
        }

        protected override void FlushService()
        {
            components = new Dictionary<string, Type>();
            componentsInfo = new List<IDebuggerCompomnentInfo>();
            sessions = new List<IDebuggerSession>();
            base.FlushService();
        }

        protected override void PrepareService()
        {
            components = new Dictionary<string, Type>();
            componentsInfo = new List<IDebuggerCompomnentInfo>();
            sessions = new List<IDebuggerSession>();
            var types = Activator.GetTypes(type =>
                type.Name.Length > nameof(DebuggerComponent).Length
                && type.Name.IndexOf(nameof(DebuggerComponent), StringComparison.InvariantCultureIgnoreCase) == type.Name.Length - nameof(DebuggerComponent).Length
                && type.GetProperty(nameof(DebuggerComponent.Key)) != null
                && type.GetProperty(nameof(DebuggerComponent.Key)).PropertyType == typeof(string)
                && type.GetMethod(nameof(DebuggerComponent.Invoke)) != null
                && type.GetMethod(nameof(DebuggerComponent.Invoke)).GetParameters().Length > 0);

            foreach (var type in types)
            {
                var componentObj = Activator.GetServiceOrCreateInstance(type);
                var componentKey = type.GetProperty(nameof(DebuggerComponent.Key))?.GetValue(componentObj) as string;
                var componentDescription = type.GetProperty(nameof(DebuggerComponent.Description))?.GetValue(componentObj) as string ?? "without description";

                components.Add(componentKey, type);
                componentsInfo.Add(IDebuggerCompomnentInfo.Create(componentKey, componentDescription));
            }
            base.PrepareService();
        }
    }
}
