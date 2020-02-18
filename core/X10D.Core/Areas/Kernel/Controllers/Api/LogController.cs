using Microsoft.AspNetCore.Mvc;
using X10D.Infrastructure;

namespace X10D.Core.Areas.Kernel.Controllers.Api
{
    [ApiController]
    [Route("/kernel/api/[controller]")]
    public class LogController : ControllerBase
    {
        Services.IKernel Kernel { get; }
        public LogController(IActivator activator)
        {
            Kernel = activator.GetService<Services.IKernel>();
        }

        public string Get()
        {
            return Kernel.Log;
        }
    }
}
