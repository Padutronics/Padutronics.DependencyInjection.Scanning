using Padutronics.Diagnostics.Tracing;

namespace Padutronics.DependencyInjection.Scanning;

public static class ContainerBuilderExtensions
{
    public static void Scan(this IContainerBuilder @this, ScannerConfigurationCallback configurationCallback)
    {
        Trace.CallStart(typeof(ContainerBuilderExtensions), "Started container scanner configuring.");

        IScannable scannable = configurationCallback(new ScannerConfigurator());
        scannable.Scan(@this);

        Trace.CallEnd(typeof(ContainerBuilderExtensions), "Finished container scanner configuring.");
    }
}