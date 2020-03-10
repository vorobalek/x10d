using Microsoft.AspNetCore.Mvc;
using X10D.Core.Services;
using X10D.Infrastructure;

namespace X10D.Core.Api.Public.Kernel
{
    public class TokenController : KernelApiControllerBase
    {
        public TokenController(IActivator activator) : base(activator)
        {
        }

        [HttpGet]
        public IActionResult Get()
        {
            Activator.GetService<IKernel>().LogToken();
            return Ok("done", "The token was printed in console.");
        }

        [HttpPost]
        public IActionResult Post([FromQuery] string token)
        {
            if (Activator.GetService<IKernelFacade>().ValidateToken(token))
            {
                return Ok("ok", "Token is valid");
            }
            else
            {
                return Unauthorized("fail", "Token is invalid");
            }
        }
    }
}
