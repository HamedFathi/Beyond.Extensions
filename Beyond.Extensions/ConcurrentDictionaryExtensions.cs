// ReSharper disable CheckNamespace
// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedType.Global

namespace Beyond.Extensions.ConcurrentDictionaryExtended;

public static class ConcurrentDictionaryExtensions
{
    public static TValue AddOrUpdate<TKey, TValue>(this ConcurrentDictionary<TKey, Lazy<TValue>> @this, TKey key,
        Func<TKey, TValue> addValueFactory, Func<TKey, TValue, TValue> updateValueFactory) where TKey : notnull
    {
        return @this.AddOrUpdate(key, new Lazy<TValue>(() => addValueFactory(key), true),
            (k, v) => new Lazy<TValue>(() => updateValueFactory(k, v.Value), true)).Value;
    }

    public static void AddOrUpdate<TKey, TValue>(this ConcurrentDictionary<TKey, TValue> @this, TKey key, TValue value)
        where TKey : notnull
    {
        @this.AddOrUpdate(key, value, (_, _) => value);
    }

    public static TValue GetOrAdd<TKey, TValue>(this ConcurrentDictionary<TKey, Lazy<TValue>> @this, TKey key,
        Func<TKey, TValue> valueFactory) where TKey : notnull
    {
        return @this.GetOrAdd(key, new Lazy<TValue>(() => valueFactory(key), true)).Value;
    }

    public static TValue GetOrAdd<TKey, TValue>(this ConcurrentDictionary<TKey, Lazy<TValue>> @this, TKey key,
        TValue value) where TKey : notnull
    {
        return @this.GetOrAdd(key, new Lazy<TValue>(() => value, true)).Value;
    }

    public static Dictionary<TKey, TValue> ToDictionary<TKey, TValue>(this ConcurrentDictionary<TKey, TValue> @this)
        where TKey : notnull
    {
        return @this.ToDictionary(entry => entry.Key, entry => entry.Value);
    }
}