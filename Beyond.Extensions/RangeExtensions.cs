// ReSharper disable CheckNamespace
// ReSharper disable UnusedType.Global
// ReSharper disable UnusedMember.Global

namespace Beyond.Extensions;

[SuppressMessage("Design", "CA1050:Declare types in namespaces")]
[EditorBrowsable(EditorBrowsableState.Never)]
public static class RangeGlobalExtensions
{
    public static IEnumerator<int> GetEnumerator(this Range range)
    {
        if (range.Start.IsFromEnd)
            for (var i = range.Start.Value; i >= range.End.Value; i--)
                yield return i;
        else
            for (var i = range.Start.Value; i <= range.End.Value; i++)
                yield return i;
    }
}