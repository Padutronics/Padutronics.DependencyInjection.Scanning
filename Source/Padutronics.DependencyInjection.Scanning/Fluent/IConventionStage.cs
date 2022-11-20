using Padutronics.DependencyInjection.Scanning.Conventions;
using System;
using System.Collections.Generic;

namespace Padutronics.DependencyInjection.Scanning.Fluent;

public interface IConventionStage
{
    IScannableConventionStage RegisterConcreteTypesAgainstAllInterfaces();
    IScannableConventionStage RegisterConcreteTypesAgainstAllInterfaces(IEnumerable<Type> interfacesToExclude);
    IScannableConventionStage RegisterConcreteTypesAgainstInterface<TInterface>()
        where TInterface : class;
    IScannableConventionStage RegisterConcreteTypesAgainstSelf();
    IScannableConventionStage RegisterFactories();
    IScannableConventionStage RegisterOpenTypesAgainstOpenInterfaces();
    IScannableConventionStage WithConvention(IScanConvention convention);
    IScannableConventionStage WithConvention<TConvention>()
        where TConvention : IScanConvention, new();
}