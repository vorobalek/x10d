using System.Threading.Tasks;

namespace X10D.Infrastructure
{
    public interface IDebuggerFacade : IServicePrototype
    {
        Task<IDebuggerSessionFacade> ProcessDebugAsync(string[] keys);
    }
}