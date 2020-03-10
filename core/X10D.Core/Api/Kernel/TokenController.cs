using Microsoft.AspNetCore.Mvc;
using X10D.Core.Services;
using X10D.Infrastructure;

namespace X10D.Core.Api.Kernel
{
    public class TokenController : KernelApiControllerBase
    {
        public TokenController(IActivator activator) : base(activator)
        {
        }

        [HttpPost]
        public IActionResult Post([FromQuery] string token)
        {
            if (Activator.GetService<IKernel>().ValidateToken(token))
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
