using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using X10D.Infrastructure;

namespace X10D.Core.Middleware
{
    internal sealed class TokenSidePolicyMiddleware
    {
        private readonly RequestDelegate _next;

        public TokenSidePolicyMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context, IKernelFacade kernel)
        {
            var segments = context.Request.Path.Value.Split("/");
            if (segments.Length > 3 && kernel.ValidateToken(segments[3]))
            {
                if (!context.User.HasClaim(Constants.TokenSidePolicy, ""))
                {
                    context.User.AddIdentity(new ClaimsIdentity(new[] { new Claim(Constants.TokenSidePolicy, "") }));
                }
            }
            await _next(context).ConfigureAwait(true);
        }
    }
}
