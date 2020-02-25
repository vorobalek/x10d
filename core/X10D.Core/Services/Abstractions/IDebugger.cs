using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using X10D.Infrastructure;

namespace X10D.Core.Services
{
    internal interface IDebugger : IDebuggerFacade
    {
        IDictionary<string, Type> Components { get; }
        IList<IDebuggerCompomnentInfo> ComponentsInfo { get; }
        IList<IDebuggerSession> Sessions { get; }
        new Task<IDebuggerSession> ProcessDebugAsync(string[] keys);
    }
}