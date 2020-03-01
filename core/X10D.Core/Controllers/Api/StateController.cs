using Microsoft.AspNetCore.Mvc;
using X10D.Core.Services;
using X10D.Infrastructure;

namespace X10D.Core.Areas.Kernel.Controllers.Api
{
    [ApiController]
    [Route("/api/kernel.[controller]")]
    public class StateController : ControllerBase
    {
        IKernel Kernel { get; }
        public StateController(IActivator activator)
        {
            Kernel = activator.GetService<IKernel>();
        }

        public string Get()
        {
            return $"{Kernel.State}";
        }
    }
}
