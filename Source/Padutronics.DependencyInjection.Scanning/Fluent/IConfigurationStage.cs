using System;

namespace Padutronics.DependencyInjection.Scanning.Fluent;

public interface IConfigurationStage
{
    IConfigurationStage Configure(Type type, TypeConfigurationCallback configurationCallback);
    IConfigurationStage Configure<T>(TypeConfigurationCallback configurationCallback);
    IConfigurationStage IncludeConfiguration(IConfigurationModule module);
    IConfigurationStage IncludeConfiguration<TModule>()
        where TModule : IConfigurationModule, new();
}