using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;
using X10D.Core.Services;
using X10D.Infrastructure;

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

            if (context.Request.Query.TryGetValue(Constants.Debug, out var debugObj) && debugObj.Count > 0 && debugObj[0] is string debug)
            {
                var timeStart = DateTime.Now;
                var session = await debugger.ProcessDebugAsync(nameof(DebuggerMiddleware), debug.Split(',')).ConfigureAwait(true);
                session.AddDebugInfo("debug time", $"{(DateTime.Now - timeStart).TotalMilliseconds} ms.");
                await context.Response.WriteAsync("\r\n" + session.DebugInfoString).ConfigureAwait(true);
            }
        }
    }
}
