using System;

namespace X10D.Infrastructure
{
    public class ServiceStateChangeEventArgs : EventArgs
    {
        public ServiceState OldValue { get; }
        public ServiceState NewValue { get; }
        public bool StateChanged { get; }

        public ServiceStateChangeEventArgs(ServiceState oldValue, ServiceState newValue, bool stateChanged)
        {
            OldValue = oldValue;
            NewValue = newValue;
            StateChanged = stateChanged;
        }
    }
}