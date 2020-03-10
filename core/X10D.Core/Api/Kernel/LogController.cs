using Microsoft.AspNetCore.Mvc;
using X10D.Infrastructure;

namespace X10D.Core.Api.Kernel
{
    public class LogController : KernelApiControllerBase
    {
        public LogController(IActivator activator) : base(activator)
        {
        }

        [HttpGet]
        public IActionResult Get([FromServices] IKernelFacade kernel)
        {
            return Ok(kernel.Log);
        }
    }
}
