using System.Globalization;
using System.Linq;
using X10D.Core.Services;

namespace X10D.Core.Components.Debugger
{
    internal sealed class AllSessionsDebuggerComponent
    {
        public string Key => "all_sessions";
        public string Description => "Show all debug info from all previous sessions. Example: ?debug=all_sessions";

        public void Invoke(IDebugger debugger, IDebuggerSession session)
        {
            session.IsTemporary = true;
            var sessions = debugger.Sessions.OrderByDescending(s => s.CreationTime);
            if (sessions.Any())
            {
                foreach (var s in sessions)
                {
                    session.AddDebugInfo($"{Key} ({s.SessionId}) - {s.CreationTime}.{s.CreationTime.Millisecond.ToString("000", new CultureInfo("en-EN"))}", s.DebugInfoString);
                }
            }
            else
            {
                session.AddDebugInfo(Key, "There are no active sessions.");
            }
        }
    }
}
