namespace Padutronics.DependencyInjection.Scanning;

public interface IAssemblyConfigurator
{
    void IncludeAllTypes();
    void IncludePublicTypes();
}