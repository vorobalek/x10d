using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Loader;

namespace X10D.Core
{
    internal interface IAssemblyProvider : ExtCore.WebApplication.IAssemblyProvider
    {
        AssemblyLoadContext CoreContext { get; }
        IList<Assembly> CoreAssemblies { get; }
        AssemblyLoadContext ModulesContext { get; }
        IList<Assembly> ModulesAssemblies { get; }
    }
}