using Microsoft.AspNetCore.Mvc;
using X10D.Infrastructure;

namespace X10D.Contest.Api.Public
{
    [ApiRoute("contest")]
    public abstract class ContestApiControllerBase : ApiControllerBase
    {
        public ContestApiControllerBase(IActivator activator) : base(activator)
        {
        }
    }
}
