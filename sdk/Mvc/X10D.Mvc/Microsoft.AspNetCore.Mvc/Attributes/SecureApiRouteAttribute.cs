namespace Microsoft.AspNetCore.Mvc
{
    public class SecureApiRouteAttribute : RouteAttribute
    {
        public bool IsHidden { get; set; } = false;

        public SecureApiRouteAttribute(string area = "shared")
            : base($"/api/secure/{{Token}}/{{Format}}/{area}.[controller]")
        {
        }
    }
}
