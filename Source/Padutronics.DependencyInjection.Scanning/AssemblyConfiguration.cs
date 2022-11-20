using Padutronics.Reflection.Assemblies.Finders;
using Padutronics.Reflection.Types.Finders;

namespace Padutronics.DependencyInjection.Scanning;

internal sealed class AssemblyConfiguration
{
    public AssemblyConfiguration(IAssemblyFinder assemblyFinder, ITypeFinder typeFinder)
    {
        AssemblyFinder = assemblyFinder;
        TypeFinder = typeFinder;
    }

    public IAssemblyFinder AssemblyFinder { get; }

    public ITypeFinder TypeFinder { get; }
}