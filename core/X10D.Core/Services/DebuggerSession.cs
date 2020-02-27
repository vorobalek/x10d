using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Linq;
using X10D.Infrastructure;

namespace X10D.Core.Services
{
    internal sealed class DebuggerSession : ServicePrototype, IDebuggerSession
    {
        public override ServiceLifetime ServiceLifetime => ServiceLifetime.Scoped;
        public string Name { get; private set; }

        private IDictionary<string, IList<string>> debugInfo = new Dictionary<string, IList<string>>();
        public IReadOnlyDictionary<string, IReadOnlyList<string>> DebugInfo
        {
            get
            {
                return debugInfo.ToDictionary(kv => kv.Key, kv => kv.Value as IReadOnlyList<string>) as IReadOnlyDictionary<string, IReadOnlyList<string>>;
            }
        }

        public string DebugInfoString
        {
            get
            {
                var dbg_info = "";
                foreach (var info in debugInfo)
                {
                    dbg_info += $"{info.Key}:\r\n";
                    foreach (var item in info.Value)
                    {
                        var tmp_item = string.Join("\r\n\t", item.Split("\r\n"));
                        dbg_info += $"\t{tmp_item}\r\n";
                    }
                };
                return dbg_info;
            }
        }

        public bool IsTemporary { get; set; }

        public void AddDebugInfo(string key, string value)
        {
            if (debugInfo.ContainsKey(key))
            {
                if (debugInfo[key] is IList<string> list)
                {
                    list.Add(value);
                }
                else
                {
                    debugInfo[key] = new List<string>(new[] { value });
                }
            }
            else
            {
                debugInfo.Add(key, new List<string>(new[] { value }));
            }
        }
        public void SetName(string name)
        {
            Name = name;
        }
    }
}
