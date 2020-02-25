using System;
using System.Collections.Generic;

namespace X10D.Infrastructure
{
    public interface IDebuggerSessionFacade
    {
        string Name { get; set; }
        IDictionary<string, IList<string>> DebugInfo { get; }
        bool IsTemporary { get; set; }
        void AddDebugInfo(string key, string value);
    }
}
