// ReSharper disable CheckNamespace
// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedType.Global

using Beyond.Extensions.EnumerableExtended;

namespace Beyond.Extensions.CollectionExtended;

public static class CollectionExtensions
{
    public static bool AddIf<T>(this ICollection<T> @this, Func<T, bool> predicate, T value)
    {
        if (predicate(value))
        {
            @this.Add(value);
            return true;
        }

        return false;
    }

    public static bool AddIfNotContains<T>(this ICollection<T> @this, T value)
    {
        if (!@this.Contains(value))
        {
            @this.Add(value);
            return true;
        }

        return false;
    }

    public static void AddRange<T>(this ICollection<T> @this, params T[] values)
    {
        foreach (var value in values) @this.Add(value);
    }

    public static void AddRange<T>(this ICollection<T> collection, IEnumerable<T> values)
    {
        if (collection == null) throw new ArgumentNullException(nameof(collection));
        if (values == null) throw new ArgumentNullException(nameof(values));

        if (collection is List<T> list)
        {
            list.AddRange(values);
            return;
        }

        var set = collection as ISet<T>;
        if (set != null)
        {
            set.UnionWith(values);
            return;
        }

        foreach (var item in values) collection.Add(item);
    }

    public static void AddRangeIf<T>(this ICollection<T> @this, Func<T, bool> predicate, params T[] values)
    {
        foreach (var value in values)
            if (predicate(value))
                @this.Add(value);
    }

    public static void AddRangeIfNotContains<T>(this ICollection<T> @this, params T[] values)
    {
        foreach (var value in values)
            if (!@this.Contains(value))
                @this.Add(value);
    }

    public static bool AddRangeUnique<T>(this ICollection<T> collection, T value)
    {
        var alreadyHas = collection.Contains(value);
        if (!alreadyHas) collection.Add(value);
        return alreadyHas;
    }

    public static int AddRangeUnique<T>(this ICollection<T> collection, IEnumerable<T> values)
    {
        var count = 0;
        foreach (var value in values)
            if (collection.AddRangeUnique(value))
                count++;
        return count;
    }

    public static bool AddRangeUniqueIf<T>(this ICollection<T> collection, Func<T, bool> predicate, T value)
    {
        var alreadyHas = collection.Contains(value);
        if (!alreadyHas && predicate(value))
        {
            collection.Add(value);
            return true;
        }

        return false;
    }

    public static bool AllSafe<T>(this ICollection<T> collection, Func<T, bool> predicate)
    {
        return collection.All(predicate);
    }

    public static bool AllSafe<T>(this ICollection<T> collection)
    {
        return collection.All();
    }

    public static bool AnySafe<T>(this ICollection<T> collection, Func<T, bool> predicate)
    {
        return collection.Any(predicate);
    }

    public static bool AnySafe<T>(this ICollection<T> collection)
    {
        return collection.Any();
    }

    public static bool ContainsAll<T>(this ICollection<T> @this, params T[] values)
    {
        foreach (var value in values)
            if (!@this.Contains(value))
                return false;

        return true;
    }

    public static bool ContainsAll<T>(this ICollection<T> @this, IEnumerable<T> values)
    {
        foreach (var value in values)
            if (!@this.Contains(value))
                return false;

        return true;
    }

    public static bool ContainsAny<T>(this ICollection<T> @this, params T[] values)
    {
        foreach (var value in values)
            if (@this.Contains(value))
                return true;

        return false;
    }

    public static bool ContainsAny<T>(this ICollection<T> @this, IEnumerable<T> values)
    {
        foreach (var value in values)
            if (@this.Contains(value))
                return true;

        return false;
    }

    public static IEnumerable<T> Distinct<T>(this IEnumerable<T> source, Func<T, T, bool> equalityComparer)
    {
        if (source == null) throw new ArgumentNullException(nameof(source));
        if (equalityComparer == null) throw new ArgumentNullException(nameof(equalityComparer));

        var enumerable = source.ToList();
        var sourceCount = enumerable.Count;
        for (var i = 0; i < sourceCount; i++)
        {
            var found = false;
            for (var j = 0; j < i; j++)
                if (equalityComparer(enumerable.ElementAt(i), enumerable.ElementAt(j)))
                {
                    found = true;
                    break;
                }

            if (!found)
                yield return enumerable.ElementAt(i);
        }
    }

    public static bool IsEmpty(this ICollection collection)
    {
        if (collection == null)
            throw new ArgumentNullException(nameof(collection));
        return collection.Count == 0;
    }

    public static bool IsEmpty<T>(this ICollection<T> collection)
    {
        if (collection == null)
            throw new ArgumentNullException(nameof(collection));

        return collection.Count == 0;
    }

    public static bool IsNotEmpty<T>(this ICollection<T> @this)
    {
        return @this.Count != 0;
    }

    public static bool IsNotNullOrEmpty<T>(this ICollection<T>? @this)
    {
        return @this != null && @this.Count != 0;
    }

    public static void Remove<T>(this ICollection<T> collection, Func<T, bool> predicate)
    {
        for (var i = collection.Count - 1; i >= 0; i--)
        {
            var element = collection.ElementAt(i);
            if (predicate(element)) collection.Remove(element);
        }
    }

    public static void RemoveAll<T>(this ICollection<T> collection, IEnumerable<T> values)
    {
        if (collection == null) throw new ArgumentNullException(nameof(collection));
        if (values == null) throw new ArgumentNullException(nameof(values));

        foreach (var value in values) collection.Remove(value);
    }

    public static IEnumerable<T> RemoveDuplicates<T>(this ICollection<T> @this)
    {
        return new HashSet<T>(@this).ToList();
    }

    public static IEnumerable<T> RemoveDuplicates<T>(this ICollection<T> list, Func<T, int> predicate)
    {
        var dict = new Dictionary<int, T>();

        foreach (var item in list)
            if (!dict.ContainsKey(predicate(item)))
                dict.Add(predicate(item), item);

        return dict.Values.AsEnumerable();
    }

    public static void RemoveIf<T>(this ICollection<T> @this, T value, Func<T, bool> predicate)
    {
        if (predicate(value)) @this.Remove(value);
    }

    public static void RemoveIfContains<T>(this ICollection<T> @this, T value)
    {
        if (@this.Contains(value)) @this.Remove(value);
    }

    public static void RemoveRange<T>(this ICollection<T> @this, params T[] values)
    {
        foreach (var value in values) @this.Remove(value);
    }

    public static void RemoveRange<T>(this ICollection<T> @this, IEnumerable<T> values)
    {
        foreach (var value in values) @this.Remove(value);
    }

    public static void RemoveRangeIf<T>(this ICollection<T> @this, Func<T, bool> predicate, params T[] values)
    {
        foreach (var value in values)
            if (predicate(value))
                @this.Remove(value);
    }

    public static void RemoveRangeIfContains<T>(this ICollection<T> @this, params T[] values)
    {
        foreach (var value in values)
            if (@this.Contains(value))
                @this.Remove(value);
    }

    public static void RemoveWhere<T>(this ICollection<T> collection, Func<T, bool> predicate)
    {
        if (collection == null)
            throw new ArgumentNullException(nameof(collection));
        var deleteList = collection.Where(predicate).ToList();
        deleteList.ForEach(t => collection.Remove(t));
    }

    public static IEnumerable<IEnumerable<T>> Split<T>(this ICollection<T> source, int chunkSize)
    {
        return source
            .Select((x, i) => new { Index = i, Value = x })
            .GroupBy(x => x.Index / chunkSize)
            .Select(x => x.Select(v => v.Value));
    }

    public static T[] ToArray<T>(this ICollection collection)
    {
        var array = new T[collection.Count];
        var index = 0;

        foreach (T item in collection) array[index++] = item;

        return array;
    }

    public static TK[] ToArray<T, TK>(this ICollection<T> collection, Converter<T, TK> converter)
    {
        var array = new TK[collection.Count];
        var index = 0;

        foreach (var item in collection) array[index++] = converter(item);

        return array;
    }

    public static IEnumerable<T> ToPaged<T>(this ICollection<T> query, int pageIndex, int pageSize)
    {
        return query
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize);
    }

    public static TList With<TList, T>(this TList list, T item) where TList : ICollection<T>, new()
    {
        var l = new TList();

        foreach (var i in list) l.Add(i);
        l.Add(item);

        return l;
    }

    public static TList Without<TList, T>(this TList list, T item) where TList : ICollection<T>, new()
    {
        var l = new TList();

        foreach (var i in list.Where(n => n != null && !n.Equals(item))) l.Add(i);

        return l;
    }
}