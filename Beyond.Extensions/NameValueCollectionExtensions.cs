// ReSharper disable CheckNamespace
// ReSharper disable UnusedMember.Global

namespace Beyond.Extensions.NameValueCollectionExtended;

public static class NameValueCollectionExtensions
{
    public static IDictionary<string, object?> ToDictionary(this NameValueCollection @this)
    {
        if (@this == null) throw new ArgumentNullException(nameof(@this));
        var dict = new Dictionary<string, object?>();

        foreach (var key in @this.AllKeys)
            if (key != null)
                dict.Add(key, @this[key]);

        return dict;
    }

    public static IEnumerable<KeyValuePair<string, string?>> ToKeyValuePairs(this NameValueCollection collection)
    {
        if (collection is null) throw new ArgumentNullException(nameof(collection));

        foreach (string key in collection.Keys)
            yield return new KeyValuePair<string, string?>(key, collection[key]);
    }
}