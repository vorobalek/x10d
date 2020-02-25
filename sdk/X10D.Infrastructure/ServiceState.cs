namespace X10D.Infrastructure
{
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
}
