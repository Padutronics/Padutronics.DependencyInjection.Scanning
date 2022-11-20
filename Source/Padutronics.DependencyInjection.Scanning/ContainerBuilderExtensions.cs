namespace Padutronics.DependencyInjection.Scanning;

public static class ContainerBuilderExtensions
{
    public static void Scan(this IContainerBuilder @this, ScannerConfigurationCallback configurationCallback)
    {
        var containerScanner = new ContainerScanner();
        containerScanner.Scan(containerBuilder: @this, configurationCallback);
    }
}