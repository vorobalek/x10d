using Microsoft.AspNetCore.Mvc;
using X10D.Infrastructure;

namespace X10D.Core.Api.Secure
{
    [SecureApiRoute("kernel")]
    public abstract class KernelSecureApiControllerBase : SecureApiControllerBase
    {
        public KernelSecureApiControllerBase(IActivator activator) : base(activator)
        {
        }
    }
}
