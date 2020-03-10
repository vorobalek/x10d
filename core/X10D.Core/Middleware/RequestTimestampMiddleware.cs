using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;
using X10D.Infrastructure;

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
            context.Items.Add(Constants.RequestStartedOn, requestStartedOn);
            
            await _next(context).ConfigureAwait(true);
            
            var requestFinishedOn = DateTime.Now;
            context.Items.Add(Constants.RequestFinishedOn, requestFinishedOn);

            var request_time = requestFinishedOn - requestStartedOn;
            context.Items.Add(Constants.RequestTime, request_time);
        }
    }
}
