namespace Microsoft.AspNetCore.Mvc
{
    public class ApiRouteAttribute : RouteAttribute
    {
        public ApiRouteAttribute(string area = "shared")
            : base($"/api/{{Format}}/{area}.[controller]")
        {
        }
    }
}
