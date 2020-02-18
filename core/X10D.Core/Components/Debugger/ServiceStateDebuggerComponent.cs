using Microsoft.AspNetCore.Http;
using System;
using System.Linq;
using X10D.Core.Services;

namespace X10D.Core.Components.Debugger
{
    internal sealed class ServiceStateDebuggerComponent
    {
        public string Key => "service_state";
        public string Description => "Show the state of a specific service. Example: ?debug=service_state,service_state_for_kernel";
        public void Invoke(IDebuggerSession session, IKernel kernel, IHttpContextAccessor contextAccessor)
        {
            var names = contextAccessor.HttpContext.Request.Query["debug"][0].Split(',').Where(key => key.StartsWith("service_state_for_", StringComparison.InvariantCultureIgnoreCase));
            foreach (var name in names)
            {
                var serviceName = name.Substring("service_state_for_".Length);
                var services = kernel.Services.Where(service => service.GetType().Name.Contains(serviceName, StringComparison.InvariantCultureIgnoreCase));

                foreach (var service in services)
                {
                    session.AddDebugInfo($"{Key} ({serviceName})", $"{service.GetType().GetFullName()}\t{service.ServiceLifetime}\t{service.State}");
                }
            }
        }
    }
}
