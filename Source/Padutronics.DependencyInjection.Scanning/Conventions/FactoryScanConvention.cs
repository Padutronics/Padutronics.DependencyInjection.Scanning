using System;

namespace Padutronics.DependencyInjection.Scanning.Conventions;

internal sealed class FactoryScanConvention : IScanConvention
{
    public void Scan(TypeRegistry typeRegistry, IContainerBuilder containerBuilder, TypeRegistrationCallback registrationCallback)
    {
        foreach (Type type in typeRegistry.ClosedTypes.InterfaceTypes)
        {
            if (type.Name.EndsWith("Factory"))
            {
                containerBuilder.For(type).UseFactory();
            }
        }
    }
}