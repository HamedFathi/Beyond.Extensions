// ReSharper disable CheckNamespace
// ReSharper disable UnusedMember.Global
namespace Beyond.Extensions.ComponentExtended;

public static class ComponentExtensions
{
    public static bool IsInDesignMode(this IComponent target)
    {
        var site = target.Site;
        return site is { DesignMode: true };
    }

    public static bool IsInRuntimeMode(this IComponent target)
    {
        return !IsInDesignMode(target);
    }
}