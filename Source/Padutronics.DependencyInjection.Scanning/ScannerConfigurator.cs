using Padutronics.DependencyInjection.Scanning.Conventions;
using Padutronics.DependencyInjection.Scanning.Filters;
using Padutronics.DependencyInjection.Scanning.Fluent;
using Padutronics.IO.Paths;
using Padutronics.Reflection.Assemblies.Finders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Padutronics.DependencyInjection.Scanning;

internal sealed class ScannerConfigurator : IScannerConfigurator, IConfigurationBuilder, IAssemblyWithFilterStage, IScannableConfigurationStage, IScannableConventionStage
{
    private static readonly AssemblyConfigurationCallback defaultAssemblyConfigurationCallback = _ => { };

    private readonly ICollection<IAssemblyConfigurationBuilder> assemblyConfigurationBuilders = new List<IAssemblyConfigurationBuilder>();
    private readonly ICollection<IScanConvention> conventions = new List<IScanConvention>();
    private readonly ICollection<ITypeFilter> excludeFilters = new List<ITypeFilter>();
    private readonly ICollection<ITypeFilter> includeFilters = new List<ITypeFilter>();
    private readonly IDictionary<Type, TypeConfigurationCallback> typeToConfigurationCallbackMappings = new Dictionary<Type, TypeConfigurationCallback>();

    private IAssemblyWithFilterStage AddAssemblyConfigurationBuilder(IAssemblyFinder assemblyFinder, AssemblyConfigurationCallback configurationCallback)
    {
        var configurator = new AssemblyConfigurator(assemblyFinder);

        assemblyConfigurationBuilders.Add(configurator);

        configurationCallback(configurator);

        return this;
    }

    public IAssemblyWithFilterStage AssembliesFromPath(DirectoryPath path)
    {
        return AssembliesFromPath(path, defaultAssemblyConfigurationCallback);
    }

    public IAssemblyWithFilterStage AssembliesFromPath(DirectoryPath path, AssemblyConfigurationCallback configurationCallback)
    {
        return AddAssemblyConfigurationBuilder(new PathAssemblyFinder(path), configurationCallback);
    }

    public IAssemblyWithFilterStage AssembliesFromPath(DirectoryPath path, bool includeExecutables)
    {
        return AssembliesFromPath(path, includeExecutables, defaultAssemblyConfigurationCallback);
    }

    public IAssemblyWithFilterStage AssembliesFromPath(DirectoryPath path, bool includeExecutables, AssemblyConfigurationCallback configurationCallback)
    {
        return AddAssemblyConfigurationBuilder(new PathAssemblyFinder(path, includeExecutables), configurationCallback);
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
        return AddAssemblyConfigurationBuilder(new ConstantAssemblyFinder(assembly), configurationCallback);
    }

    public IAssemblyWithFilterStage AssemblyContaining(Type type)
    {
        return AssemblyContaining(type, defaultAssemblyConfigurationCallback);
    }

    public IAssemblyWithFilterStage AssemblyContaining(Type type, AssemblyConfigurationCallback configurationCallback)
    {
        return AddAssemblyConfigurationBuilder(new TypeAssemblyFinder(type), configurationCallback);
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
        typeToConfigurationCallbackMappings.Add(type, configurationCallback);

        return this;
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
        excludeFilters.Add(filter);

        return this;
    }

    public IFilterStage Exclude<TFilter>()
        where TFilter : ITypeFilter, new()
    {
        return Exclude(new TFilter());
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
        includeFilters.Add(filter);

        return this;
    }

    public IFilterStage Include<TFilter>()
        where TFilter : ITypeFilter, new()
    {
        return Include(new TFilter());
    }

    public IScannableConfigurationStage IncludeConfiguration(IConfigurationModule module)
    {
        module.Load(configurationBuilder: this);

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

    public void Scan(IContainerBuilder containerBuilder)
    {
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