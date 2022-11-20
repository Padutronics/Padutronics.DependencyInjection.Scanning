using Padutronics.IO.Paths;
using System;
using System.Reflection;

namespace Padutronics.DependencyInjection.Scanning.Fluent;

public interface IAssemblyStage
{
    IAssemblyWithFilterStage AssembliesFromPath(DirectoryPath path);
    IAssemblyWithFilterStage AssembliesFromPath(DirectoryPath path, Action<IAssemblyConfigurator> configurationCallback);
    IAssemblyWithFilterStage AssembliesFromPath(DirectoryPath path, bool includeExecutables);
    IAssemblyWithFilterStage AssembliesFromPath(DirectoryPath path, bool includeExecutables, Action<IAssemblyConfigurator> configurationCallback);
    IAssemblyWithFilterStage Assembly(string assemblyName);
    IAssemblyWithFilterStage Assembly(string assemblyName, Action<IAssemblyConfigurator> configurationCallback);
    IAssemblyWithFilterStage Assembly(Assembly assembly);
    IAssemblyWithFilterStage Assembly(Assembly assembly, Action<IAssemblyConfigurator> configurationCallback);
    IAssemblyWithFilterStage AssemblyContaining(Type type);
    IAssemblyWithFilterStage AssemblyContaining(Type type, Action<IAssemblyConfigurator> configurationCallback);
    IAssemblyWithFilterStage AssemblyContaining<T>();
    IAssemblyWithFilterStage AssemblyContaining<T>(Action<IAssemblyConfigurator> configurationCallback);
}