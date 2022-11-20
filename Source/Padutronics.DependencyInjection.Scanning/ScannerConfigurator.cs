using Padutronics.DependencyInjection.Scanning.Conventions;
using Padutronics.DependencyInjection.Scanning.Filters;
using Padutronics.DependencyInjection.Scanning.Fluent;
using Padutronics.IO.Paths;
using Padutronics.Reflection.Assemblies.Finders;
using System;
using System.Collections.Generic;
using System.Reflection;

using Trace = Padutronics.Diagnostics.Tracing.Trace<Padutronics.DependencyInjection.Scanning.ScannerConfigurator>;

namespace Padutronics.DependencyInjection.Scanning;

internal sealed class ScannerConfigurator : IScannerConfigurator, IConfigurationBuilder, IAssemblyWithFilterStage, IScannableConfigurationStage, IScannableConventionStage
{
    private static readonly AssemblyConfigurationCallback defaultAssemblyConfigurationCallback = _ => { };

    private readonly Scanner scanner = new Scanner();

    public IAssemblyWithFilterStage AssembliesFromPath(DirectoryPath path)
    {
        return AssembliesFromPath(path, defaultAssemblyConfigurationCallback);
    }

    public IAssemblyWithFilterStage AssembliesFromPath(DirectoryPath path, AssemblyConfigurationCallback configurationCallback)
    {
        scanner.AddAssemblyFinder(new PathAssemblyFinder(path), configurationCallback);

        return this;
    }

    public IAssemblyWithFilterStage AssembliesFromPath(DirectoryPath path, bool includeExecutables)
    {
        return AssembliesFromPath(path, includeExecutables, defaultAssemblyConfigurationCallback);
    }

    public IAssemblyWithFilterStage AssembliesFromPath(DirectoryPath path, bool includeExecutables, AssemblyConfigurationCallback configurationCallback)
    {
        scanner.AddAssemblyFinder(new PathAssemblyFinder(path, includeExecutables), configurationCallback);

        return this;
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
        scanner.AddAssemblyFinder(new ConstantAssemblyFinder(assembly), configurationCallback);

        return this;
    }

    public IAssemblyWithFilterStage AssemblyContaining(Type type)
    {
        return AssemblyContaining(type, defaultAssemblyConfigurationCallback);
    }

    public IAssemblyWithFilterStage AssemblyContaining(Type type, AssemblyConfigurationCallback configurationCallback)
    {
        scanner.AddAssemblyFinder(new TypeAssemblyFinder(type), configurationCallback);

        return this;
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
        scanner.AddConfiguration(type, configurationCallback);

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
        scanner.AddExcludeFilter(filter);

        return this;
    }

    public IFilterStage Exclude<TFilter>()
        where TFilter : ITypeFilter, new()
    {
        return Exclude(new TFilter());
    }

    public IFilterStage Include(Predicate<Type> predicate)
    {
        return Include(new PredicateTypeFilter(predicate));
    }

    public IFilterStage Include(ITypeFilter filter)
    {
        scanner.AddIncludeFilter(filter);

        return this;
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
        scanner.Scan(containerBuilder);
    }

    public IScannableConventionStage WithConvention(IScanConvention convention)
    {
        scanner.AddConvention(convention);

        return this;
    }

    public IScannableConventionStage WithConvention<TConvention>()
        where TConvention : IScanConvention, new()
    {
        return WithConvention(new TConvention());
    }
}