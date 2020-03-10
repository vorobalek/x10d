using System.Threading.Tasks;

namespace X10D.Infrastructure
{
    public interface IDebuggerFacade<TSession>
        where TSession : IDebuggerSessionFacade
    {
        Task<TSession> ProcessDebugAsync(string[] keys);
        Task<TSession> ProcessDebugAsync(string session_name, string[] keys);
    }

    public interface IDebuggerFacade : IDebuggerFacade<IDebuggerSessionFacade> { }
}