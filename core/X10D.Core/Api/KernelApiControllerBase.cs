using Microsoft.AspNetCore.Mvc;
using X10D.Infrastructure;

namespace X10D.Core.Api
{
    [ApiRoute("kernel")]
    public abstract class KernelApiControllerBase : ApiControllerBase
    {
        public KernelApiControllerBase(IActivator activator) : base(activator)
        {
        }
    }
}
