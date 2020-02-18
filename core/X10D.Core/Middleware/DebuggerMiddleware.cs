using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using X10D.Core.Services;

namespace X10D.Core.Middleware
{
    internal sealed class DebuggerMiddleware
    {
        private readonly RequestDelegate _next;

        public DebuggerMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context, IDebugger debugger)
        {
            await _next(context).ConfigureAwait(true);

            if (context.Request.Query.TryGetValue("debug", out var debugObj) && debugObj.Count > 0 && debugObj[0] is string debug)
            {
                var session = debugger.CreateSession();
                foreach (var key in debug.Split(','))
                {
                    session.ProcessDebug(key);
                }
                var dbg_info = "";
                foreach (var info in session.DebugInfo)
                {
                    dbg_info += $"{info.Key}:\r\n";
                    foreach (var item in info.Value)
                    {
                        dbg_info += $"\t{item}\r\n";
                    }
                }
                await context.Response.WriteAsync(dbg_info).ConfigureAwait(true);
            }
        }
    }
}
