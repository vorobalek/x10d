using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using X10D.Infrastructure;

namespace X10D.Core.Services
{
    internal sealed class DebuggerSession : ServicePrototype, IDebuggerSession
    {
        public override ServiceLifetime ServiceLifetime => ServiceLifetime.Scoped;
        public string Name { get; set; }

        private IDictionary<string, IList<string>> debugInfo = new Dictionary<string, IList<string>>();
        public IDictionary<string, IList<string>> DebugInfo
        {
            get
            {
                return new Dictionary<string, IList<string>>(debugInfo);
            }
            private set
            {
                debugInfo = value;
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
    }
}
