using Microsoft.AspNetCore.Mvc;
using X10D.Core.Services;
using X10D.Infrastructure;

namespace X10D.Core.Api.Secure.Kernel
{
    public class TokenController : KernelSecureApiControllerBase
    {
        public TokenController(IActivator activator) : base(activator)
        {
        }

        [HttpPut]
        public IActionResult Put()
        {
            Activator.GetService<IKernel>().ChangeToken();
            return Ok("done", "New token was printed in console.");
        }
    }
}
