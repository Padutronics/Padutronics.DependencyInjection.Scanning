using Padutronics.DependencyInjection.Scanning.Conventions;

namespace Padutronics.DependencyInjection.Scanning.Fluent;

public interface IConventionStage
{
    IScannableConventionStage WithConvention(IScanConvention convention);
    IScannableConventionStage WithConvention<TConvention>()
        where TConvention : IScanConvention, new();
}