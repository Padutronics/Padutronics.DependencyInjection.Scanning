using Padutronics.DependencyInjection.Scanning.Filters;
using System;

namespace Padutronics.DependencyInjection.Scanning.Fluent;

public interface IFilterStage : IConventionStage
{
    IFilterStage Exclude(Predicate<Type> predicate);
    IFilterStage Exclude(ITypeFilter filter);
    IFilterStage Exclude<TFilter>()
        where TFilter : ITypeFilter, new();
    IFilterStage Include(Predicate<Type> predicate);
    IFilterStage Include(ITypeFilter filter);
    IFilterStage Include<TFilter>()
        where TFilter : ITypeFilter, new();
}