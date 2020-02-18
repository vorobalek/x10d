using System;
using X10D.Core.Services;

namespace X10D.Core.Components.Debugger
{
    internal sealed class ServerUTCTimeDebuggerComponent
    {
        public string Key => "server_utc_time";
        public string Description => "Show server time in UTC format. Example: ?debug=server_utc_time";

        public void Invoke(IDebuggerSession session)
        {
            session.AddDebugInfo(Key, $"{DateTime.Now.ToUniversalTime()}");
        }
    }
}
