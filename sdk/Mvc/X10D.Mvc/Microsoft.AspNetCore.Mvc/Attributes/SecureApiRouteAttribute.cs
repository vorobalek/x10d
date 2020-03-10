namespace Microsoft.AspNetCore.Mvc
{
    public class SecureApiRouteAttribute : RouteAttribute
    {
        public SecureApiRouteAttribute(string area = "shared")
            : base($"/api/{{Format}}/{{Token}}/{area}.[controller]")
        {
        }
    }
}
