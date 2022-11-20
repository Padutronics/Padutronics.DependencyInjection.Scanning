using Padutronics.DependencyInjection.Scanning.Conventions;
using System;
using System.Collections.Generic;

namespace Padutronics.DependencyInjection.Scanning.Fluent;

public interface IConventionStage
{
    IConventionConfigurationStage RegisterConcreteTypesAgainstAllInterfaces();
    IConventionConfigurationStage RegisterConcreteTypesAgainstAllInterfaces(IEnumerable<Type> interfacesToExclude);
    IConventionConfigurationStage RegisterConcreteTypesAgainstInterface<TInterface>()
        where TInterface : class;
    IConventionConfigurationStage RegisterConcreteTypesAgainstSelf();
    IConventionConfigurationStage RegisterFactories();
    IConventionConfigurationStage RegisterFactories(string typeNamePattern);
    IConventionConfigurationStage RegisterOpenTypesAgainstOpenInterfaces();
    IConventionConfigurationStage WithConvention(IScanConvention convention);
    IConventionConfigurationStage WithConvention<TConvention>()
        where TConvention : IScanConvention, new();
}