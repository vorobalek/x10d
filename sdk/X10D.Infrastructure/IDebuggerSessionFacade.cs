using System.Collections.Generic;

namespace X10D.Infrastructure
{
    public interface IDebuggerSessionFacade
    {
        string Name { get; }
        IReadOnlyDictionary<string, IReadOnlyList<string>> DebugInfo { get; }
        bool IsTemporary { get; set; }
        void AddDebugInfo(string key, string value);
    }
}
