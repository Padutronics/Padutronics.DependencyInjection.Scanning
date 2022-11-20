namespace Padutronics.DependencyInjection.Scanning.Conventions;

public interface IScanConvention
{
    void Scan(TypeRegistry typeRegistry, IContainerBuilder containerBuilder);
}