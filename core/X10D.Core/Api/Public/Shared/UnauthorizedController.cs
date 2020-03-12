using Microsoft.AspNetCore.Mvc;
using X10D.Infrastructure;

namespace X10D.Core.Api.Public.Shared
{
    [ApiRoute(IsHidden = true)]
    public class UnauthorizedController : ApiControllerBase
    {
        public UnauthorizedController(IActivator activator) : base(activator)
        {
        }

        public IActionResult Get()
        {
            return Unauthorized("Unauthorized", 
                "It looks like you are trying to access the API system area, but your token doesn't match. " +
                "Check that your token is correct and try again.");
        }
    }
}
