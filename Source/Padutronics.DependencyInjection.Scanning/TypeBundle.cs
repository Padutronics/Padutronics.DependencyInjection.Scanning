using Padutronics.Reflection.Types.Selectors;
using System;
using System.Collections.Generic;

namespace Padutronics.DependencyInjection.Scanning;

public sealed class TypeBundle
{
    private readonly Lazy<IEnumerable<Type>> abstractTypes;
    private readonly Lazy<IEnumerable<Type>> concreteTypes;
    private readonly Lazy<IEnumerable<Type>> interfaceTypes;
    private readonly IEnumerable<Type> types;

    public TypeBundle(IEnumerable<Type> types)
    {
        this.types = types;

        abstractTypes = new Lazy<IEnumerable<Type>>(() => SelectTypes(new AbstractTypeSelector()));
        concreteTypes = new Lazy<IEnumerable<Type>>(() => SelectTypes(new ConcreteTypeSelector()));
        interfaceTypes = new Lazy<IEnumerable<Type>>(() => SelectTypes(new InterfaceTypeSelector()));
    }

    public IEnumerable<Type> AbstractTypes => abstractTypes.Value;

    public IEnumerable<Type> ConcreteTypes => concreteTypes.Value;

    public IEnumerable<Type> InterfaceTypes => interfaceTypes.Value;

    private IEnumerable<Type> SelectTypes(ITypeSelector typeSelector)
    {
        return typeSelector.SelectTypes(types);
    }
}