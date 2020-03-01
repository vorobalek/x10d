using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace X10D.Infrastructure
{
    public abstract class ServicePrototype : IServicePrototype
    {
        Type IServicePrototype.InterfaceType => null;
        Guid IServicePrototype.UID { get; } = Guid.NewGuid();
        DateTime IServicePrototype.CreationTime { get; } = DateTime.Now;

        Task IServicePrototype.Prepare()
        {
            return Task.Run(() =>
            {
                IfStable(() => SetState(ServiceState.Preparing));
                IfStable(() =>
                {
                    PrepareService();
                    if (Process != null)
                    {
                        process = new Task(() =>
                        {
                            Process();
                        }, TaskCreationOptions.LongRunning);
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
                    if (process != null)
                    {
                        process.Start();
                    }
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
                    Critical(() => processId = Guid.NewGuid(), processId);
                    if (process != null)
                    {
                        process.Wait();
                        process.Dispose();
                    }
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
                    Critical(() => processId = Guid.NewGuid(), processId);
                    if (process != null)
                    {
                        process.Wait();
                    }
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
                    Critical(() => processId = Guid.NewGuid(), processId);
                    if (process != null)
                    {
                        process?.Wait();
                    }
                });
                AnyWay(() => SetState(ServiceState.Blocked));
            });
        }
        IServicePrototype IServicePrototype.AddOnStateChange(ServiceStateChangeEventHandler handler)
        {
            ServiceChangeStateEvent += handler;
            return this;
        }
        IServicePrototype IServicePrototype.RemoveOnStateChange(ServiceStateChangeEventHandler handler)
        {
            ServiceChangeStateEvent -= handler;
            return this;
        }

        public virtual int? LoadPriority => 0;
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
        protected virtual Action Process { get; } = null;
        protected ThreadStart ThreadStart(Action action) => () => action();
        protected void CriticalWhile(Func<bool> condition, Action action)
        {
            var currentThreadId = Guid.NewGuid();
            Critical(() => processId = currentThreadId, processId);

            while (condition() && Critical(() => processId is Guid guid && guid == currentThreadId, processId))
            {
                action();
            }
        }
        protected void CriticalDoWhile(Func<bool> condition, Action action)
        {
            var currentThreadId = Guid.NewGuid();
            Critical(() => processId = currentThreadId, processId);

            do
            {
                action();
            }
            while (condition() && Critical(() => processId is Guid guid && guid == currentThreadId, processId));
        }

        protected void CriticalForEach<T>(IEnumerable<T> enumerable, Action<T> action)
        {
            var currentThreadId = Guid.NewGuid();
            Critical(() => processId = currentThreadId, processId);

            foreach (var item in enumerable)
            {
                if (Critical(() => processId is Guid guid && guid == currentThreadId, processId))
                {
                    action(item);
                }
                else
                {
                    break;
                }
            }
        }
        protected void IfStable(Action action)
        {
            IfStable<object>(() => { action(); return null; });
        }
        protected T IfStable<T>(Func<T> func)
        {
            if (IsStable)
            {
                return func();
            }
            return default;
        }
        protected void IfNotStable(Action action)
        {
            IfNotStable<object>(() => { action(); return null; });
        }
        protected T IfNotStable<T>(Func<T> func)
        {
            if (!IsStable)
            {
                return func();
            }
            return default;
        }
        protected void AnyWay(Action action)
        {
            AnyWay<object>(() => { action(); return null; });
        }
        protected T AnyWay<T>(Func<T> func)
        {
            return func();
        }
        protected void Critical(Action action, object locker = null)
        {
            Critical<object>(() => { action(); return null; }, locker);
        }
        protected T Critical<T>(Func<T> func, object locker = null)
        {
            lock (locker ?? baseLocker)
            {
                return func();
            }
        }

        protected Task Prepare() => (this as IServicePrototype).Prepare();
        protected Task Start() => (this as IServicePrototype).Stop();
        protected Task Stop() => (this as IServicePrototype).Stop();
        protected Task Flush() => (this as IServicePrototype).Flush();
        protected Task Block() => (this as IServicePrototype).Block();
        protected IServicePrototype AddOnStateChange(ServiceStateChangeEventHandler handler) => (this as IServicePrototype).AddOnStateChange(handler);
        protected IServicePrototype RemoveOnStateChange(ServiceStateChangeEventHandler handler) => (this as IServicePrototype).RemoveOnStateChange(handler);

        private event ServiceStateChangeEventHandler ServiceChangeStateEvent;
        private Task process;
        private object processId;
        private readonly object baseLocker = new object();
        private bool IsStable =>
            State != ServiceState.Blocking
            && State != ServiceState.Blocked;

        private void SetState(ServiceState newValue)
        {
            Critical(() =>
            {
                var oldValue = State;
                ServiceChangeStateEvent?.Invoke(this, new ServiceStateChangeEventArgs(oldValue, newValue, false));
                State = newValue;
                ServiceChangeStateEvent?.Invoke(this, new ServiceStateChangeEventArgs(oldValue, newValue, true));
            }, State);
        }

        public virtual void Dispose()
        {
            Stop().Wait();
            Flush().Wait();
        }
    }

    public abstract class ServicePrototype<TService> : ServicePrototype, IServicePrototype
    {
        Type IServicePrototype.InterfaceType => typeof(TService);
    }
}
