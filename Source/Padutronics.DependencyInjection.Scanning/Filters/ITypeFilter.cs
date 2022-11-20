using System;

namespace Padutronics.DependencyInjection.Scanning.Filters;

public interface ITypeFilter
{
    bool IsValid(Type type);
}