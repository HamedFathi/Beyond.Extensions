// ReSharper disable CheckNamespace
// ReSharper disable UnusedType.Global
// ReSharper disable UnusedMember.Global

namespace Beyond.Extensions;

public static class EnumeratorExtensions
{
    public static IEnumerable<T> ToIEnumerable<T>(this IEnumerator<T> enumerator)
    {
        while (enumerator.MoveNext())
        {
            yield return enumerator.Current;
        }
    }
}

[SuppressMessage("Design", "CA1050:Declare types in namespaces")]
public static class EnumeratorGlobalExtensions
{
    public static IEnumerator<T> GetEnumerator<T>(this IEnumerator<T> enumerator)
    {
        return enumerator;
    }

    public static IEnumerator<int> GetEnumerator(this int input)
    {
        if (input >= 0)
            for (var i = 0; i <= input; i++)
                yield return i;
        else
            for (var i = input; i <= 0; i++)
                yield return i;
    }

    public static IEnumerator<int> GetEnumerator(this (int from, int to) range)
    {
        for (var i = range.from; i < range.to; i++) yield return i;
    }

    public static IEnumerator<int> GetEnumerator(this (int from, int to, int step) range)
    {
        for (var i = range.from; i < range.to; i += range.step) yield return i;
    }
}