﻿using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;

namespace X10D.Infrastructure
{
    public delegate void ServiceOnBeforeStateChangeFrom(ServiceState state);
    public delegate void ServiceOnBeforeStateChangeTo(ServiceState state);
    public delegate void ServiceOnAfterStateChangeFrom(ServiceState state);
    public delegate void ServiceOnAfterStateChangeTo(ServiceState state);

    public enum ServiceState
    {
        Unknown,
        Idles,
        Preparing,
        Prepared,
        Running,
        InProgress,
        Stopping,
        Stopped,
        Flushing,
        Flushed,
        Blocking,
        Blocked,
    }

    public interface IServicePrototype
    {
        ServiceLifetime ServiceLifetime { get; }
        ServiceState State { get; }
        Task Prepare();
        Task Start();
        Task Stop();
        Task Flush();
        Task Block();
        string Log { get; }
        IServicePrototype OnBeforeStateChange(ServiceOnBeforeStateChangeFrom onBeforeStateChangeFrom, ServiceOnBeforeStateChangeTo onBeforeStateChangeTo);
        IServicePrototype OnAfterStateChange(ServiceOnAfterStateChangeFrom onAfterStateChangeFrom, ServiceOnAfterStateChangeTo onAfterStateChangeTo);
    }
}
