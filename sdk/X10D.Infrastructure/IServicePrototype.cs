using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace X10D.Infrastructure
{
    public delegate void ServiceStateChangeEventHandler(object sender, ServiceStateChangeEventArgs args);

    public interface IServicePrototype
    {
        Guid UID { get; }
        DateTime CreationTime { get; }
        ServiceLifetime ServiceLifetime { get; }
        ServiceState State { get; }
        Task Prepare();
        Task Start();
        Task Stop();
        Task Flush();
        Task Block();
        string Log { get; }
        IServicePrototype AddOnStateChange(ServiceStateChangeEventHandler handler);
        IServicePrototype RemoveOnStateChange(ServiceStateChangeEventHandler handler);
    }
}
