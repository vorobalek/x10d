using X10D.Infrastructure;

namespace X10D.Core.Services
{
    internal sealed class DebuggerComponent
    {
        internal string Key => "template";
        internal string Description => "template";
        internal void Invoke(IDebuggerSessionFacade session) { session.AddDebugInfo(Key, "template"); }
    }
}
