using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using X10D.Infrastructure;

namespace X10D.Core.Services
{
    internal interface IDebuggerSession : IDebuggerSessionFacade, IServicePrototype
    {
        string DebugInfoString { get; }
    }

    internal interface IDebuggerCompomnent
    {
        string Key { get; }
        string Description { get; }
        static IDebuggerCompomnent Create(string key, string description)
        {
            return new DebuggerComponent()
            {
                Key = key,
                Description = description,
            };
        }

        private sealed class DebuggerComponent : IDebuggerCompomnent
        {
            public string Key { get; set; }
            public string Description { get; set; }
        }
    }

    internal interface IDebugger : IDebuggerFacade
    {
        IDictionary<string, Type> Components { get; }
        IList<IDebuggerCompomnent> ComponentsInfo { get; }
        IList<IDebuggerSession> Sessions { get; }
        new Task<IDebuggerSession> ProcessDebugAsync(string[] keys);
    }
}