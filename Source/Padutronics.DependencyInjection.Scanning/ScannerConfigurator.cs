using Padutronics.DependencyInjection.Scanning.Conventions;
using Padutronics.DependencyInjection.Scanning.Filters;
using Padutronics.DependencyInjection.Scanning.Fluent;
using Padutronics.IO.Paths;
using Padutronics.Reflection.Assemblies.Finders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using Trace = Padutronics.Diagnostics.Tracing.Trace<Padutronics.DependencyInjection.Scanning.ScannerConfigurator>;

namespace Padutronics.DependencyInjection.Scanning;

internal sealed class ScannerConfigurator : IScannerConfigurator, IConfigurationBuilder, IAssemblyWithFilterStage, IScannableConfigurationStage, IScannableConventionStage
{
    private static readonly AssemblyConfigurationCallback defaultAssemblyConfigurationCallback = _ => { };

    private readonly ICollection<IAssemblyConfigurationBuilder> assemblyConfigurationBuilders = new List<IAssemblyConfigurationBuilder>();
    private readonly ICollection<IScanConvention> conventions = new List<IScanConvention>();
    private readonly ICollection<ITypeFilter> excludeFilters = new List<ITypeFilter>();
    private readonly ICollection<ITypeFilter> includeFilters = new List<ITypeFilter>();
    private readonly IDictionary<Type, TypeConfigurationCallback> typeToConfigurationCallbackMappings = new Dictionary<Type, TypeConfigurationCallback>();

    private ScannerConfigurator AddAssemblyFinder(IAssemblyFinder assemblyFinder, AssemblyConfigurationCallback configurationCallback)
    {
        Trace.Call($"Added assembly finder: {assemblyFinder.GetType()}.");

        var configurator = new AssemblyConfigurator(assemblyFinder);

        assemblyConfigurationBuilders.Add(configurator);

        configurationCallback(configurator);

        return this;
    }

    private ScannerConfigurator AddConfiguration(Type type, TypeConfigurationCallback configurationCallback)
    {
        Trace.Call($"Added configuration for type: {type}.");

        typeToConfigurationCallbackMappings.Add(type, configurationCallback);

        return this;
    }

    private ScannerConfigurator AddConvention(IScanConvention convention)
    {
        Trace.Call($"Added scan convention: {convention.GetType()}.");

        conventions.Add(convention);

        return this;
    }

    private ScannerConfigurator AddExcludeFilter(ITypeFilter filter)
    {
        Trace.Call($"Added exclude filter: {filter.GetType()}.");

        excludeFilters.Add(filter);

        return this;
    }

    private ScannerConfigurator AddIncludeFilter(ITypeFilter filter)
    {
        Trace.Call($"Added include filter: {filter.GetType()}.");

        includeFilters.Add(filter);

        return this;
    }

    public IAssemblyWithFilterStage AssembliesFromPath(DirectoryPath path)
    {
        return AssembliesFromPath(path, defaultAssemblyConfigurationCallback);
    }

    public IAssemblyWithFilterStage AssembliesFromPath(DirectoryPath path, AssemblyConfigurationCallback configurationCallback)
    {
        return AddAssemblyFinder(new PathAssemblyFinder(path), configurationCallback);
    }

    public IAssemblyWithFilterStage AssembliesFromPath(DirectoryPath path, bool includeExecutables)
    {
        return AssembliesFromPath(path, includeExecutables, defaultAssemblyConfigurationCallback);
    }

    public IAssemblyWithFilterStage AssembliesFromPath(DirectoryPath path, bool includeExecutables, AssemblyConfigurationCallback configurationCallback)
    {
        return AddAssemblyFinder(new PathAssemblyFinder(path, includeExecutables), configurationCallback);
    }

    public IAssemblyWithFilterStage Assembly(string assemblyName)
    {
        return Assembly(assemblyName, defaultAssemblyConfigurationCallback);
    }

    public IAssemblyWithFilterStage Assembly(string assemblyName, AssemblyConfigurationCallback configurationCallback)
    {
        return Assembly(System.Reflection.Assembly.Load(assemblyName), configurationCallback);
    }

    public IAssemblyWithFilterStage Assembly(Assembly assembly)
    {
        return Assembly(assembly, defaultAssemblyConfigurationCallback);
    }

    public IAssemblyWithFilterStage Assembly(Assembly assembly, AssemblyConfigurationCallback configurationCallback)
    {
        return AddAssemblyFinder(new ConstantAssemblyFinder(assembly), configurationCallback);
    }

    public IAssemblyWithFilterStage AssemblyContaining(Type type)
    {
        return AssemblyContaining(type, defaultAssemblyConfigurationCallback);
    }

    public IAssemblyWithFilterStage AssemblyContaining(Type type, AssemblyConfigurationCallback configurationCallback)
    {
        return AddAssemblyFinder(new TypeAssemblyFinder(type), configurationCallback);
    }

    public IAssemblyWithFilterStage AssemblyContaining<T>()
    {
        return AssemblyContaining<T>(defaultAssemblyConfigurationCallback);
    }

    public IAssemblyWithFilterStage AssemblyContaining<T>(AssemblyConfigurationCallback configurationCallback)
    {
        return AssemblyContaining(typeof(T), configurationCallback);
    }

    public IScannableConfigurationStage Configure(Type type, TypeConfigurationCallback configurationCallback)
    {
        return AddConfiguration(type, configurationCallback);
    }

    public IScannableConfigurationStage Configure<T>(TypeConfigurationCallback configurationCallback)
    {
        return Configure(typeof(T), configurationCallback);
    }

    public IFilterStage Exclude(Predicate<Type> predicate)
    {
        return Exclude(new PredicateTypeFilter(predicate));
    }

    public IFilterStage Exclude(ITypeFilter filter)
    {
        return AddExcludeFilter(filter);
    }

    public IFilterStage Exclude<TFilter>()
        where TFilter : ITypeFilter, new()
    {
        return Exclude(new TFilter());
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

    public IFilterStage Include(Predicate<Type> predicate)
    {
        return Include(new PredicateTypeFilter(predicate));
    }

    public IFilterStage Include(ITypeFilter filter)
    {
        return AddIncludeFilter(filter);
    }

    public IFilterStage Include<TFilter>()
        where TFilter : ITypeFilter, new()
    {
        return Include(new TFilter());
    }

    public IScannableConfigurationStage IncludeConfiguration(IConfigurationModule module)
    {
        Trace.CallStart();
        Trace.Information($"Including configuration module: {module.GetType()}");

        module.Load(configurationBuilder: this);

        Trace.CallEnd();

        return this;
    }

    public IScannableConfigurationStage IncludeConfiguration<TModule>()
        where TModule : IConfigurationModule, new()
    {
        return IncludeConfiguration(new TModule());
    }

    public IScannableConventionStage RegisterConcreteTypesAgainstAllInterfaces()
    {
        return WithConvention<AllInterfacesScanConvention>();
    }

    public IScannableConventionStage RegisterConcreteTypesAgainstAllInterfaces(IEnumerable<Type> interfacesToExclude)
    {
        return WithConvention(new AllInterfacesScanConvention(interfacesToExclude));
    }

    public IScannableConventionStage RegisterConcreteTypesAgainstInterface<TInterface>()
        where TInterface : class
    {
        return WithConvention(InterfaceScanConvention.Create<TInterface>());
    }

    public IScannableConventionStage RegisterConcreteTypesAgainstSelf()
    {
        return WithConvention<SelfScanConvention>();
    }

    public IScannableConventionStage RegisterFactories()
    {
        return WithConvention<FactoryScanConvention>();
    }

    public IScannableConventionStage RegisterFactories(string typeNamePattern)
    {
        return WithConvention(new FactoryScanConvention(typeNamePattern));
    }

    public IScannableConventionStage RegisterOpenTypesAgainstOpenInterfaces()
    {
        return WithConvention<OpenTypeScanConvention>();
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
                if (typeToConfigurationCallbackMappings.TryGetValue(type, out TypeConfigurationCallback? configurator))
                {
                    configurator(lifetimeStage);
                }
            });
        }

        Trace.CallEnd("Finished container scanning.");
    }

    public IScannableConventionStage WithConvention(IScanConvention convention)
    {
        return AddConvention(convention);
    }

    public IScannableConventionStage WithConvention<TConvention>()
        where TConvention : IScanConvention, new()
    {
        return WithConvention(new TConvention());
    }
}