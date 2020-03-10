using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace X10D.Infrastructure.Components.Debugger
{
    internal sealed class AllTypesDebuggerComponent
    {
        public string Key => "all_types";
        public string Description => "Show all types of all Assemblies. Example: ?debug=all_types or ?debug=all_types,all_types_for_core for get all types of concrete assembly.";
        public void Invoke(IDebuggerSessionFacade session, IActivator activator, IHttpContextAccessor contextAccessor)
        {
            var assemblies = activator.Assemblies;
            var names = contextAccessor.HttpContext.Request.Query[Constants.Debug][0].Split(',').Where(key => key.StartsWith("all_types_for_", StringComparison.InvariantCultureIgnoreCase));
            if (names.Count() > 0)
            {
                foreach (var name in names)
                {
                    var assemblyName = name.Substring("all_types_for_".Length);
                    GetAllTypesForAssemblies(assemblies.Where(assembly => assembly.GetName().Name?.Contains(assemblyName, StringComparison.InvariantCultureIgnoreCase) ?? false), session);
                }
            }
            else
            {
                GetAllTypesForAssemblies(assemblies, session);
            }
        }

        private Func<Type, string> Classificate = (type) =>
                    $"{(type.isPublic() ? "public\t\t|" : "")}" +
                    $"{(type.isInternal() ? "internal\t|" : "")}" +
                    // Тут костыль с определением типов, на самом деле бывают ещё private protected и другие. 
                    $"{(!type.isPublic() && !type.isInternal() && !type.isPrivate() ? "protected\t|" : "")}" +
                    $"{(type.isPrivate() ? "private\t\t|" : "")}" +

                    $"{(type.isStatic() ? "static\t\t|" : "\t\t|")}" +
                    $"{(type.isSealed() ? "sealed\t\t|" : "\t\t|")}" +
                    $"{(type.isAbstract() ? "abstract\t|" : "\t\t|")}" +
                    $"{(type.isStruct() ? $"struct\t\t|" : "")}" +
                    $"{(type.isAsyncMethod() ? $"async-metod\t|" : "")}" +
                    $"{(type.isEnum() ? $"enum\t\t|" : "")}" +
                    $"{(type.isClass() ? $"class\t\t|" : "")}" +
                    $"{(type.isInterface() ? "interface\t|" : "")}" +
                    $"{(!type.isStruct() && !type.isEnum() && !type.isAsyncMethod() && !type.isClass() && !type.isInterface() ? "unknown\t|" : "")}" +
                    $"{type.GetFullName()}";

        private void GetAllTypesForAssemblies(IEnumerable<Assembly> assemblies, IDebuggerSessionFacade session)
        {
            foreach (var assembly in assemblies)
            {
                var publicTypes = new List<Type>();
                var internalTypes = new List<Type>();
                var protectedTypes = new List<Type>();
                var privateTypes = new List<Type>();

                foreach (var type in assembly.GetTypes())
                {
                    if (type.isPublic())
                    {
                        publicTypes.Add(type);
                    }
                    else if (type.isInternal())
                    {
                        internalTypes.Add(type);
                    }
                    else if (type.isPrivate())
                    {
                        privateTypes.Add(type);
                    }
                    // Тут костыль с определением типов, на самом деле бывают ещё private protected и другие. 
                    else
                    {
                        protectedTypes.Add(type);
                    }
                }

                foreach (var t in publicTypes)
                {
                    session.AddDebugInfo($"{Key} ({assembly.FullName})", Classificate(t));
                }
                foreach (var t in internalTypes)
                {
                    session.AddDebugInfo($"{Key} ({assembly.FullName})", Classificate(t));
                }
                foreach (var t in protectedTypes)
                {
                    session.AddDebugInfo($"{Key} ({assembly.FullName})", Classificate(t));
                }
                foreach (var t in privateTypes)
                {
                    session.AddDebugInfo($"{Key} ({assembly.FullName})", Classificate(t));
                }
            }
        }
    }
}
