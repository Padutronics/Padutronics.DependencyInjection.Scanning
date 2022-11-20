using Padutronics.DependencyInjection.Scanning.Conventions;
using System;
using System.Collections.Generic;

namespace Padutronics.DependencyInjection.Scanning.Fluent;

public interface IConventionStage : IConfigurationStage
{
    IScannableConventionStage RegisterConcreteTypesAgainstAllInterfaces();
    IScannableConventionStage RegisterConcreteTypesAgainstAllInterfaces(IEnumerable<Type> interfacesToExclude);
    IScannableConventionStage WithConvention(IScanConvention convention);
    IScannableConventionStage WithConvention<TConvention>()
        where TConvention : IScanConvention, new();
}