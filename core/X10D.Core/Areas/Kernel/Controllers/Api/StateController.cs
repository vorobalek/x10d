using Microsoft.AspNetCore.Mvc;
using X10D.Core.Services;
using X10D.Infrastructure;

namespace X10D.Core.Areas.Kernel.Controllers.Api
{
    [ApiController]
    [Route("/kernel/api/[controller]")]
    public class StateController : ControllerBase
    {
        IKernel Kernel { get; }
        IAssemblyProvider AssemblyProvider { get; }
        public StateController(IActivator activator)
        {
            Kernel = activator.GetService<IKernel>();
            AssemblyProvider = activator.GetService<IAssemblyProvider>();
        }

        public string Get()
        {
            return $"{Kernel.State}";
        }

#if DEBUG
        [Route("restart")]
        public async void Restart()
        {
            await Kernel.Stop().ConfigureAwait(true);
            await Kernel.Flush().ConfigureAwait(true);

            //AssemblyProvider.ModulesContext.Unload();
            
            await Kernel.Prepare().ConfigureAwait(true);
            await Kernel.Start().ConfigureAwait(true);
        }
#endif
    }
}
