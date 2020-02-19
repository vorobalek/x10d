using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using X10D.Infrastructure;

namespace X10D.Core.Services
{
    internal sealed class DebuggerComponent
    {
        internal string Key => "template";
        internal string Description => "template";
        internal void Invoke(IDebuggerSessionFacade session) { session.AddDebugInfo(Key, "template"); }
    }

    internal sealed class DebuggerSession : ServicePrototype, IDebuggerSession
    {
        public override ServiceLifetime ServiceLifetime => ServiceLifetime.Scoped;

        Guid IDebuggerSessionFacade.SessionId { get; } = Guid.NewGuid();

        DateTime IDebuggerSessionFacade.CreationTime { get; } = DateTime.Now;

        IDictionary<string, IList<string>> IDebuggerSessionFacade.DebugInfo
        {
            get
            {
                return new Dictionary<string, IList<string>>(debugInfo);
            }
        }

        string IDebuggerSession.DebugInfoString
        {
            get
            {
                var dbg_info = "";
                foreach (var info in (this as IDebuggerSession).DebugInfo)
                {
                    dbg_info += $"{info.Key}:\r\n";
                    foreach (var item in info.Value)
                    {
                        var tmp_item = string.Join("\r\n\t", item.Split("\r\n"));
                        dbg_info += $"\t{tmp_item}\r\n";
                    }
                };
                return dbg_info;
            }
        }

        bool IDebuggerSessionFacade.IsTemporary { get; set; }

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
        IList<IDebuggerSession> IDebugger.Sessions
        {
            get
            {
                return new List<IDebuggerSession>(sessions);
            }
        }

        protected override void FlushService()
        {
            components = new Dictionary<string, Type>();
            componentsInfo = new List<IDebuggerCompomnent>();
            sessions = new List<IDebuggerSession>();
            base.FlushService();
        }

        protected override void PrepareService()
        {
            components = new Dictionary<string, Type>();
            componentsInfo = new List<IDebuggerCompomnent>();
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
                var componentObj = Activator.CreateInstance(type);
                var componentKey = type.GetProperty(nameof(DebuggerComponent.Key))?.GetValue(componentObj) as string;
                var componentDescription = type.GetProperty(nameof(DebuggerComponent.Description))?.GetValue(componentObj) as string ?? "without description";

                components.Add(componentKey, type);
                componentsInfo.Add(IDebuggerCompomnent.Create(componentKey, componentDescription));
            }
            base.PrepareService();
        }

        Task<IDebuggerSession> IDebugger.ProcessDebugAsync(string[] keys)
        {
            return Task.Run(() =>
            {
                using var scope = Activator.GetService<IServiceProvider>().CreateScope();
                foreach (var key in keys)
                {
                    if (components.TryGetValue(key, out var componentType))
                    {
                        var component = ActivatorUtilities.CreateInstance(scope.ServiceProvider, componentType);
                        var method = componentType.GetMethod(nameof(DebuggerComponent.Invoke));

                        if (method != null)
                        {
                            var args = method.GetParameters()
                                .Select(p => p.ParameterType)
                                .Select(type => ActivatorUtilities.GetServiceOrCreateInstance(scope.ServiceProvider, type));

                            method.Invoke(component, args.ToArray());
                        }
                    }
                }
                var session = scope.ServiceProvider.GetService<IDebuggerSession>();
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

        private IDictionary<string, Type> components;
        private IList<IDebuggerCompomnent> componentsInfo;
        private IList<IDebuggerSession> sessions;
    }
}
