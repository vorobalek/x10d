using Microsoft.AspNetCore.Http;
using System;
using System.Linq;
using X10D.Core.Services;
using X10D.Infrastructure;

namespace X10D.Core.Components.Debugger
{
    internal sealed class ServiceLogDebuggerComponent
    {
        public string Key => "service_log";
        public string Description => "Show the log of a specific service. Example: ?debug=service_log,service_log_for_kernel";

        public void Invoke(IDebuggerSession session, IKernel kernel, IHttpContextAccessor contextAccessor)
        {
            var names = contextAccessor.HttpContext.Request.Query[Constants.Debug][0].Split(',').Where(key => key.StartsWith("service_log_for_", StringComparison.InvariantCultureIgnoreCase));
            foreach (var name in names)
            {
                var serviceName = name.Substring("service_log_for_".Length);
                var services = kernel.Services.Where(service => service.GetType().Name.Contains(serviceName, StringComparison.InvariantCultureIgnoreCase));

                foreach (var service in services)
                {
                    session.AddDebugInfo($"{Key} ({serviceName})", $"{service.Log}");
                }   
            }
        }
    }
}
