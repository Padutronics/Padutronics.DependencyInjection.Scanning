using Padutronics.DependencyInjection.Scanning.Conventions;
using System;
using System.Collections.Generic;

namespace Padutronics.DependencyInjection.Scanning.Fluent;

public interface IConventionStage
{
    IConventionWithConfigurationStage RegisterConcreteTypesAgainstAllInterfaces();
    IConventionWithConfigurationStage RegisterConcreteTypesAgainstAllInterfaces(IEnumerable<Type> interfacesToExclude);
    IConventionWithConfigurationStage RegisterConcreteTypesAgainstInterface<TInterface>()
        where TInterface : class;
    IConventionWithConfigurationStage RegisterConcreteTypesAgainstSelf();
    IConventionWithConfigurationStage RegisterFactories();
    IConventionWithConfigurationStage RegisterFactories(string typeNamePattern);
    IConventionWithConfigurationStage RegisterOpenTypesAgainstOpenInterfaces();
    IConventionWithConfigurationStage WithConvention(IScanConvention convention);
    IConventionWithConfigurationStage WithConvention<TConvention>()
        where TConvention : IScanConvention, new();
}