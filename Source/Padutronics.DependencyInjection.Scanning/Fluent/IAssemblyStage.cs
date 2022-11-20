using System;
using System.Reflection;

namespace Padutronics.DependencyInjection.Scanning.Fluent;

public interface IAssemblyStage
{
    IConventionStage Assembly(string assemblyName);
    IConventionStage Assembly(Assembly assembly);
    IConventionStage AssemblyContaining(Type type);
    IConventionStage AssemblyContaining<T>();
}