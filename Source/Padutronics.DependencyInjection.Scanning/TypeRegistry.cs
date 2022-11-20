using Padutronics.Reflection.Types.Selectors;
using System;
using System.Collections.Generic;

namespace Padutronics.DependencyInjection.Scanning;

public sealed class TypeRegistry
{
    private readonly Lazy<TypeBundle> closedTypes;
    private readonly Lazy<TypeBundle> openTypes;

    public TypeRegistry(IEnumerable<Type> allTypes)
    {
        AllTypes = allTypes;

        closedTypes = new Lazy<TypeBundle>(() => CreateAssemblyPart(new ClosedTypeSelector()));
        openTypes = new Lazy<TypeBundle>(() => CreateAssemblyPart(new OpenTypeSelector()));
    }

    public IEnumerable<Type> AllTypes { get; }

    public TypeBundle ClosedTypes => closedTypes.Value;

    public TypeBundle OpenTypes => openTypes.Value;

    private TypeBundle CreateAssemblyPart(ITypeSelector typeSelector)
    {
        return new TypeBundle(typeSelector.SelectTypes(AllTypes));
    }
}