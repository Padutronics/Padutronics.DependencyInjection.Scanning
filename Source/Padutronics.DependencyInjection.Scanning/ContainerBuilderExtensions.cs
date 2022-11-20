namespace Padutronics.DependencyInjection.Scanning;

public static class ContainerBuilderExtensions
{
    public static void Scan(this IContainerBuilder @this, ScannerConfigurationCallback configurationCallback)
    {
        configurationCallback(new ScannerConfigurator()).Scan(@this);
    }
}