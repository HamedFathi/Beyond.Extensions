// ReSharper disable CheckNamespace
// ReSharper disable UnusedMember.Global

namespace Beyond.Extensions.GuidExtended;

public static class GuidExtensions
{
    private static readonly int[] GuidByteOrder = { 15, 14, 13, 12, 11, 10, 9, 8, 6, 7, 4, 5, 0, 1, 2, 3 };

    public static Guid GetNewGuidIfEmpty(this Guid @this)
    {
        return @this == Guid.Empty ? Guid.NewGuid() : @this;
    }

    public static bool In(this Guid @this, params Guid[] values)
    {
        return Array.IndexOf(values, @this) != -1;
    }

    public static Guid Increment(this Guid guid)
    {
        var bytes = guid.ToByteArray();
        var carry = true;
        for (var i = 0; i < GuidByteOrder.Length && carry; i++)
        {
            var index = GuidByteOrder[i];
            var oldValue = bytes[index]++;
            carry = oldValue > bytes[index];
        }

        return new Guid(bytes);
    }

    public static bool IsEmpty(this Guid @this)
    {
        return @this == Guid.Empty;
    }

    public static bool IsNotEmpty(this Guid @this)
    {
        return @this != Guid.Empty;
    }

    public static bool NotIn(this Guid @this, params Guid[] values)
    {
        return Array.IndexOf(values, @this) == -1;
    }

    public static string ToUrlFriendlyString(this Guid guid)
    {
        var base64Guid = Convert.ToBase64String(guid.ToByteArray());
        base64Guid = base64Guid.Replace('+', '-').Replace('/', '_');
        return base64Guid[..^2];
    }
}