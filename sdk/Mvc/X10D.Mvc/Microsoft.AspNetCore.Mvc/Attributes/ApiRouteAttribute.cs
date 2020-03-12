namespace Microsoft.AspNetCore.Mvc
{
    public class ApiRouteAttribute : RouteAttribute
    {
        public bool IsHidden { get; set; } = false;

        public ApiRouteAttribute(string area = "shared")
            : base($"/api/{{Format}}/{area}.[controller]")
        {
        }
    }
}
