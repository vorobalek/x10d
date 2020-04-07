using Microsoft.AspNetCore.Mvc;
using X10D.Infrastructure;

namespace X10D.Contest.Api.Secure
{
    [SecureApiRoute("contest")]
    public class ContestSecureApiControllerBase : SecureApiControllerBase
    {
        public ContestSecureApiControllerBase(IActivator activator) : base(activator)
        {
        }
    }
}
