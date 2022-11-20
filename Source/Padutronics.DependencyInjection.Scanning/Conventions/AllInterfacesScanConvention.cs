using Padutronics.DependencyInjection.Registration.Fluent;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Padutronics.DependencyInjection.Scanning.Conventions;

internal sealed class AllInterfacesScanConvention : IScanConvention
{
    private readonly IEnumerable<Type> interfacesToExclude;

    public AllInterfacesScanConvention() :
        this(interfacesToExclude: Enumerable.Empty<Type>())
    {
    }

    public AllInterfacesScanConvention(IEnumerable<Type> interfacesToExclude)
    {
        foreach (Type interfaceToExclude in interfacesToExclude)
        {
            if (!interfaceToExclude.IsInterface)
            {
                throw new ArgumentException($"Type {interfaceToExclude} is not an interface.", nameof(interfacesToExclude));
            }
        }

        this.interfacesToExclude = interfacesToExclude;
    }

    public void Scan(TypeRegistry typeRegistry, IContainerBuilder containerBuilder, Action<Type, ILifetimeStage> registrationCallback)
    {
        foreach (Type type in typeRegistry.ClosedTypes.ConcreteTypes)
        {
            IEnumerable<Type> interfaceTypes = type
                .GetInterfaces()
                .Except(interfacesToExclude);
            if (interfaceTypes.Any())
            {
                registrationCallback(type, containerBuilder.For(interfaceTypes).Use(type));
            }
        }
    }
}