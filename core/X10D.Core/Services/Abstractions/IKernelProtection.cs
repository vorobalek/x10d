using System.Collections.Generic;
using X10D.Infrastructure;

namespace X10D.Core.Services
{
    internal interface IKernelProtection : IServicePrototype
    {
        string SafeRedirectUrl { get; set; }
        IList<string> SafeUrlPrefixes { get; }
        bool IsReady { get; }
    }
}