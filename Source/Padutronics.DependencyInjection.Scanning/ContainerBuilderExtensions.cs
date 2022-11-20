using Padutronics.Diagnostics.Tracing;
using System;

namespace Padutronics.DependencyInjection.Scanning;

public static class ContainerBuilderExtensions
{
    public static void Scan(this IContainerBuilder @this, Action<IScannerConfigurator> configurationCallback)
    {
        Trace.CallStart(typeof(ContainerBuilderExtensions), "Started container scanner configuring.");

        var configurator = new ScannerConfigurator();

        configurationCallback(configurator);

        configurator.Scan(containerBuilder: @this);

        Trace.CallEnd(typeof(ContainerBuilderExtensions), "Finished container scanner configuring.");
    }
}