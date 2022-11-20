using Padutronics.IO.Paths;
using System;
using System.Reflection;

namespace Padutronics.DependencyInjection.Scanning.Fluent;

public interface IAssemblyStage
{
    IAssemblyWithFilterStage AssembliesFromPath(DirectoryPath path);
    IAssemblyWithFilterStage AssembliesFromPath(DirectoryPath path, AssemblyConfigurationCallback configurationCallback);
    IAssemblyWithFilterStage AssembliesFromPath(DirectoryPath path, bool includeExecutables);
    IAssemblyWithFilterStage AssembliesFromPath(DirectoryPath path, bool includeExecutables, AssemblyConfigurationCallback configurationCallback);
    IAssemblyWithFilterStage Assembly(string assemblyName);
    IAssemblyWithFilterStage Assembly(string assemblyName, AssemblyConfigurationCallback configurationCallback);
    IAssemblyWithFilterStage Assembly(Assembly assembly);
    IAssemblyWithFilterStage Assembly(Assembly assembly, AssemblyConfigurationCallback configurationCallback);
    IAssemblyWithFilterStage AssemblyContaining(Type type);
    IAssemblyWithFilterStage AssemblyContaining(Type type, AssemblyConfigurationCallback configurationCallback);
    IAssemblyWithFilterStage AssemblyContaining<T>();
    IAssemblyWithFilterStage AssemblyContaining<T>(AssemblyConfigurationCallback configurationCallback);
}