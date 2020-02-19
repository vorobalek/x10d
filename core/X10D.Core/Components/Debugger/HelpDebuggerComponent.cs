using System.Linq;
using X10D.Core.Services;

namespace X10D.Core.Components.Debugger
{
    internal sealed class HelpDebuggerComponent
    {
        public string Key => "help";
        public string Description => "Show available debugger components with description.";

        public void Invoke(IDebuggerSession session, IDebugger debugger)
        {
            var components = debugger.ComponentsInfo.OrderBy(component => component.Key);
            foreach (var component in components)
            {
                session.AddDebugInfo(Key, $"{component.Key}\t{component.Description}");
            }
        }
    }
}
