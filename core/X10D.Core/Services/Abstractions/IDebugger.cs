using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using X10D.Infrastructure;

namespace X10D.Core.Services
{
    internal interface IDebugger : IServicePrototype, IDebuggerFacade
    {
        IReadOnlyDictionary<string, Type> Components { get; }
        IReadOnlyList<IDebuggerCompomnentInfo> ComponentsInfo { get; }
        IReadOnlyList<IDebuggerSession> Sessions { get; }
        new Task<IDebuggerSession> ProcessDebugAsync(string[] keys);
        new Task<IDebuggerSession> ProcessDebugAsync(string session_name, string[] keys);
    }
}