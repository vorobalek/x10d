using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Routing;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using X10D.Infrastructure;

namespace X10D.Mvc.Components.Debugger
{
    internal sealed class BackendEndpoindsDebuggerComponent
    {
        public string Key => "backend_endpoints";
        public string Description => "Show all backend mapped system endpoints. Example: ?debug=all_sessions";
        public void Invoke(IDebuggerSessionFacade session, IActionDescriptorCollectionProvider actionDescriptorCollectionProvider)
        {
            var items = actionDescriptorCollectionProvider
                .ActionDescriptors
                .Items
                .Where(descriptor => descriptor is ControllerActionDescriptor controllerActionDescriptor &&
                (!controllerActionDescriptor.ControllerTypeInfo.GetCustomAttribute<ApiRouteAttribute>()?.IsHidden ?? true) &&
                (!controllerActionDescriptor.ControllerTypeInfo.GetCustomAttribute<SecureApiRouteAttribute>()?.IsHidden ?? true))
                .Select(descriptor => (ControllerActionDescriptor)descriptor)
                .GroupBy(descriptor => descriptor.ControllerTypeInfo.FullName)
                .ToList();
            var endpoints = new List<string>();
            foreach (var actionDescriptors in items)
            {
                if (!actionDescriptors.Any())
                    continue;

                var actionDescriptor = actionDescriptors.First();
                var controllerTypeInfo = actionDescriptor.ControllerTypeInfo;

                foreach (var descriptor in actionDescriptors.GroupBy(a => a.ActionName).Select(g => g.First()))
                {
                    var methodInfo = descriptor.MethodInfo;
                    var httpMethod = "ANY";
                    if (descriptor.EndpointMetadata.FirstOrDefault(obj => obj.GetType() == typeof(HttpMethodMetadata)) is HttpMethodMetadata httpMethodMetadata)
                    {
                        httpMethod = string.Join(", ", httpMethodMetadata.HttpMethods);
                    }
                    endpoints.Add($"{(IsProtectedAction(controllerTypeInfo, methodInfo) ? "*" : "-")}\t{httpMethod}:\t{descriptor.AttributeRouteInfo.Template.ToLower()}");
                }
            }
            endpoints
                .OrderBy(endpoint => endpoint)
                .ToList()
                .ForEach(endpoint => session.AddDebugInfo(Key, endpoint));
        }

        private bool IsProtectedAction(MemberInfo controllerTypeInfo, MemberInfo actionMethodInfo)
        {
            if (actionMethodInfo.GetCustomAttribute<AllowAnonymousAttribute>(true) != null)
                return false;

            if (controllerTypeInfo.GetCustomAttribute<AuthorizeAttribute>(true) != null)
                return true;

            if (actionMethodInfo.GetCustomAttribute<AuthorizeAttribute>(true) != null)
                return true;

            if (controllerTypeInfo.GetCustomAttribute<SecureApiRouteAttribute>(true) != null)
                return true;

            return false;
        }
    }
}
