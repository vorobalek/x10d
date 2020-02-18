using System;
using System.Collections.Generic;
using X10D.Infrastructure;

namespace X10D.Core.Services
{
    internal interface IDebuggerSession : IDebuggerSessionFacade
    {
        void ProcessDebug(string key);
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
        new IDebuggerSession CreateSession();
    }
}