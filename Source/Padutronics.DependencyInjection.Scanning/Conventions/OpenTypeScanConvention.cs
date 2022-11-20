using Padutronics.DependencyInjection.Registration.Fluent;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Padutronics.DependencyInjection.Scanning.Conventions;

internal sealed class OpenTypeScanConvention : IScanConvention
{
    public void Scan(TypeRegistry typeRegistry, IContainerBuilder containerBuilder, Action<Type, ILifetimeStage> registrationCallback)
    {
        foreach (Type openType in typeRegistry.OpenTypes.ConcreteTypes)
        {
            Type[] typeGenericArguments = openType.GetGenericArguments();

            registrationCallback(openType, containerBuilder.For(openType).UseSelf());

            IEnumerable<Type> interfaceTypes = openType.GetInterfaces();
            foreach (Type interfaceType in interfaceTypes)
            {
                if (interfaceType.IsGenericType)
                {
                    Type[] interfaceGenericArguments = interfaceType.GetGenericArguments();

                    if (Enumerable.SequenceEqual(typeGenericArguments, interfaceGenericArguments))
                    {
                        Type openTypeInterface = interfaceType.GetGenericTypeDefinition();

                        registrationCallback(openType, containerBuilder.For(openTypeInterface).Use(openType));
                    }
                }
            }
        }
    }
}