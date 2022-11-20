using System;

namespace Padutronics.DependencyInjection.Scanning.Fluent;

public interface IConfigurationStage
{
    IScannableConfigurationStage Configure(Type type, TypeConfigurationCallback configurationCallback);
    IScannableConfigurationStage Configure<T>(TypeConfigurationCallback configurationCallback);
    IScannableConfigurationStage IncludeConfiguration(IConfigurationModule module);
    IScannableConfigurationStage IncludeConfiguration<TModule>()
        where TModule : IConfigurationModule, new();
}