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
                var session = await debugger.ProcessDebugAsync(debug.Split(',')).ConfigureAwait(true);
                await context.Response.WriteAsync(session.DebugInfoString).ConfigureAwait(true);
            }
        }
    }
}
