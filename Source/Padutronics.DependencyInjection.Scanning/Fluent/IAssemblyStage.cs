using Padutronics.IO.Paths;
using System;
using System.Reflection;

namespace Padutronics.DependencyInjection.Scanning.Fluent;

public interface IAssemblyStage
{
    IConventionStage AssembliesFromPath(DirectoryPath path);
    IConventionStage AssembliesFromPath(DirectoryPath path, AssemblyConfigurationCallback configurationCallback);
    IConventionStage AssembliesFromPath(DirectoryPath path, bool includeExecutables);
    IConventionStage AssembliesFromPath(DirectoryPath path, bool includeExecutables, AssemblyConfigurationCallback configurationCallback);
    IConventionStage Assembly(string assemblyName);
    IConventionStage Assembly(string assemblyName, AssemblyConfigurationCallback configurationCallback);
    IConventionStage Assembly(Assembly assembly);
    IConventionStage Assembly(Assembly assembly, AssemblyConfigurationCallback configurationCallback);
    IConventionStage AssemblyContaining(Type type);
    IConventionStage AssemblyContaining(Type type, AssemblyConfigurationCallback configurationCallback);
    IConventionStage AssemblyContaining<T>();
    IConventionStage AssemblyContaining<T>(AssemblyConfigurationCallback configurationCallback);
}