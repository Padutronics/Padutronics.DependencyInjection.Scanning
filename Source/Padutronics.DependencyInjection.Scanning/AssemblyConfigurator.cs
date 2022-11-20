using Padutronics.Reflection.Assemblies.Finders;
using Padutronics.Reflection.Types.Finders;

namespace Padutronics.DependencyInjection.Scanning;

internal sealed class AssemblyConfigurator : IAssemblyConfigurator, IAssemblyConfigurationBuilder
{
    private readonly IAssemblyFinder assemblyFinder;

    private ITypeFinder typeFinder = new PublicTypeFinder();

    public AssemblyConfigurator(IAssemblyFinder assemblyFinder)
    {
        this.assemblyFinder = assemblyFinder;
    }

    public AssemblyConfiguration Build()
    {
        return new AssemblyConfiguration(assemblyFinder, typeFinder);
    }

    public void IncludeAllTypes()
    {
        typeFinder = new AllTypeFinder();
    }

    public void IncludePublicTypes()
    {
        typeFinder = new PublicTypeFinder();
    }
}