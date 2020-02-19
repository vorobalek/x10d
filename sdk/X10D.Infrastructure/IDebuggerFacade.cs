using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace X10D.Infrastructure
{
    public interface IDebuggerSessionFacade
    {
        Guid SessionId { get; }
        DateTime CreationTime { get; }
        IDictionary<string, IList<string>> DebugInfo { get; }
        bool IsTemporary { get; set; }
        void AddDebugInfo(string key, string value);
    }

    public interface IDebuggerFacade : IServicePrototype
    {
        Task<IDebuggerSessionFacade> ProcessDebugAsync(string[] keys);
    }
}