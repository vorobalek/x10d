using Microsoft.Extensions.DependencyInjection;
using System;

namespace X10D.Infrastructure.Components.Debugger
{
    internal sealed class AllServicesDebuggerComponent
    {
        public string Key => "all_services";
        public string Description => "Get all services. Example: ?debug=all_services";
        public void Invoke(IDebuggerSessionFacade session, IServiceProvider serviceProvider)
        {
            foreach (var prototype in serviceProvider.GetServices<IServicePrototype>())
            {
                session.AddDebugInfo(Key, $"{prototype.GetType().GetFullName()}\t{prototype.ServiceLifetime}\t{prototype.State}");
            }
        }
    }
}
