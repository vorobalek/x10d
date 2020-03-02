using Microsoft.AspNetCore.Mvc;
using X10D.Core.Services;
using X10D.Infrastructure;

namespace X10D.Core.Api
{
    [ApiController]
    [Route("/api/kernel.[controller]")]
    public class LogController : ControllerBase
    {
        IKernel Kernel { get; }
        public LogController(IActivator activator)
        {
            Kernel = activator.GetService<IKernel>();
        }

        public string Get()
        {
            return Kernel.Log;
        }
    }
}
