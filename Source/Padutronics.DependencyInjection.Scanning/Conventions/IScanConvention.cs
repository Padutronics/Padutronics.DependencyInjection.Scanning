using Padutronics.DependencyInjection.Registration.Fluent;
using System;

namespace Padutronics.DependencyInjection.Scanning.Conventions;

public interface IScanConvention
{
    void Scan(TypeRegistry typeRegistry, IContainerBuilder containerBuilder, Action<Type, ILifetimeStage> registrationCallback);
}