using System.Collections.Generic;

namespace X10D.Infrastructure
{
    public interface IDebuggerSessionFacade
    {
        IDictionary<string, IList<string>> DebugInfo { get; }
        void AddDebugInfo(string key, string value);
    }

    public interface IDebuggerFacade : IServicePrototype
    {
        IDebuggerSessionFacade CreateSession();
    }
}