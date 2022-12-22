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

    public static async Task<TValue> GetOrAddAsync<TKey, TValue>(
        this ConcurrentDictionary<TKey, Task<TValue>> dictionary,
        TKey key,
        Func<TKey, Task<TValue>> valueFactory) where TKey : notnull
    {
        if (dictionary == null) throw new ArgumentNullException(nameof(dictionary));
        while (true)
        {
            if (dictionary.TryGetValue(key, out var task))
            {
                return await task;
            }

            var tcs = new TaskCompletionSource<TValue>(TaskCreationOptions.RunContinuationsAsynchronously);
            if (dictionary.TryAdd(key, tcs.Task))
            {
                try
                {
                    var value = await valueFactory(key);
                    tcs.TrySetResult(value);
                    return await tcs.Task;
                }
                catch (Exception ex)
                {
                    tcs.SetException(ex);

                    dictionary.TryRemove(key, out _);
                    throw;
                }
            }
        }
    }

    public static TValue GetOrAddWithDispose<TKey, TValue>(
                            this ConcurrentDictionary<TKey, TValue> dictionary,
        [DisallowNull] TKey key,
        Func<TKey, TValue> valueFactory) where TValue : IDisposable where TKey : notnull
    {
        if (dictionary == null) throw new ArgumentNullException(nameof(dictionary));
        if (key == null) throw new ArgumentNullException(nameof(key));
        if (valueFactory == null) throw new ArgumentNullException(nameof(valueFactory));
        while (true)
        {
            if (dictionary.TryGetValue(key, out var value))
            {
                return value;
            }

            value = valueFactory(key);
            if (dictionary.TryAdd(key, value))
            {
                return value;
            }

            value.Dispose();
        }
    }

    public static Dictionary<TKey, TValue> ToDictionary<TKey, TValue>(this ConcurrentDictionary<TKey, TValue> @this)
        where TKey : notnull
    {
        return @this.ToDictionary(entry => entry.Key, entry => entry.Value);
    }
}