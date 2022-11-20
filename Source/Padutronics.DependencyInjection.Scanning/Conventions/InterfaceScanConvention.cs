using Padutronics.Generics;
using System;

namespace Padutronics.DependencyInjection.Scanning.Conventions;

internal sealed class InterfaceScanConvention : IScanConvention
{
    private readonly Type interfaceType;

    public InterfaceScanConvention(Type interfaceType)
    {
        this.interfaceType = interfaceType;
    }

    public static InterfaceScanConvention Create<TInterface>()
        where TInterface : class
    {
        Type interfaceType = typeof(TInterface);
        if (!interfaceType.IsInterface)
        {
            throw new TypeArgumentException($"Type {interfaceType} is not an interface.", nameof(TInterface));
        }

        return new InterfaceScanConvention(interfaceType);
    }
    
    public void Scan(TypeRegistry typeRegistry, IContainerBuilder containerBuilder, TypeRegistrationCallback registrationCallback)
    {
        foreach (Type type in typeRegistry.ClosedTypes.ConcreteTypes)
        {
            if (interfaceType.IsAssignableFrom(type))
            {
                registrationCallback(type, containerBuilder.For(interfaceType).Use(type));
            }
        }
    }
}