using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace X10D.Infrastructure
{
    public abstract class ServicePrototype : IServicePrototype
    {
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
                    Critical(() => threadId = Guid.NewGuid(), threadId);
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
                    Critical(() => threadId = Guid.NewGuid(), threadId);
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
                    Critical(() => threadId = Guid.NewGuid(), threadId);
                    thread = null;
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
        protected ThreadStart ThreadStart(Action action) => () => action();
        protected void CriticalWhile(Func<bool> condition, Action action)
        {
            var currentThreadId = Guid.NewGuid();
            Critical(() => threadId = currentThreadId, threadId);

            while (condition() && Critical(() => threadId is Guid guid && guid == currentThreadId, threadId))
            {
                action();
            }
        }
        protected void CriticalDoWhile(Func<bool> condition, Action action)
        {
            var currentThreadId = Guid.NewGuid();
            Critical(() => threadId = currentThreadId, threadId);

            do
            {
                action();
            }
            while (condition() && Critical(() => threadId is Guid guid && guid == currentThreadId, threadId));
        }

        protected void CriticalForEach<T>(IEnumerable<T> enumerable, Action<T> action)
        {
            var currentThreadId = Guid.NewGuid();
            Critical(() => threadId = currentThreadId, threadId);

            foreach (var item in enumerable)
            {
                if (Critical(() => threadId is Guid guid && guid == currentThreadId, threadId))
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

        private event ServiceStateChangeEventHandler ServiceChangeStateEvent;
        private object threadId;
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
        private Thread thread;
    }
}
