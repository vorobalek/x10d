using Microsoft.AspNetCore.Mvc;
using X10D.Infrastructure;

namespace X10D.Core.Api.Kernel
{
    public class StateController : KernelApiControllerBase
    {
        public StateController(IActivator activator) : base(activator)
        {
        }

        public IActionResult Get([FromServices] IKernelFacade kernel)
        {
            return Ok((int)kernel.State, $"{kernel.State}");
        }
    }
}
