using Microsoft.AspNetCore.Http;
using System;
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
                var timeStart = DateTime.Now;
                var session = await debugger.ProcessDebugAsync(debug.Split(',')).ConfigureAwait(true);
                session.AddDebugInfo("debugger", $"debug time: {(DateTime.Now - timeStart).TotalMilliseconds} ms.");
                session.Name = nameof(DebuggerMiddleware);
                await context.Response.WriteAsync("\r\n" + session.DebugInfoString).ConfigureAwait(true);
            }
        }
    }
}
