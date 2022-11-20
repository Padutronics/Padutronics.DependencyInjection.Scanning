using Padutronics.DependencyInjection.Registration.Fluent;
using System;

namespace Padutronics.DependencyInjection.Scanning;

public delegate void TypeRegistrationCallback(Type type, ILifetimeStage lifetimeStage);