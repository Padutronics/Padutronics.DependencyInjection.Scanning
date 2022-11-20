using System;

namespace Padutronics.DependencyInjection.Scanning.Conventions;

internal sealed class SelfScanConvention : IScanConvention
{
    public void Scan(TypeRegistry typeRegistry, IContainerBuilder containerBuilder, TypeRegistrationCallback registrationCallback)
    {
        foreach (Type type in typeRegistry.ClosedTypes.ConcreteTypes)
        {
            registrationCallback(type, containerBuilder.For(type).UseSelf());
        }
    }
}