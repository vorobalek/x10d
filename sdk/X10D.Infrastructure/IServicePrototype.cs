using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace X10D.Infrastructure
{
    public delegate void ServiceOnBeforeStateChangeFrom(ServiceState state);
    public delegate void ServiceOnBeforeStateChangeTo(ServiceState state);
    public delegate void ServiceOnAfterStateChangeFrom(ServiceState state);
    public delegate void ServiceOnAfterStateChangeTo(ServiceState state);

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
        IServicePrototype AddOnBeforeStateChange(ServiceOnBeforeStateChangeFrom onBeforeStateChangeFrom, ServiceOnBeforeStateChangeTo onBeforeStateChangeTo);
        IServicePrototype AddOnAfterStateChange(ServiceOnAfterStateChangeFrom onAfterStateChangeFrom, ServiceOnAfterStateChangeTo onAfterStateChangeTo);
        IServicePrototype RemoveOnBeforeStateChange(ServiceOnBeforeStateChangeFrom onBeforeStateChangeFrom, ServiceOnBeforeStateChangeTo onBeforeStateChangeTo);
        IServicePrototype RemoveOnAfterStateChange(ServiceOnAfterStateChangeFrom onAfterStateChangeFrom, ServiceOnAfterStateChangeTo onAfterStateChangeTo);
    }
}
