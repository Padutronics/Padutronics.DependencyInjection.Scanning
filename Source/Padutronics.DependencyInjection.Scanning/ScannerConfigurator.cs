using Padutronics.DependencyInjection.Scanning.Conventions;
using Padutronics.DependencyInjection.Scanning.Fluent;
using Padutronics.IO.Paths;
using Padutronics.Reflection.Assemblies.Finders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Padutronics.DependencyInjection.Scanning;

internal sealed class ScannerConfigurator : IScannerConfigurator, IScannableConventionStage
{
    private static readonly AssemblyConfigurationCallback defaultAssemblyConfigurationCallback = _ => { };

    private readonly ICollection<IAssemblyConfigurationBuilder> assemblyConfigurationBuilders = new List<IAssemblyConfigurationBuilder>();
    private readonly ICollection<IScanConvention> conventions = new List<IScanConvention>();

    private IConventionStage AddAssemblyConfigurationBuilder(IAssemblyFinder assemblyFinder, AssemblyConfigurationCallback configurationCallback)
    {
        var configurator = new AssemblyConfigurator(assemblyFinder);

        assemblyConfigurationBuilders.Add(configurator);

        configurationCallback(configurator);

        return this;
    }

    public IConventionStage AssembliesFromPath(DirectoryPath path)
    {
        return AssembliesFromPath(path, defaultAssemblyConfigurationCallback);
    }

    public IConventionStage AssembliesFromPath(DirectoryPath path, AssemblyConfigurationCallback configurationCallback)
    {
        return AddAssemblyConfigurationBuilder(new PathAssemblyFinder(path), configurationCallback);
    }

    public IConventionStage AssembliesFromPath(DirectoryPath path, bool includeExecutables)
    {
        return AssembliesFromPath(path, includeExecutables, defaultAssemblyConfigurationCallback);
    }

    public IConventionStage AssembliesFromPath(DirectoryPath path, bool includeExecutables, AssemblyConfigurationCallback configurationCallback)
    {
        return AddAssemblyConfigurationBuilder(new PathAssemblyFinder(path, includeExecutables), configurationCallback);
    }

    public IConventionStage Assembly(string assemblyName)
    {
        return Assembly(assemblyName, defaultAssemblyConfigurationCallback);
    }

    public IConventionStage Assembly(string assemblyName, AssemblyConfigurationCallback configurationCallback)
    {
        return Assembly(System.Reflection.Assembly.Load(assemblyName), configurationCallback);
    }

    public IConventionStage Assembly(Assembly assembly)
    {
        return Assembly(assembly, defaultAssemblyConfigurationCallback);
    }

    public IConventionStage Assembly(Assembly assembly, AssemblyConfigurationCallback configurationCallback)
    {
        return AddAssemblyConfigurationBuilder(new ConstantAssemblyFinder(assembly), configurationCallback);
    }

    public IConventionStage AssemblyContaining(Type type)
    {
        return AssemblyContaining(type, defaultAssemblyConfigurationCallback);
    }

    public IConventionStage AssemblyContaining(Type type, AssemblyConfigurationCallback configurationCallback)
    {
        return AddAssemblyConfigurationBuilder(new TypeAssemblyFinder(type), configurationCallback);
    }

    public IConventionStage AssemblyContaining<T>()
    {
        return AssemblyContaining<T>(defaultAssemblyConfigurationCallback);
    }

    public IConventionStage AssemblyContaining<T>(AssemblyConfigurationCallback configurationCallback)
    {
        return AssemblyContaining(typeof(T), configurationCallback);
    }

    private IEnumerable<Type> GetAllTypes()
    {
        return assemblyConfigurationBuilders
            .Select(assemblyConfigurationBuilder => assemblyConfigurationBuilder.Build())
            .SelectMany(
                assemblyConfiguration => assemblyConfiguration.AssemblyFinder
                    .FindAssemblies()
                    .SelectMany(assembly => assemblyConfiguration.TypeFinder.FindTypes(assembly))
            );
    }

    public void Scan(IContainerBuilder containerBuilder)
    {
        IEnumerable<Type> allTypes = GetAllTypes();

        var typeRegistry = new TypeRegistry(allTypes);

        foreach (IScanConvention convention in conventions)
        {
            convention.Scan(typeRegistry, containerBuilder);
        }
    }

    public IScannableConventionStage WithConvention(IScanConvention convention)
    {
        conventions.Add(convention);

        return this;
    }

    public IScannableConventionStage WithConvention<TConvention>()
        where TConvention : IScanConvention, new()
    {
        return WithConvention(new TConvention());
    }
}