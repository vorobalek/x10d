using Microsoft.AspNetCore.Http;
using System;
using System.Linq;
using System.Threading.Tasks;
using X10D.Core.Services;

namespace X10D.Core.Middleware
{
    internal sealed class KernelProtectionMiddleware
    {
        private readonly RequestDelegate _next;

        public KernelProtectionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public Task Invoke(HttpContext context, IKernelProtection kernelProtection)
        {
            if (!kernelProtection.IsReady)
            {
                var path = context.Request.Path;
                if (kernelProtection.SafeUrlPrefixes.All(prefix => !path.StartsWithSegments(prefix, StringComparison.InvariantCultureIgnoreCase)))
                {
                    context.Request.Path = kernelProtection.SafeRedirectUrl;
                }
            }
            return _next(context);
        }
    }
}
