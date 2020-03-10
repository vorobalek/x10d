using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using X10D.Infrastructure;

namespace X10D.Core.Services
{
    internal sealed class Debugger : ServicePrototype<IDebugger>, IDebugger
    {
        public override ServiceLifetime ServiceLifetime => ServiceLifetime.Scoped;
        public override int? LoadPriority => 1;
        private IActivator Activator { get; }

        private IDebuggerCache DebuggerCache { get; }

        public Debugger(IActivator activator, IDebuggerCache debuggerCache)
        {
            Activator = activator;
            DebuggerCache = debuggerCache;
            Idle();
        }

        public IReadOnlyDictionary<string, Type> Components => DebuggerCache.Components;

        public IReadOnlyList<IDebuggerCompomnentInfo> ComponentsInfo => DebuggerCache.ComponentsInfo;

        public IReadOnlyList<IDebuggerSession> Sessions => DebuggerCache.Sessions;
        public Task<IDebuggerSession> ProcessDebugAsync(string[] keys) => ProcessDebugAsync(null, keys);
        public Task<IDebuggerSession> ProcessDebugAsync(string session_name, string[] keys)
        {
            return Task.Run(() =>
            {
                foreach (var key in keys)
                {
                    if (Components.TryGetValue(key, out var componentType))
                    {
                        var component = Activator.GetServiceOrCreateInstance(componentType);
                        var method = componentType.GetMethod(nameof(DebuggerComponent.Invoke));

                        if (method != null)
                        {
                            var args = method.GetParameters()
                                .Select(p => p.ParameterType)
                                .Select(type => Activator.GetServiceOrCreateInstance(type));

                            method.Invoke(component, args.ToArray());
                        }
                    }
                }
                var session = Activator.GetServiceOrCreateInstance<IDebuggerSession>();
                if (!session.IsTemporary)
                {
                    session.SetName(session_name);
                    DebuggerCache.AddSession(session);
                }
                return session;
            });
        }
    }
}
