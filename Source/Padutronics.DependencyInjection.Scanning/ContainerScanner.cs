using Trace = Padutronics.Diagnostics.Tracing.Trace<Padutronics.DependencyInjection.Scanning.ContainerScanner>;

namespace Padutronics.DependencyInjection.Scanning;

internal sealed class ContainerScanner
{
    public void Scan(IContainerBuilder containerBuilder, ScannerConfigurationCallback configurationCallback)
    {
        Trace.CallStart("Started container scanner configuring.");

        IScannable scannable = configurationCallback(new ScannerConfigurator());
        scannable.Scan(containerBuilder);

        Trace.CallEnd("Finished container scanner configuring.");
    }
}