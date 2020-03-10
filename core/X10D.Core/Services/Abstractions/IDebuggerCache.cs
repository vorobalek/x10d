using System;
using System.Collections.Generic;

namespace X10D.Core.Services
{
    internal interface IDebuggerCache
    {
        IReadOnlyDictionary<string, Type> Components { get; }
        IReadOnlyList<IDebuggerCompomnentInfo> ComponentsInfo { get; }
        IReadOnlyList<IDebuggerSession> Sessions { get; }
        void AddSession(IDebuggerSession session);
    }
}