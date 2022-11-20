namespace Padutronics.DependencyInjection.Scanning;

public interface IScannable
{
    void Scan(IContainerBuilder containerBuilder);
}