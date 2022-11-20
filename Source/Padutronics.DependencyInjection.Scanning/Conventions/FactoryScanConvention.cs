using System;
using System.Text.RegularExpressions;

namespace Padutronics.DependencyInjection.Scanning.Conventions;

internal sealed class FactoryScanConvention : IScanConvention
{
    private readonly string typeNamePattern;

    public FactoryScanConvention() :
        this("Factory$")
    {
    }

    public FactoryScanConvention(string typeNamePattern)
    {
        this.typeNamePattern = typeNamePattern;
    }

    public void Scan(TypeRegistry typeRegistry, IContainerBuilder containerBuilder, TypeRegistrationCallback registrationCallback)
    {
        var regularExpression = new Regex(typeNamePattern);

        foreach (Type type in typeRegistry.ClosedTypes.InterfaceTypes)
        {
            if (regularExpression.IsMatch(type.Name))
            {
                containerBuilder.For(type).UseFactory();
            }
        }
    }
}