using System.Threading.Tasks;

namespace X10D.Infrastructure
{
    public interface IDebuggerFacade
    {
        Task<IDebuggerSessionFacade> ProcessDebugAsync(string[] keys);
        Task<IDebuggerSessionFacade> ProcessDebugAsync(string session_name, string[] keys);
    }
}