﻿using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using X10D.Infrastructure;

namespace X10D.Core.Services
{
    internal sealed class DebuggerCache : ServicePrototype<IDebuggerCache>, IDebuggerCache
    {
        public override ServiceLifetime ServiceLifetime => ServiceLifetime.Singleton;
        public override int? LoadPriority => 0;
        private IActivator Activator { get; }

        private IDictionary<string, Type> components;
        public IReadOnlyDictionary<string, Type> Components
        {
            get
            {
                return components.ToDictionary(kv => kv.Key, kv => kv.Value) as IReadOnlyDictionary<string, Type>;
            }
        }

        private IList<IDebuggerCompomnentInfo> componentsInfo;
        public IReadOnlyList<IDebuggerCompomnentInfo> ComponentsInfo
        {
            get
            {
                return componentsInfo.ToList().AsReadOnly();
            }
        }

        private IList<IDebuggerSession> sessions;
        public IReadOnlyList<IDebuggerSession> Sessions
        {
            get
            {
                return sessions.ToList().AsReadOnly();
            }
        }

        public DebuggerCache(IActivator activator)
        {
            Activator = activator;
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

        public void AddSession(IDebuggerSession session)
        {
            sessions.Add(session);
        }
    }
}