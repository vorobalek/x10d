using System.Collections.Generic;

namespace X10D.Core.Services
{
    internal interface IKernelProtection
    {
        string SafeRedirectUrl { get; set; }
        IList<string> SafeUrlPrefixes { get; }
        bool IsReady { get; }
    }
}