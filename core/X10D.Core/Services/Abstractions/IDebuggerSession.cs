using X10D.Infrastructure;

namespace X10D.Core.Services
{
    internal interface IDebuggerSession : IDebuggerSessionFacade, IServicePrototype
    {
        string DebugInfoString { get; }
    }
}
