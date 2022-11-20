using Padutronics.DependencyInjection.Registration.Fluent;
using Padutronics.DependencyInjection.Scanning.Conventions;
using Padutronics.DependencyInjection.Scanning.Filters;
using Padutronics.Reflection.Assemblies.Finders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using Trace = Padutronics.Diagnostics.Tracing.Trace<Padutronics.DependencyInjection.Scanning.Scanner>;

namespace Padutronics.DependencyInjection.Scanning;

internal sealed class Scanner
{
    private readonly ICollection<IAssemblyConfigurationBuilder> assemblyConfigurationBuilders = new List<IAssemblyConfigurationBuilder>();
    private readonly ICollection<IScanConvention> conventions = new List<IScanConvention>();
    private readonly ICollection<ITypeFilter> excludeFilters = new List<ITypeFilter>();
    private readonly ICollection<ITypeFilter> includeFilters = new List<ITypeFilter>();
    private readonly IDictionary<Type, Action<ILifetimeStage>> typeToConfigurationCallbackMappings = new Dictionary<Type, Action<ILifetimeStage>>();

    public void AddAssemblyFinder(IAssemblyFinder assemblyFinder, Action<IAssemblyConfigurator> configurationCallback)
    {
        Trace.Call($"Added assembly finder: {assemblyFinder.GetType()}.");

        var configurator = new AssemblyConfigurator(assemblyFinder);

        assemblyConfigurationBuilders.Add(configurator);

        configurationCallback(configurator);
    }

    public void AddConfiguration(Type type, Action<ILifetimeStage> configurationCallback)
    {
        Trace.Call($"Added configuration for type: {type}.");

        typeToConfigurationCallbackMappings.Add(type, configurationCallback);
    }

    public void AddConvention(IScanConvention convention)
    {
        Trace.Call($"Added scan convention: {convention.GetType()}.");

        conventions.Add(convention);
    }

    public void AddExcludeFilter(ITypeFilter filter)
    {
        Trace.Call($"Added exclude filter: {filter.GetType()}.");

        excludeFilters.Add(filter);
    }

    public void AddIncludeFilter(ITypeFilter filter)
    {
        Trace.Call($"Added include filter: {filter.GetType()}.");

        includeFilters.Add(filter);
    }

    private IEnumerable<Type> GetAllTypes()
    {
        Trace.CallStart("Started types retrieval from assemblies.");

        IReadOnlyDictionary<Assembly, IEnumerable<Type>> assemblyToTypesMappings = assemblyConfigurationBuilders
            .Select(assemblyConfigurationBuilder => assemblyConfigurationBuilder.Build())
            .SelectMany(
                assemblyConfiguration => assemblyConfiguration.AssemblyFinder
                    .FindAssemblies()
                    .Select(assembly => new
                    {
                        Assembly = assembly,
                        Types = assemblyConfiguration.TypeFinder.FindTypes(assembly)
                    })
            )
            .DistinctBy(assemblyData => assemblyData.Assembly.FullName ?? throw new Exception("Assembly name is null."))
            .ToDictionary(
                assemblyData => assemblyData.Assembly,
                assemblyData => assemblyData.Types
            );

        Trace.Information($"Found {assemblyToTypesMappings.Count} assemblies: [{string.Join(", ", assemblyToTypesMappings.Select(assemblyToTypesMapping => $"\"{assemblyToTypesMapping.Key.FullName}\" ({assemblyToTypesMapping.Value.Count()})"))}].");

        IEnumerable<Type> types = assemblyToTypesMappings
            .SelectMany(assemblyToTypesMapping => assemblyToTypesMapping.Value)
            .ToList();

        Trace.CallEnd("Finished types retrieval from assemblies.");

        return types;
    }

    private IEnumerable<Type> GetExcludedTypes(IEnumerable<Type> allTypes)
    {
        return allTypes.Where(type => excludeFilters.Any(filter => filter.IsValid(type)));
    }

    private IEnumerable<Type> GetIncludedTypes(IEnumerable<Type> allTypes)
    {
        return includeFilters.Any()
            ? allTypes.Where(type => includeFilters.Any(filter => filter.IsValid(type)))
            : allTypes;
    }

    public void Scan(IContainerBuilder containerBuilder)
    {
        Trace.CallStart("Started container scanning.");

        IEnumerable<Type> allTypes = GetAllTypes();

        IEnumerable<Type> includedTypes = GetIncludedTypes(allTypes);
        IEnumerable<Type> excludedTypes = GetExcludedTypes(allTypes);

        includedTypes = includedTypes.Except(excludedTypes);

        var typeRegistry = new TypeRegistry(includedTypes);

        foreach (IScanConvention convention in conventions)
        {
            convention.Scan(typeRegistry, containerBuilder, (type, lifetimeStage) =>
            {
                if (typeToConfigurationCallbackMappings.TryGetValue(type, out Action<ILifetimeStage>? configurator))
                {
                    configurator(lifetimeStage);
                }
            });
        }

        Trace.CallEnd("Finished container scanning.");
    }
}