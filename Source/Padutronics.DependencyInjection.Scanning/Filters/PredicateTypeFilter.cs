using System;

namespace Padutronics.DependencyInjection.Scanning.Filters;

internal sealed class PredicateTypeFilter : ITypeFilter
{
    private readonly Predicate<Type> predicate;

    public PredicateTypeFilter(Predicate<Type> predicate)
    {
        this.predicate = predicate;
    }

    public bool IsValid(Type type)
    {
        return predicate(type);
    }
}