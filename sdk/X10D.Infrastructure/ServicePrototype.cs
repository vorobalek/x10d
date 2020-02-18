using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace X10D.Infrastructure
{
    public abstract class ServicePrototype : IServicePrototype
    {
        Task IServicePrototype.Prepare()
        {
            return Task.Run(() =>
            {
                IfStable(() => SetState(ServiceState.Preparing));
                IfStable(() =>
                {
                    PrepareService();
                    if (MainProcess != null)
                    {
                        thread = new Thread(MainProcess)
                        {
                            IsBackground = true,
                            Name = $"{GetType().GetFullName()}_MainThread"
                        };
                    }
                });
                IfStable(() => SetState(ServiceState.Prepared));
            });
        }
        Task IServicePrototype.Start()
        {
            return Task.Run(() =>
            {
                IfStable(() => SetState(ServiceState.Running));
                IfStable(() =>
                {
                    StartService();
                    thread?.Start();
                });
                IfStable(() => SetState(ServiceState.InProgress));
            });
        }
        Task IServicePrototype.Flush()
        {
            return Task.Run(() =>
            {
                AnyWay(() => SetState(ServiceState.Flushing));
                AnyWay(() =>
                {
                    FlushService();
                    thread = null;
                });
                AnyWay(() => SetState(ServiceState.Flushed));
            });
        }
        Task IServicePrototype.Stop()
        {
            return Task.Run(() =>
            {
                IfStable(() => SetState(ServiceState.Stopping));
                IfStable(() =>
                {
                    StopService();
                    thread = null;
                });
                IfStable(() => SetState(ServiceState.Stopped));
            });
        }
        Task IServicePrototype.Block()
        {
            return Task.Run(() =>
            {
                AnyWay(() => SetState(ServiceState.Blocking));
                AnyWay(() =>
                {
                    BlockService();
                    thread = null;
                });
                AnyWay(() => SetState(ServiceState.Blocked));
            });
        }
        IServicePrototype IServicePrototype.OnBeforeStateChange(ServiceOnBeforeStateChangeFrom onBeforeStateChangeFrom, ServiceOnBeforeStateChangeTo onBeforeStateChangeTo)
        {
            BeforeStateChangeFrom += onBeforeStateChangeFrom;
            BeforeStateChangeTo += onBeforeStateChangeTo;
            return this;
        }
        IServicePrototype IServicePrototype.OnAfterStateChange(ServiceOnAfterStateChangeFrom onAfterStateChangeFrom, ServiceOnAfterStateChangeTo onAfterStateChangeTo)
        {
            AfterStateChangeFrom += onAfterStateChangeFrom;
            AfterStateChangeTo += onAfterStateChangeTo;
            return this;
        }
        public abstract ServiceLifetime ServiceLifetime { get; }
        public ServiceState State { get; private set; } = ServiceState.Unknown;
        public virtual string Log => $"{GetType().GetFullName()}\t{ServiceLifetime}\t{State}";
        protected void Idle()
        {
            IfStable(() => SetState(ServiceState.Idles));
        }
        protected virtual void PrepareService() { }
        protected virtual void StartService() { }
        protected virtual void FlushService() { }
        protected virtual void StopService() { }
        protected virtual void BlockService() { }
        protected virtual ThreadStart MainProcess { get; } = null;
        protected void IfStable(Action action)
        {
            if (IsStable)
            {
                action();
            }
        }
        protected void IfNotStable(Action action)
        {
            if (!IsStable)
            {
                action();
            }
        }
        protected void AnyWay(Action action)
        {
            action();
        }

        private bool IsStable =>
            State != ServiceState.Blocking
            && State != ServiceState.Blocked;
        private event ServiceOnBeforeStateChangeFrom BeforeStateChangeFrom;
        private event ServiceOnBeforeStateChangeTo BeforeStateChangeTo;
        private event ServiceOnAfterStateChangeFrom AfterStateChangeFrom;
        private event ServiceOnAfterStateChangeTo AfterStateChangeTo;
        private void SetState(ServiceState value)
        {
            BeforeStateChangeFrom?.Invoke(State);
            BeforeStateChangeTo?.Invoke(value);
            var old_state = State;
            State = value;
            AfterStateChangeFrom?.Invoke(old_state);
            AfterStateChangeTo?.Invoke(State);
        }
        private Thread thread;
    }
}
