using Padutronics.IO.Paths;
using System;
using System.Reflection;

namespace Padutronics.DependencyInjection.Scanning.Fluent;

public interface IAssemblyStage
{
    IConventionStage AssembliesFromPath(DirectoryPath path);
    IConventionStage AssembliesFromPath(DirectoryPath path, bool includeExecutables);
    IConventionStage Assembly(string assemblyName);
    IConventionStage Assembly(Assembly assembly);
    IConventionStage AssemblyContaining(Type type);
    IConventionStage AssemblyContaining<T>();
}