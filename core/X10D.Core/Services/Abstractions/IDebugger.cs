using System;
using System.Collections.Generic;
using X10D.Infrastructure;

namespace X10D.Core.Services
{
    internal interface IDebugger : IServicePrototype, IDebuggerFacade<IDebuggerSession>
    {
        IReadOnlyDictionary<string, Type> Components { get; }
        IReadOnlyList<IDebuggerCompomnentInfo> ComponentsInfo { get; }
        IReadOnlyList<IDebuggerSession> Sessions { get; }
    }
}