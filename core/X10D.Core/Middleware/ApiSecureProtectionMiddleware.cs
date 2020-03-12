using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;
using X10D.Infrastructure;

namespace X10D.Core.Middleware
{
    internal sealed class ApiSecureProtectionMiddleware
    {
        private readonly RequestDelegate _next;

        public ApiSecureProtectionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context, IKernelFacade kernel)
        {
            if (context.Request.Path.StartsWithSegments("/api/secure", StringComparison.InvariantCultureIgnoreCase))
            {
                var segments = context.Request.Path.Value.Split("/");
                if (segments.Length < 4 || !kernel.ValidateToken(segments[3]))
                {
                    var format = segments[4];
                    context.Request.Path = $"/api/{format}/shared.unauthorized";
                }
            }
            await _next(context).ConfigureAwait(true);
        }
    }
}
