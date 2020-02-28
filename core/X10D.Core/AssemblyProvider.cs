using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyModel;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using X10D.Infrastructure;

namespace X10D.Core
{
    internal sealed class AssemblyProvider : IAssemblyProvider
    {
        public AssemblyLoadContext CoreContext { get; private set; }
        public AssemblyLoadContext ModulesContext { get; private set; }
        public IList<Assembly> CoreAssemblies => CoreContext.Assemblies.ToList();
        public IList<Assembly> ModulesAssemblies => ModulesContext.Assemblies.ToList();

        public AssemblyProvider(IServiceProvider serviceProvider)
        {
            Logger = serviceProvider.GetService<ILoggerFactory>().CreateLogger("X10D.Core");

            isCandidateAssembly = assembly =>
                !assembly.FullName.StartsWith("System", StringComparison.OrdinalIgnoreCase) &&
                !assembly.FullName.StartsWith("Microsoft", StringComparison.OrdinalIgnoreCase);

            isCandidateCompilationLibrary = library =>
                !library.Name.StartsWith("mscorlib", StringComparison.OrdinalIgnoreCase) &&
                !library.Name.StartsWith("netstandard", StringComparison.OrdinalIgnoreCase) &&
                !library.Name.StartsWith("System", StringComparison.OrdinalIgnoreCase) &&
                !library.Name.StartsWith("Microsoft", StringComparison.OrdinalIgnoreCase) &&
                !library.Name.StartsWith("WindowsBase", StringComparison.OrdinalIgnoreCase);
        }

        public IEnumerable<Assembly> GetAssemblies(string path, bool includingSubpaths)
        {
            List<Assembly> assemblies = new List<Assembly>();
            CoreContext = AssemblyLoadContext.Default;
            ModulesContext = new AssemblyLoadContext($"{prefix}.ModulesContext.{iteration++}", true);

            CoreContext.Unloading += Context_Unloading;
            ModulesContext.Unloading += Context_Unloading;

            GetAssembliesFromDependencyContext(assemblies);
            GetAssembliesFromPath(assemblies, path, includingSubpaths);
            return assemblies;
        }

        private void Context_Unloading(AssemblyLoadContext context)
        {
            Logger.LogWarning($"Unloading {context.Name}");
        }

        private ILogger Logger { get; }
        private int iteration = 0;
        private readonly string prefix = "X10D";
        private readonly Func<Assembly, bool> isCandidateAssembly;
        private readonly Func<Library, bool> isCandidateCompilationLibrary;

        private void GetAssembliesFromDependencyContext(List<Assembly> assemblies)
        {
            Logger.LogTrace($"Discovering and loading {prefix} assemblies from DependencyContext");

            int sandBoxId = 0;
            foreach (CompilationLibrary compilationLibrary in DependencyContext.Default.CompileLibraries)
            {
                if (isCandidateCompilationLibrary(compilationLibrary))
                {
                    Assembly assembly = null;

                    try
                    {
                        var sandBox = new AssemblyLoadContext($"{prefix}.LibrarySandBox.{++sandBoxId}", true);
                        assembly = sandBox.LoadFromAssemblyName(new AssemblyName(compilationLibrary.Name));

                        if (!assemblies.Any(a => string.Equals(a.FullName, assembly.FullName, StringComparison.OrdinalIgnoreCase)))
                        {
                            if (compilationLibrary.Name.StartsWith(prefix, StringComparison.InvariantCultureIgnoreCase) &&
                                compilationLibrary.Name != Assembly.GetExecutingAssembly().GetName().Name &&
                                assembly.GetTypes().Any(type => type.GetBaseTypes().Contains(typeof(BaseMetadata)) && type != typeof(BaseMetadata)))
                            {
                                sandBox.Unload();
                                assembly = ModulesContext.LoadFromAssemblyName(new AssemblyName(compilationLibrary.Name));
                                Logger.LogInformation($"Assembly '{assembly.FullName}' is discovered and loaded");
                            }
                            else
                            {
                                sandBox.Unload();
                                assembly = CoreContext.LoadFromAssemblyName(new AssemblyName(compilationLibrary.Name));
                                Logger.LogTrace($"Assembly '{assembly.FullName}' is discovered and loaded");
                            }

                            assemblies.Add(assembly);
                        }
                        else
                        {
                            sandBox.Unload();
                        }
                    }

                    catch (Exception e)
                    {
                        Logger.LogTrace($"Error loading assembly '{compilationLibrary.Name}'");
                        Logger.LogTrace(e.ToString());
                    }
                }
            }
        }

        private void GetAssembliesFromPath(List<Assembly> assemblies, string path, bool includingSubpaths)
        {
            if (!string.IsNullOrEmpty(path) && Directory.Exists(path))
            {
                Logger.LogTrace($"Discovering and loading {prefix} assemblies from path '{path}'");

                int sandBoxId = 0;
                foreach (string extensionPath in Directory.EnumerateFiles(path, "*.dll"))
                {
                    Assembly assembly = null;

                    try
                    {
                        var sandBox = new AssemblyLoadContext($"{prefix}.AssemblySandBox.{++sandBoxId}", true);
                        assembly = sandBox.LoadFromAssemblyPath(extensionPath);

                        if (isCandidateAssembly(assembly) &&
                            !assemblies.Any(a => string.Equals(a.FullName, assembly.FullName, StringComparison.InvariantCultureIgnoreCase)))
                        {
                            if (assembly.FullName.StartsWith(prefix, StringComparison.InvariantCultureIgnoreCase) &&
                                assembly.GetTypes().Any(type => type.GetBaseTypes().Contains(typeof(BaseMetadata)) && type != typeof(BaseMetadata)))
                            {
                                sandBox.Unload();
                                assembly = ModulesContext.LoadFromAssemblyPath(extensionPath);
                                Logger.LogInformation($"Assembly '{assembly.FullName}' is discovered and loaded");
                            }
                            else
                            {
                                sandBox.Unload();
                                assembly = CoreContext.LoadFromAssemblyPath(extensionPath);
                                Logger.LogTrace($"Assembly '{assembly.FullName}' is discovered and loaded");
                            }

                            assemblies.Add(assembly);
                        }
                        else
                        {
                            sandBox.Unload();
                        }
                    }

                    catch (Exception e)
                    {
                        Logger.LogTrace($"Error loading assembly '{extensionPath}'");
                        Logger.LogTrace(e.ToString());
                    }
                }

                if (includingSubpaths)
                    foreach (string subpath in Directory.GetDirectories(path))
                        GetAssembliesFromPath(assemblies, subpath, includingSubpaths);
            }

            else
            {
                if (string.IsNullOrEmpty(path))
                {
                    Logger.LogTrace($"Discovering and loading {prefix} assemblies from path skipped: path not provided", path);
                }
                else
                {
                    Logger.LogTrace($"Discovering and loading {prefix} assemblies from path '{path}' skipped: path not found");
                }
            }
        }
    }
}
