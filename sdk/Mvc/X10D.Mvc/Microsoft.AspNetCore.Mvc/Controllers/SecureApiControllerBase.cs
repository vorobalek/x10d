using Microsoft.AspNetCore.Authorization;
using X10D.Infrastructure;

namespace Microsoft.AspNetCore.Mvc
{
    [SecureApiRoute]
    public class SecureApiControllerBase : ApiControllerBase
    {
        public string Token { get; set; }
        public SecureApiControllerBase(IActivator activator) : base(activator)
        {
        }
    }
}
