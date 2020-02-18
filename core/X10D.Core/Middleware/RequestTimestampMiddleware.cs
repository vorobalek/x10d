using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;

namespace X10D.Core.Middleware
{
    internal sealed class RequestTimestampMiddleware
    {
        private readonly RequestDelegate _next;

        public RequestTimestampMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            var requestStartedOn = DateTime.Now;
            context.Items.Add("__request_started_on__", requestStartedOn);
            
            await _next(context).ConfigureAwait(true);
            
            var requestFinishedOn = DateTime.Now;
            context.Items.Add("__request_finished_on__", requestFinishedOn);

            var request_time = requestFinishedOn - requestStartedOn;
            context.Items.Add("__request_time__", request_time);
        }
    }
}
