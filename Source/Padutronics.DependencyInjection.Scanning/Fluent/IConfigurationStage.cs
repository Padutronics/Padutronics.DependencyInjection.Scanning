using Padutronics.DependencyInjection.Registration.Fluent;
using System;

namespace Padutronics.DependencyInjection.Scanning.Fluent;

public interface IConfigurationStage
{
    IConfigurationStage Configure(Type type, Action<ILifetimeStage> configurationCallback);
    IConfigurationStage Configure<T>(Action<ILifetimeStage> configurationCallback);
    IConfigurationStage IncludeConfiguration(IConfigurationModule module);
    IConfigurationStage IncludeConfiguration<TModule>()
        where TModule : IConfigurationModule, new();
}