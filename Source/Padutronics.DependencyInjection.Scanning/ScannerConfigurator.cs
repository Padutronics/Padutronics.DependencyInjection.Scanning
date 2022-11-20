using Padutronics.DependencyInjection.Scanning.Conventions;
using Padutronics.DependencyInjection.Scanning.Fluent;
using Padutronics.Reflection.Assemblies.Finders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Padutronics.DependencyInjection.Scanning;

internal sealed class ScannerConfigurator : IScannerConfigurator, IScannableConventionStage
{
    private readonly ICollection<IAssemblyFinder> assemblyFinders = new List<IAssemblyFinder>();
    private readonly ICollection<IScanConvention> conventions = new List<IScanConvention>();

    private IConventionStage AddAssemblyFinder(IAssemblyFinder assemblyFinder)
    {
        assemblyFinders.Add(assemblyFinder);

        return this;
    }

    public IConventionStage Assembly(string assemblyName)
    {
        return Assembly(System.Reflection.Assembly.Load(assemblyName));
    }

    public IConventionStage Assembly(Assembly assembly)
    {
        return AddAssemblyFinder(new ConstantAssemblyFinder(assembly));
    }

    public IConventionStage AssemblyContaining(Type type)
    {
        return AddAssemblyFinder(new TypeAssemblyFinder(type));
    }

    public IConventionStage AssemblyContaining<T>()
    {
        return AssemblyContaining(typeof(T));
    }

    private IEnumerable<Type> GetAllTypes()
    {
        return assemblyFinders.SelectMany(
            assemblyFinder => assemblyFinder
                .FindAssemblies()
                .SelectMany(assembly => assembly.GetExportedTypes())
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