using Microsoft.AspNetCore.Http;
using System;
using X10D.Core.Services;

namespace X10D.Core.Components.Debugger
{
    internal sealed class RequestTimeDebuggerComponent
    {
        public string Key => "request_time";
        public string Description => "Show the query processing time. Example: ?debug=request_time";

        public void Invoke(IDebuggerSession session, IHttpContextAccessor contextAccessor)
        {
            if (contextAccessor.HttpContext.Items.TryGetValue("__request_time__", out var requestTimeObj) && requestTimeObj is TimeSpan requestTime)
            {
                session.AddDebugInfo(Key, $"{requestTime.TotalMilliseconds} ms.");
            }
            else
            {
                session.AddDebugInfo(Key, $"unknown");
            }
        }
    }
}
