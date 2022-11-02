// ReSharper disable CheckNamespace
// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedType.Global

namespace Beyond.Extensions.HashSetExtended;

public static class HashSetExtensions
{
    public static bool AddRange<T>(this HashSet<T> @this, IEnumerable<T>? items)
    {
        if (items == null) return false;
        var allAdded = true;
        foreach (var item in items) allAdded &= @this.Add(item);
        return allAdded;
    }

    public static bool AddRange<T>(this HashSet<T> @this, params T[]? items)
    {
        if (items == null) return false;
        var allAdded = true;
        foreach (var item in items) allAdded &= @this.Add(item);
        return allAdded;
    }
}