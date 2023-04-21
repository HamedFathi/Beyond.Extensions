// ReSharper disable CheckNamespace
// ReSharper disable UnusedMember.Global
// ReSharper disable PossibleMultipleEnumeration
// ReSharper disable IdentifierTypo
// ReSharper disable StringLiteralTypo

using Beyond.Extensions.StringExtended;
using Beyond.Extensions.Types;

namespace Beyond.Extensions.EnumerableExtended;

public static class EnumerableExtensions
{
    private static readonly Random Rnd = new(Guid.NewGuid().GetHashCode());

    public static string Aggregate<T>(this IEnumerable<T> enumeration, Func<T, string> toString, string separator)
    {
        if (toString == null)
            throw new ArgumentNullException(nameof(toString));

        return Aggregate(enumeration.Select(toString), separator);
    }

    public static string Aggregate(this IEnumerable<string> enumeration, string separator)
    {
        if (enumeration == null)
            throw new ArgumentNullException(nameof(enumeration));
        if (separator == null)
            throw new ArgumentNullException(nameof(separator));
        var returnValue = string.Join(separator, enumeration.ToArray());
        if (returnValue.Length > separator.Length)
            return returnValue.Substring(separator.Length);
        return returnValue;
    }

    public static bool All<T>(this IEnumerable<T> @this, params T[] values)
    {
        var list = @this.ToArray();
        return values.All(value => list.Contains(value));
    }

    public static bool AllSafe<T>(this IEnumerable<T>? enumerable, Func<T, bool> predicate)
    {
        return enumerable?.All(predicate) == true;
    }

    public static bool AllSafe<T>(this IEnumerable<T>? enumerable)
    {
        return enumerable?.All() == true;
    }

    public static bool Any(this IEnumerable source)
    {
        return Enumerable.Any(source.Cast<object>());
    }

    public static bool Any<T>(this IEnumerable<T> @this, params T[] values)
    {
        var list = @this.ToArray();
        foreach (var value in values)
            if (list.Contains(value))
                return true;

        return false;
    }

    public static bool Any<TSource>(this IEnumerable<TSource> source, Func<TSource, int, bool> predicate)
    {
        if (source == null) throw new ArgumentNullException(nameof(source));
        if (predicate == null) throw new ArgumentNullException(nameof(predicate));

        var index = 0;
        foreach (var item in source)
        {
            if (predicate(item, index))
                return true;

            index++;
        }

        return false;
    }

    public static bool AnyOfType<TSource>(this IEnumerable source)
    {
        return source.OfType<TSource>().Any();
    }

    public static bool AnyOrNotNull(this IEnumerable<string> source)
    {
        var enumerable = source.ToList();
        var hasData = enumerable.Aggregate((a, b) => a + b).Any();
        if (enumerable.Any() && hasData)
            return true;
        return false;
    }

    public static bool AnySafe<T>(this IEnumerable<T>? enumerable, Func<T, bool> predicate)
    {
        return enumerable?.Any(predicate) == true;
    }

    public static bool AnySafe<T>(this IEnumerable<T>? enumerable)
    {
        return enumerable?.Any() == true;
    }

    public static bool AnySafe(this IEnumerable? source)
    {
        return source != null && Enumerable.Any(source.Cast<object>());
    }

    public static IEnumerable<T> Append<T>(this IEnumerable<T> source, T element)
    {
        foreach (var item in source) yield return item;
        yield return element;
    }

    public static bool AreAllSame<T>(this IEnumerable<T> enumerable)
    {
        if (enumerable == null) throw new ArgumentNullException(nameof(enumerable));

        using var enumerator = enumerable.GetEnumerator();
        var toCompare = default(T);
        if (enumerator.MoveNext()) toCompare = enumerator.Current;

        while (enumerator.MoveNext())
            if (toCompare != null && !toCompare.Equals(enumerator.Current))
                return false;

        return true;
    }

    public static bool AreItemsUnique<T>(this IEnumerable<T> items)
    {
        if (items == null) throw new ArgumentNullException(nameof(items));
        var enumerable = items.ToList();
        return enumerable.Count == enumerable.Distinct().Count();
    }

    public static IEnumerable<T?> AsNullable<T>(this IEnumerable<T> enumeration) where T : struct
    {
        return from item in enumeration
               select new T?(item);
    }

    public static ReadOnlyCollection<T> AsReadOnlyCollection<T>(this IEnumerable<T>? @this)
    {
        if (@this != null)
            return new ReadOnlyCollection<T>(new List<T>(@this));
        throw new ArgumentNullException(nameof(@this));
    }

    public static Span<T> AsSpan<T>(this IEnumerable<T>? list)
    {
        return CollectionsMarshal.AsSpan(list?.ToList());
    }

    public static T At<T>(this IEnumerable<T> enumeration, int index)
    {
        if (enumeration == null)
            throw new ArgumentNullException(nameof(enumeration));
        return enumeration.Skip(index).First();
    }

    public static IEnumerable<T> At<T>(this IEnumerable<T> enumeration, params int[] indices)
    {
        return At(enumeration, (IEnumerable<int>)indices);
    }

    public static IEnumerable<T> At<T>(this IEnumerable<T> enumeration, IEnumerable<int> indices)
    {
        if (enumeration == null)
            throw new ArgumentNullException(nameof(enumeration));
        if (indices == null)
            throw new ArgumentNullException(nameof(indices));
        var currentIndex = 0;
        foreach (var index in indices.OrderBy(i => i))
        {
            while (currentIndex != index)
            {
                enumeration = enumeration.Skip(1);
                currentIndex++;
            }

            yield return enumeration.First();
        }
    }

    public static bool AtLeast<T>(this IEnumerable<T> enumeration, int count)
    {
        if (enumeration == null)
            throw new ArgumentNullException(nameof(enumeration));

        return enumeration.Count() >= count;
    }

    public static bool AtLeast<T>(this IEnumerable<T> enumeration, Func<T, bool> predicate, int count)
    {
        if (enumeration == null)
            throw new ArgumentNullException(nameof(enumeration));

        return enumeration.Count(predicate) >= count;
    }

    public static bool AtMost<T>(this IEnumerable<T> enumeration, int count)
    {
        if (enumeration == null)
            throw new ArgumentNullException(nameof(enumeration));

        return enumeration.Count() <= count;
    }

    public static bool AtMost<T>(this IEnumerable<T> enumeration, Func<T, bool> predicate, int count)
    {
        if (enumeration == null)
            throw new ArgumentNullException(nameof(enumeration));

        return enumeration.Count(predicate) <= count;
    }

    public static BigInteger BigIntCount<T>(this IEnumerable<T> source)
    {
        BigInteger count = 0;
        foreach (var _ in source) count++;
        return count;
    }

    public static IEnumerable<(T1 left, T2 right)> Cartesian<T1, T2>(this (IEnumerable<T1>, IEnumerable<T2>) seqs)
    {
        foreach (var a in seqs.Item1)
            foreach (var b in seqs.Item2)
                yield return (a, b);
    }

    public static IEnumerable<(T1 left, T2 right)> Cartesian<T1, T2>(this IEnumerable<T1> left, IEnumerable<T2> right)
    {
        return (left, right).Cartesian();
    }

    public static IEnumerable<IEnumerable<T>> Combinations<T>(this IEnumerable<T> source, int select,
        bool repetition = false)
    {
        if (source == null || select < 0)
            throw new ArgumentNullException();
        return select == 0
            ? new[] { Array.Empty<T>() }
            : source.SelectMany((element, index) =>
                source
                    .Skip(repetition ? index : index + 1)
                    .Combinations(select - 1, repetition)
                    .Select(c => new[] { element }.Concat(c)));
    }

    public static IEnumerable<T> Concat<T>(IEnumerable<T> target, T element)
    {
        foreach (var e in target) yield return e;
        yield return element;
    }

    public static string Concatenate(this IEnumerable<string> @this)
    {
        var sb = new StringBuilder();
        foreach (var s in @this) sb.Append(s);
        return sb.ToString();
    }

    public static string Concatenate<T>(this IEnumerable<T> source, Func<T, string> func)
    {
        var sb = new StringBuilder();
        foreach (var item in source) sb.Append(func(item));
        return sb.ToString();
    }

    public static IDictionary<TValue, TKey> ConcatToDictionary<TValue, TKey>(
        this IEnumerable<KeyValuePair<TValue, TKey>> first, IEnumerable<KeyValuePair<TValue, TKey>> second)
        where TValue : notnull
    {
        if (first is null) throw new ArgumentNullException(nameof(first));

        if (second is null) throw new ArgumentNullException(nameof(second));

        return first
            .Concat(second)
            .ToDictionary(x => x.Key, x => x.Value);
    }

    public static IDictionary<TValue, TKey> ConcatToDictionarySafe<TValue, TKey>(
        this IEnumerable<KeyValuePair<TValue, TKey>> first, IEnumerable<KeyValuePair<TValue, TKey>> second)
        where TValue : notnull
    {
        if (first is null) throw new ArgumentNullException(nameof(first));

        if (second is null) throw new ArgumentNullException(nameof(second));

        return first
            .Concat(second)
            .GroupBy(x => x.Key)
            .ToDictionary(x => x.Key,
                x => x.First()
                    .Value);
    }

    public static string ConcatWith<T>(this IEnumerable<T> items, string separator = ",", string formatString = "")
    {
        if (items == null) throw new ArgumentNullException(nameof(items));
        if (separator == null) throw new ArgumentNullException(nameof(separator));
        if (typeof(T) == typeof(string)) return string.Join(separator, ((IEnumerable<string>)items).ToArray());
        formatString = string.IsNullOrEmpty(formatString) ? "{0}" : $"{{0:{formatString}}}";
        return string.Join(separator, items.Select(x => string.Format(formatString, x)).ToArray());
    }

    public static bool Contains<T>(this IEnumerable<T> collection, Func<T, bool> predicate)
    {
        return collection.Count(predicate) > 0;
    }

    public static bool ContainsIgnoreCase(this IEnumerable<string>? source, string str)
    {
        if (string.IsNullOrEmpty(str) || source == null || !source.Any())
            return false;
        foreach (var item in source) return item.ContainsIgnoreCase(str);
        return false;
    }

    public static int Count(this IEnumerable enumerable, bool excludeNullValues = false)
    {
        var list = enumerable.Cast<object?>();
        if (excludeNullValues) list = list.Where(x => x != null);
        return Enumerable.Count(list);
    }

    public static void Delete(this IEnumerable<FileInfo> @this)
    {
        foreach (var t in @this) t.Delete();
    }

    public static void Delete(this IEnumerable<DirectoryInfo> @this)
    {
        foreach (var t in @this) t.Delete();
    }

    public static IEnumerable<T> Distinct<T>(this IEnumerable<T> source, Func<T, T, bool> equalityComparer)
    {
        var sourceCount = source.Count();
        for (var i = 0; i < sourceCount; i++)
        {
            var found = false;
            for (var j = 0; j < i; j++)
                if (equalityComparer(source.ElementAt(i),
                        source.ElementAt(
                            j)))
                {
                    found = true;
                    break;
                }

            if (!found)
                yield return source.ElementAt(i);
        }
    }

    public static IEnumerable<TKey> Distinct<T, TKey>(this IEnumerable<T> source, Func<T, TKey> selector)
    {
        return source.GroupBy(selector).Select(x => x.Key);
    }

    public static IEnumerable<T> Distinct<T, TKey>(this IEnumerable<T> source, Func<T, TKey> keySelector,
        IEqualityComparer<TKey> keyComparer)
    {
        var keyHashSet = new HashSet<TKey>(keyComparer);
        foreach (var element in source)
            if (keyHashSet.Add(keySelector(element)))
                yield return element;
    }

    public static IEnumerable<T> DistinctBy<T, TKey>(this IEnumerable<T> items, Func<T, TKey> property)
    {
        return items.GroupBy(property).Select(x => x.First());
    }

    public static IEnumerable<T> DistinctBy<T>(this IEnumerable<T> list, Func<T, object> propertySelector)
    {
        return list.GroupBy(propertySelector).Select(x => x.First());
    }

    public static IEnumerable<T> EmptyIfNull<T>(this IEnumerable<T>? items)
    {
        return items ?? Enumerable.Empty<T>();
    }

    public static IEnumerable<string> EnumNamesToList<T>(this IEnumerable<T> collection)
    {
        var cls = typeof(T);
        var enumArrayList = cls.GetInterfaces();
        return (from objType in enumArrayList where objType.IsEnum select objType.Name).ToList();
    }

    [SuppressMessage("Roslynator", "RCS1175:Unused 'this' parameter.", Justification = "<Pending>")]
    [SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "<Pending>")]
    public static IEnumerable<T> EnumValuesToList<T>(this IEnumerable<T> collection)
    {
        var enumType = typeof(T);
        if (enumType.BaseType != typeof(Enum))
            throw new ArgumentException("T must be of type System.Enum");
        var enumValArray = Enum.GetValues(enumType);
        var enumValList = new List<T>(enumValArray.Length);
        enumValList.AddRange(from int val in enumValArray select (T)Enum.Parse(enumType, val.ToString()));
        return enumValList;
    }

    public static bool Exactly<T>(this IEnumerable<T> source, int count)
    {
        return source.Count() == count;
    }

    public static bool Exactly<T>(this IEnumerable<T> source, Func<T, bool> query, int count)
    {
        return source.Count(query) == count;
    }

    public static IEnumerable<T> Except<T>(this IEnumerable<T> source, T value, IEqualityComparer<T> comparer)
    {
        return source.Where(i => !comparer.Equals(i, value));
    }

    public static IEnumerable<TSource> Except<TSource>(this IEnumerable<TSource> source, TSource item)
    {
        if (source == null) throw new ArgumentNullException(nameof(source));

        foreach (var eachItem in source)
        {
            if (Equals(eachItem, item))
                continue;

            yield return eachItem;
        }
    }

    public static IEnumerable<T> ExceptDefault<T>(this IEnumerable<T> source)
    {
        return source.Except(default!);
    }

    public static IEnumerable<string> ExceptNullOrWhiteSpace(this IEnumerable<string> source)
    {
        return source.Where(s => !s.IsNullOrWhiteSpace());
    }

    public static bool Exists<T>(this IEnumerable<T> list, Func<T, bool> predicate)
    {
        return list.Index(predicate) > -1;
    }

    public static IEnumerable<TSource> FallbackIfEmpty<TSource>(this IEnumerable<TSource> source, IEnumerable<TSource> fallback)
    {
        if (source == null)
        {
            throw new ArgumentNullException(nameof(source));
        }

        if (fallback == null)
        {
            throw new ArgumentNullException(nameof(fallback));
        }

        return source.Any() ? source : fallback;
    }

    public static IEnumerable<TSource> FallbackIfEmpty<TSource>(this IEnumerable<TSource> source, params TSource[] fallback)
    {
        if (source == null)
        {
            throw new ArgumentNullException(nameof(source));
        }

        return source.Any() ? source : fallback;
    }

    public static IEnumerable<TSource> FallbackIfNull<TSource>(this IEnumerable<TSource> source, IEnumerable<TSource> fallback)
    {
        if (fallback == null)
        {
            throw new ArgumentNullException(nameof(fallback));
        }

        return source ?? fallback;
    }
    public static IEnumerable<TSource> FallbackIfNull<TSource>(this IEnumerable<TSource> source, params TSource[] fallback)
    {
        return source ?? fallback;
    }

    public static IEnumerable<TSource> FallbackIfNullOrEmpty<TSource>(this IEnumerable<TSource> source, IEnumerable<TSource> fallback)
    {
        return source.FallbackIfNull(fallback).FallbackIfEmpty(fallback);
    }
    public static IEnumerable<TSource> FallbackIfNullOrEmpty<TSource>(this IEnumerable<TSource> source, params TSource[] fallback)
    {
        return source.FallbackIfNull(fallback).FallbackIfEmpty(fallback);
    }
    public static List<T> FindAll<T>(this IEnumerable<T> list, Func<T, bool> predicate)
    {
        if (predicate == null) throw new ArgumentNullException(nameof(predicate));
        var found = new List<T>();
        using var enumerator = list.GetEnumerator();
        while (enumerator.MoveNext())
            if (predicate(enumerator.Current))
                found.Add(enumerator.Current);
        return found;
    }

    public static decimal FindClosest(this IEnumerable<decimal> sequence, decimal value)
    {
        var diffList = sequence.Select(x => new { n = x, diff = Math.Abs(x - value) });
        var result = diffList.First(x => x.diff == diffList.Select(y => y.diff).Min());
        return result.n;
    }

    public static long FindClosest(this IEnumerable<long> sequence, long value)
    {
        var diffList = sequence.Select(x => new { n = x, diff = Math.Abs(x - value) });
        var result = diffList.First(x => x.diff == diffList.Select(y => y.diff).Min());
        return result.n;
    }

    public static double FindClosest(this IEnumerable<double> sequence, double value)
    {
        if (sequence == null) throw new ArgumentNullException(nameof(sequence));

        var diffList = sequence.Select(x => new { n = x, diff = Math.Abs(x - value) });
        var result = diffList.First(x => x.diff == diffList.Select(y => y.diff).Min());
        return result.n;
    }

    public static int FindClosest(this IEnumerable<int> sequence, int value)
    {
        if (sequence == null) throw new ArgumentNullException(nameof(sequence));
        var diffList = sequence.Select(x => new { n = x, diff = Math.Abs(x - value) });
        var result = diffList.First(x => x.diff == diffList.Select(y => y.diff).Min());
        return result.n;
    }

    public static decimal FindFarthest(this IEnumerable<decimal> sequence, decimal value)
    {
        var diffList = sequence.Select(x => new { n = x, diff = Math.Abs(x - value) });
        var result = diffList.First(x => x.diff == diffList.Select(y => y.diff).Max());
        return result.n;
    }

    public static long FindFarthest(this IEnumerable<long> sequence, long value)
    {
        var diffList = sequence.Select(x => new { n = x, diff = Math.Abs(x - value) });
        var result = diffList.First(x => x.diff == diffList.Select(y => y.diff).Max());
        return result.n;
    }

    public static double FindFarthest(this IEnumerable<double> sequence, double value)
    {
        var diffList = sequence.Select(x => new { n = x, diff = Math.Abs(x - value) });
        var result = diffList.First(x => x.diff == diffList.Select(y => y.diff).Max());
        return result.n;
    }

    public static int FindFarthest(this IEnumerable<int> sequence, int value)
    {
        var diffList = sequence.Select(x => new { n = x, diff = Math.Abs(x - value) });
        var result = diffList.First(x => x.diff == diffList.Select(y => y.diff).Max());
        return result.n;
    }

    public static TSource FirstOfType<TSource>(this IEnumerable source)
    {
        return source.OfType<TSource>().First();
    }

    public static T? FirstOr<T>(this IEnumerable<T> @this, Func<T, bool> predicate, Func<T> onOr)
    {
        var found = @this.FirstOrDefault(predicate);
        if (found != null && found.Equals(default(T))) found = onOr();
        return found;
    }

    public static T FirstOrDefault<T>(this IEnumerable<T> source, T defaultValue)
    {
        return source.IsNotEmpty() ? source.First() : defaultValue;
    }

    public static TSource? FirstOrDefaultOfType<TSource>(this IEnumerable source)
    {
        return source.OfType<TSource>().FirstOrDefault();
    }

    public static IEnumerable<T> Flatten<T>(this T source,
        Func<T, IEnumerable<T>> childrenSelector,
        Func<T, object> keySelector) where T : class
    {
        return Flatten(new[] { source }, childrenSelector, keySelector);
    }

    public static IEnumerable<T> Flatten<T>(this IEnumerable<T> source,
        Func<T, IEnumerable<T>?>? getChildren,
        Func<T, object> keySelector) where T : class
    {
        if (source == null) throw new ArgumentNullException(nameof(source));
        if (getChildren == null) throw new ArgumentNullException(nameof(getChildren));
        if (keySelector == null) throw new ArgumentNullException(nameof(keySelector));

        var stack = new Stack<T>(source);
        var dictionary = new Dictionary<object, T>();
        while (stack.Any())
        {
            var currentItem = stack.Pop();
            var currentKey = keySelector(currentItem);
            if (!dictionary.ContainsKey(currentKey))
            {
                dictionary.Add(currentKey, currentItem);
                var children = getChildren(currentItem);
                if (children != null)
                {
                    foreach (var child in children)
                    {
                        stack.Push(child);
                    }
                }
            }

            yield return currentItem;
        }
    }

    public static string Flatten(this IEnumerable<string>? strings, string separator, string head, string tail)
    {
        if (strings == null || !strings.Any())
            return string.Empty;

        var flattenedString = new StringBuilder();

        flattenedString.Append(head);
        foreach (var s in strings)
            flattenedString.AppendFormat("{0}{1}", s, separator);
        flattenedString.Remove(flattenedString.Length - separator.Length, separator.Length);
        flattenedString.Append(tail);

        return flattenedString.ToString();
    }

    public static string Flatten(this IEnumerable<string> strings, string prefix, string suffix, string head,
        string tail)
    {
        return strings
            .Select(s => $"{prefix}{s}{suffix}")
            .Flatten(string.Empty, head, tail);
    }

    public static IEnumerable<T> FlattenByLinq<T>(this IEnumerable<T> items, Func<T, IEnumerable<T>?>? getChildren)
    {
        return items.SelectMany(c => (getChildren?.Invoke(c) ?? Array.Empty<T>()).FlattenByLinq(getChildren))
            .Concat(items);
    }

    public static IEnumerable<T> FlattenByQueue<T>(this IEnumerable<T> items,
        Func<T, IEnumerable<T>?>? getChildren)
    {
        // Queue => BFS (Breadth First Search)
        var itemsToYield = new Queue<T>(items);
        while (itemsToYield.Count > 0)
        {
            var item = itemsToYield.Dequeue();
            yield return item;

            var children = getChildren?.Invoke(item);
            if (children != null)
                foreach (var child in children)
                {
                    if (child != null)
                        itemsToYield.Enqueue(child);
                }
        }
    }

    public static IEnumerable<T> FlattenByStack<T>(this IEnumerable<T> items,
        Func<T, IEnumerable<T>?>? getChildren)
    {
        // Stack => DFS (Depth First Search)
        var stack = new Stack<T>();
        foreach (var item in items)
            stack.Push(item);

        while (stack.Count > 0)
        {
            var current = stack.Pop();
            yield return current;

            var children = getChildren?.Invoke(current);
            if (children != null)
                foreach (var child in children)
                {
                    if (child != null)
                        stack.Push(child);
                }
        }
    }

    public static IEnumerable<T> FlattenObject<T>(T root, Func<T, IEnumerable<T>?>? getChildren)
    {
        if (root == null)
        {
            yield break;
        }

        yield return root;

        var children = getChildren?.Invoke(root);
        if (children == null)
        {
            yield break;
        }

        foreach (var child in children)
        {
            if (child != null)
            {
                foreach (var node in FlattenObject(child, getChildren))
                {
                    yield return node;
                }
            }
        }
    }

    public static IEnumerable<T> FlattenRecursively<T>(this IEnumerable<T> items, Func<T, IEnumerable<T>?>? getChildren)
    {
        foreach (var item in items)
        {
            yield return item;

            var children = getChildren?.Invoke(item);
            if (children == null)
                continue;

            foreach (var child in children.FlattenRecursively(getChildren))
            {
                if (child != null)
                    yield return child;
            }
        }
    }

    public static IEnumerable<FileInfo> ForEach(this IEnumerable<FileInfo> @this, Action<FileInfo> action)
    {
        foreach (var t in @this) action(t);
        return @this;
    }

    public static IEnumerable<DirectoryInfo> ForEach(this IEnumerable<DirectoryInfo> @this,
        Action<DirectoryInfo> action)
    {
        foreach (var t in @this) action(t);
        return @this;
    }

    public static void ForEach<T>(this IEnumerable<T> collection, Action<T> action)
    {
        if (collection == null) throw new ArgumentNullException(nameof(collection));
        if (action == null) throw new ArgumentNullException(nameof(action));
        foreach (var item in collection) action(item);
    }

    public static void ForEach<T>(this IEnumerable<T> collection, Action<T> action, Func<T, bool> predicate)
    {
        if (collection == null) throw new ArgumentNullException(nameof(collection));
        if (action == null) throw new ArgumentNullException(nameof(action));
        foreach (var item in collection.Where(predicate)) action(item);
    }

    public static void ForEach(this IEnumerable collection, Action<object> action)
    {
        if (collection == null) throw new ArgumentNullException(nameof(collection));
        if (action == null) throw new ArgumentNullException(nameof(action));
        foreach (var item in collection) action(item);
    }

    public static void ForEach<T>(this IEnumerable<T>? items, Action<int, T> action)
    {
        if (items != null)
        {
            var i = 0;
            foreach (var item in items)
                action(i++, item);
        }
    }

    public static void ForEach<T>(this IEnumerable<T> @this, Action<T, int> action)
    {
        var array = @this.ToArray();
        for (var i = 0; i < array.Length; i++)
            action(array[i], i);
    }

    public static void ForEachOrBreak<T>(this IEnumerable<T> source, Func<T, bool> breakFunc)
    {
        foreach (var item in source)
        {
            var result = breakFunc(item);
            if (result) break;
        }
    }

    public static void ForEachReverse<T>(this IEnumerable<T> source, Action<T> action)
    {
        var items = source.ToList();
        for (var i = items.Count - 1; i >= 0; i--)
            action(items[i]);
    }

    public static void ForEachReverse<T>(this IEnumerable<T> source, Action<T> action, Func<T, bool> predicate)
    {
        var items = source.ToList();
        for (var i = items.Count - 1; i >= 0; i--)
            if (predicate(items[i]))
                action(items[i]);
    }

    public static void ForEachReverse<T>(this IEnumerable<T> @this, Action<T, int> action)
    {
        var array = @this.ToArray();
        for (var i = array.Length - 1; i >= 0; i--)
            action(array[i], i);
    }

    public static IEnumerable<T> GetDuplicates<T>(this IEnumerable<T> @this)
    {
        var hashset = new HashSet<T>();
        return @this.Where(e => !hashset.Add(e));
    }

    public static IEnumerable<IEnumerable<T>> GetKCombinations<T>(this IEnumerable<T> list, int length)
        where T : IComparable
    {
        if (length == 1) return list.Select(t => new[] { t });
        return GetKCombinations(list, length - 1)
            .SelectMany(t => list.Where(o => o.CompareTo(t.Last()) > 0),
                (t1, t2) => t1.Concat(new[] { t2 }));
    }

    public static IEnumerable<IEnumerable<T>> GetKCombinationsWithRepetition<T>(this IEnumerable<T> list, int length)
        where T : IComparable
    {
        if (length == 1) return list.Select(t => new[] { t });
        return GetKCombinationsWithRepetition(list, length - 1)
            .SelectMany(t => list.Where(o => o.CompareTo(t.Last()) >= 0),
                (t1, t2) => t1.Concat(new[] { t2 }));
    }

    public static IEnumerable<IEnumerable<T>> GetPermutations<T>(this IEnumerable<T> list, int length)
    {
        if (length == 1) return list.Select(t => new[] { t });
        return GetPermutations(list, length - 1)
            .SelectMany(t => list.Where(o => !t.Contains(o)),
                (t1, t2) => t1.Concat(new[] { t2 }));
    }

    public static IEnumerable<IEnumerable<T>> GetPermutationsWithRepetition<T>(this IEnumerable<T> list, int length)
    {
        if (length == 1) return list.Select(t => new[] { t });
        return GetPermutationsWithRepetition(list, length - 1)
            .SelectMany(_ => list,
                (t1, t2) => t1.Concat(new[] { t2 }));
    }

    public static IEnumerable<T[]> GroupEvery<T>(this IEnumerable<T> enumeration, int count)
    {
        if (enumeration == null)
            throw new ArgumentNullException(nameof(enumeration));
        if (count <= 0)
            throw new ArgumentOutOfRangeException(nameof(count));
        var current = 0;
        var array = new T[count];
        foreach (var item in enumeration)
        {
            array[current++] = item;
            if (current == count)
            {
                yield return array;
                current = 0;
                array = new T[count];
            }
        }

        if (current != 0) yield return array;
    }

    public static IEnumerable<T> If<T>(
                                                                                                                                                                                                                                                                                                                                                                                                    this IEnumerable<T> query,
        bool should,
        params Func<IEnumerable<T>, IEnumerable<T>>[] transforms)
    {
        return should
            ? transforms.Aggregate(query,
                (current, transform) => transform.Invoke(current))
            : query;
    }

    public static IEnumerable<T> IgnoreNulls<T>(this IEnumerable<T> target)
    {
        if (ReferenceEquals(target, null))
            yield break;
        foreach (var item in target.Where(item => !ReferenceEquals(item, null)))
            yield return item;
    }

    public static IEnumerable<string> Indent(this IEnumerable<string> texts, bool useTabs = true, int? count = 0)
    {
        return texts.Select(t => t.Indent(useTabs, count));
    }

    public static int Index<T>(this IEnumerable<T> list, Func<T, bool> predicate)
    {
        if (predicate == null) throw new ArgumentNullException(nameof(predicate));
        using var enumerator = list.GetEnumerator();
        for (var i = 0; enumerator.MoveNext(); ++i)
            if (predicate(enumerator.Current))
                return i;
        return -1;
    }

    public static int IndexOf<T>(this IEnumerable<T> items, T item, IEqualityComparer<T> comparer)
    {
        return IndexOf(items, item, comparer.Equals);
    }

    public static int IndexOf<T>(this IEnumerable<T> items, T item, Func<T, T, bool> predicate)
    {
        var index = 0;
        foreach (var instance in items)
        {
            if (predicate(item, instance)) return index;
            ++index;
        }

        return -1;
    }

    public static IEnumerable<int> IndicesOf<T>(this IEnumerable<T> obj, T value)
    {
        return from i in Enumerable.Range(0, obj.Count())
               where obj.ElementAt(i).Equals(value)
               select i;
    }

    public static IEnumerable<int> IndicesOf<T>(this IEnumerable<T> obj, IEnumerable<T> value)
    {
        return from i in Enumerable.Range(0, obj.Count())
               where value.Contains(obj.ElementAt(i))
               select i;
    }

    public static IEnumerable<int> IndicesWhere<T>(this IEnumerable<T> enumeration, Func<T, bool> predicate)
    {
        if (enumeration == null)
            throw new ArgumentNullException(nameof(enumeration));
        if (predicate == null)
            throw new ArgumentNullException(nameof(predicate));
        var i = 0;
        foreach (var item in enumeration)
        {
            if (predicate(item))
                yield return i;
            i++;
        }
    }

    public static IEnumerable<TSource> Intersperse<TSource>(this IEnumerable<TSource> source, TSource separator,
        int count = 1)
    {
        var isFirst = true;

        foreach (var element in source)
        {
            if (!isFirst)
                for (var i = 0; i < count; i++)
                    yield return separator;
            else
                isFirst = false;

            yield return element;
        }
    }

    public static bool IsCountEqual<TSource>(this IEnumerable<TSource> source, int expectedCount)
    {
        return source.Take(expectedCount + 1).Count() == expectedCount;
    }

    public static bool IsCountEqual<TSource>(this IEnumerable<TSource> source, int expectedCount,
        Func<TSource, bool> predicate)
    {
        return source.Where(predicate).Take(expectedCount + 1).Count() == expectedCount;
    }

    public static bool IsCountGreater<TSource>(this IEnumerable<TSource> source, int comparisonCount)
    {
        return source.Skip(comparisonCount).Any();
    }

    public static bool IsCountGreater<TSource>(this IEnumerable<TSource> source, int comparisonCount,
        Func<TSource, bool> predicate)
    {
        return source.Where(predicate).Skip(comparisonCount).Any();
    }

    public static bool IsCountSmaller<TSource>(this IEnumerable<TSource> source, int comparisonCount)
    {
        return !source.Skip(comparisonCount - 1).Any();
    }

    public static bool IsCountSmaller<TSource>(this IEnumerable<TSource> source, int comparisonCount,
        Func<TSource, bool> predicate)
    {
        return !source.Where(predicate).Skip(comparisonCount - 1).Any();
    }

    public static bool IsEmpty(this IEnumerable collection)
    {
        foreach (var _ in collection) return false;
        return true;
    }

    public static bool IsEmpty<T>(this IEnumerable<T> @this)
    {
        return !@this.Any();
    }

    public static bool IsEqual<T>(this IEnumerable<T>? source, IEnumerable<T>? toCompareWith)
    {
        if (source == null || toCompareWith == null) return false;
        return source.IsEqual(toCompareWith, null);
    }

    public static bool IsEqual<T>(this IEnumerable<T>? source, IEnumerable<T>? toCompareWith,
        IEqualityComparer<T>? comparer)
    {
        if (source == null || toCompareWith == null) return false;
        var countSource = source.Count();
        var countToCompareWith = toCompareWith.Count();
        if (countSource != countToCompareWith) return false;
        if (countSource == 0) return true;

        var comparerToUse = comparer ?? EqualityComparer<T>.Default;
        return source.Intersect(toCompareWith, comparerToUse).Count() == countSource;
    }

    public static IEnumerable<string> IsLike(this IEnumerable<string> @this, string pattern)
    {
        return @this.Where(x => x.IsLike(pattern));
    }

    public static bool IsNotEmpty<T>(this IEnumerable<T> @this)
    {
        return @this.Any();
    }

    public static bool IsNotNullOrEmpty<T>(this IEnumerable<T>? @this)
    {
        return @this != null && @this.Any();
    }

    public static bool IsNullOrEmpty<T>(this IEnumerable<T>? @this)
    {
        return @this == null || !@this.Any();
    }

    public static bool IsOneItem<T>(this IEnumerable<T> source)
    {
        return source.Count() == 1;
    }

    public static bool IsOneItem<T>(this IEnumerable<T> source, Func<T, bool> query)
    {
        return source.Count(query) == 1;
    }

    public static bool IsSingle<T>(this IEnumerable<T> source)
    {
        if (source == null)
            throw new ArgumentNullException(nameof(source));
        using var enumerator = source.GetEnumerator();
        return enumerator.MoveNext() && !enumerator.MoveNext();
    }

    public static string Join(this IEnumerable<string> values, string separator)
    {
        return string.Join(separator, values.ToArray());
    }

    public static string Join<T>(this IEnumerable<T> collection, Func<T, string> func, string separator)
    {
        return string.Join(separator, collection.Select(func).ToArray());
    }

    public static int LastIndexOf<T>(this IEnumerable<T> source, Func<T, bool> predicate)
    {
        var asReadOnlyList = source as IReadOnlyList<T> ?? source.ToArray();

        for (var i = asReadOnlyList.Count - 1; i >= 0; i--)
            if (predicate(asReadOnlyList[i]))
                return i;

        return -1;
    }

    public static int LastIndexOf<T>(this IEnumerable<T> source, T element)
    {
        return source.LastIndexOf(element, EqualityComparer<T>.Default);
    }

    public static int LastIndexOf<T>(this IEnumerable<T> source, T element, IEqualityComparer<T> comparer)
    {
        return source.LastIndexOf(i => comparer.Equals(i, element));
    }

    public static IEnumerable<TResult> LeftOuterJoin<TLeft, TRight, TKey, TResult>(
        this IEnumerable<TLeft> left,
        IEnumerable<TRight> right,
        Func<TLeft, TKey> leftKeySelector,
        Func<TRight, TKey> rightKeySelector,
        Func<JoinResult<TLeft, TRight>, TResult> resultSelector)
    {
        var groupJoinResult = left
            .GroupJoin(
                right,
                leftKeySelector,
                rightKeySelector,
                (l, r) => new GroupJoinResult<TLeft, TRight>
                {
                    Key = l,
                    Values = r
                });

        return groupJoinResult
            .SelectMany(
                x => x.Values?.Select(c => new JoinResult<TLeft, TRight>
                {
                    Left = x.Key,
                    Right = c
                }) ?? Array.Empty<JoinResult<TLeft, TRight>>())
            .Select(resultSelector);
    }

    public static string LineByLine(this IEnumerable<string> lines, string separator = "")
    {
        return string.Join(separator + Environment.NewLine, lines);
    }

    public static long LongCount(this IEnumerable enumerable, bool excludeNullValues = false)
    {
        var list = enumerable.Cast<object?>();
        if (excludeNullValues) list = list.Where(x => x != null);
        return Enumerable.LongCount(list);
    }

    public static bool Many<T>(this IEnumerable<T> source)
    {
        return source.Count() > 1;
    }

    public static bool Many<T>(this IEnumerable<T> source, Func<T, bool> query)
    {
        return source.Count(query) > 1;
    }

    public static TSource? Max<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, TResult> selector)
    {
        return source.FirstOrDefault(c => selector(c).Equals(source.Max(selector)));
    }

    public static TItem? MaxItem<TItem, TValue>(this IEnumerable<TItem?> items, Func<TItem, TValue> selector,
        out TValue? maxValue)
        where TItem : class
        where TValue : IComparable
    {
        TItem? maxItem = null;
        maxValue = default;
        foreach (var item in items)
        {
            if (item != null)
            {
                var itemValue = selector(item);
                if (maxItem != null && itemValue.CompareTo(maxValue) <= 0)
                    continue;
                maxValue = itemValue;
            }

            maxItem = item;
        }

        return maxItem;
    }

    public static TItem? MaxItem<TItem, TValue>(this IEnumerable<TItem?> items, Func<TItem, TValue> selector)
        where TItem : class
        where TValue : IComparable
    {
        return items.MaxItem(selector, out _);
    }

    public static IEnumerable<TSource>? Maxs<TSource, TResult>(this IEnumerable<TSource> source,
        Func<TSource, TResult> selector)
    {
        return source.ToLookup(selector).Max(n => n.Key);
    }

    public static IEnumerable<T> MergeDistinctInnerEnumerable<T>(this IEnumerable<IEnumerable<T>> @this)
    {
        var listItem = @this.ToList();

        var list = new List<T>();

        foreach (var item in listItem) list = list.Union(item).ToList();

        return list;
    }

    public static IEnumerable<T> MergeInnerEnumerable<T>(this IEnumerable<IEnumerable<T>> @this)
    {
        var listItem = @this.ToList();

        var list = new List<T>();

        foreach (var item in listItem) list.AddRange(item);

        return list;
    }

    public static TSource? Min<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, TResult> selector)
    {
        return source.FirstOrDefault(c => selector(c).Equals(source.Min(selector)));
    }

    public static TItem? MinItem<TItem, TValue>(this IEnumerable<TItem?> items, Func<TItem, TValue> selector,
        out TValue? minValue)
        where TItem : class
        where TValue : IComparable
    {
        TItem? minItem = null;
        minValue = default;
        foreach (var item in items)
        {
            if (item == null)
                continue;
            var itemValue = selector(item);
            if (minItem != null && itemValue.CompareTo(minValue) >= 0)
                continue;
            minValue = itemValue;
            minItem = item;
        }

        return minItem;
    }

    public static TItem? MinItem<TItem, TValue>(this IEnumerable<TItem?> items, Func<TItem, TValue> selector)
        where TItem : class
        where TValue : IComparable
    {
        return items.MinItem(selector, out _);
    }

    public static IEnumerable<TSource>? Mins<TSource, TResult>(this IEnumerable<TSource> source,
        Func<TSource, TResult> selector)
    {
        return source.ToLookup(selector).Min(n => n.Key);
    }

    public static bool None<T>(this IEnumerable<T> source)
    {
        return !source.Any();
    }

    public static bool None<T>(this IEnumerable<T> source, Func<T, bool> query)
    {
        return !source.Any(query);
    }

    public static IEnumerable NotOfType<TSource>(this IEnumerable source)
    {
        return source.Cast<object>().Where(x => !(x is TSource));
    }

    public static bool OnlyOne<T>(this IEnumerable<T> source)
    {
        return source.Count() == 1;
    }

    public static bool OnlyOne<T>(this IEnumerable<T> source, Func<T, bool> condition)
    {
        return source.Count(condition) == 1;
    }

    public static PartitionedSequence<TSource> Partition<TSource>(this IEnumerable<TSource> source,
        Func<TSource, bool> predicate)
    {
        var matches = new List<TSource>();
        var mismatches = new List<TSource>();

        foreach (var value in source)
            if (predicate(value))
                matches.Add(value);
            else
                mismatches.Add(value);

        return new PartitionedSequence<TSource>(matches, mismatches);
    }

    public static string PathCombine(this IEnumerable<string> enumerable)
    {
        if (enumerable is null) throw new ArgumentNullException(nameof(enumerable));

        return Path.Combine(enumerable.ToArray());
    }

    public static IEnumerable<T> Perform<T>(this IEnumerable<T> collection, Func<T, T> func)
    {
        if (collection == null) throw new ArgumentNullException(nameof(collection));
        if (func == null) throw new ArgumentNullException(nameof(func));
        foreach (var item in collection) yield return func(item);
    }

    public static Dictionary<TFirstKey, Dictionary<TSecondKey, TValue>> Pivot<TSource, TFirstKey, TSecondKey, TValue>(
        this IEnumerable<TSource> source, Func<TSource, TFirstKey> firstKeySelector,
        Func<TSource, TSecondKey> secondKeySelector, Func<IEnumerable<TSource>, TValue> aggregate)
        where TSecondKey : notnull where TFirstKey : notnull
    {
        var retVal = new Dictionary<TFirstKey, Dictionary<TSecondKey, TValue>>();

        var l = source.ToLookup(firstKeySelector);
        foreach (var item in l)
        {
            var dict = new Dictionary<TSecondKey, TValue>();
            retVal.Add(item.Key, dict);
            var subdict = item.ToLookup(secondKeySelector);
            foreach (var subitem in subdict) dict.Add(subitem.Key, aggregate(subitem));
        }

        return retVal;
    }

    public static IEnumerable<T> Prepend<T>(this IEnumerable<T> source, T element)
    {
        yield return element;
        foreach (var item in source) yield return item;
    }

    public static T Random<T>(this IEnumerable<T> source)
    {
        var asReadOnlyList = source as IReadOnlyList<T> ?? source.ToArray();

        if (!asReadOnlyList.Any())
            throw new InvalidOperationException("Sequence contains no elements.");

        return asReadOnlyList[Rnd.Next(0, asReadOnlyList.Count)];
    }

    public static T RandomOrDefault<T>(this IEnumerable<T> source)
    {
        var asReadOnlyList = source as IReadOnlyList<T> ?? source.ToArray();

        if (!asReadOnlyList.Any())
            return default!;

        return asReadOnlyList[Rnd.Next(0, asReadOnlyList.Count)];
    }

    public static IEnumerable<T> RandomSubset<T>(this IEnumerable<T> sequence, int subsetSize)
    {
        return RandomSubset(sequence, subsetSize, new Random());
    }

    public static IEnumerable<T> RandomSubset<T>(this IEnumerable<T> sequence, int subsetSize, Random rand)
    {
        if (rand == null) throw new ArgumentNullException(nameof(rand));
        if (sequence == null) throw new ArgumentNullException(nameof(sequence));
        if (subsetSize < 0) throw new ArgumentOutOfRangeException(nameof(subsetSize));
        return RandomSubsetImpl(sequence, subsetSize, rand);
    }

    public static IEnumerable<T> RemoveDuplicates<T>(this IEnumerable<T> @this)
    {
        return new HashSet<T>(@this).ToList();
    }

    public static IEnumerable<T> RemoveDuplicates<T>(this IEnumerable<T> list, Func<T, int> predicate)
    {
        var dict = new Dictionary<int, T>();

        foreach (var item in list)
            if (!dict.ContainsKey(predicate(item)))
                dict.Add(predicate(item), item);

        return dict.Values.AsEnumerable();
    }

    public static IEnumerable<string> RemoveEmptyElements(this IEnumerable<string> strings)
    {
        foreach (var s in strings)
            if (!string.IsNullOrEmpty(s))
                yield return s;
    }

    public static IEnumerable<T> RemoveWhere<T>(this IEnumerable<T>? source, Predicate<T> predicate)
    {
        if (source == null)
            yield break;
        foreach (var t in source)
            if (!predicate(t))
                yield return t;
    }

    public static IEnumerable<TSource> Repeat<TSource>(this IEnumerable<TSource> source, int count)
    {
        if (count == 0) yield break;

        var collection = source as ICollection<TSource>;

        var elementBuffer = collection == null
            ? new List<TSource>()
            : new List<TSource>(collection.Count);

        foreach (var element in source)
        {
            yield return element;

            elementBuffer.Add(element);
        }

        if (elementBuffer.IsEmpty())
            yield break;

        for (var i = 0; i < count - 1; i++)
            foreach (var element in elementBuffer)
                yield return element;
    }

    public static IEnumerable<T> ReplaceWhere<T>(this IEnumerable<T>? enumerable, Predicate<T> predicate,
        Func<T> replacement)
    {
        if (enumerable == null) yield break;
        foreach (var item in enumerable)
            if (predicate(item))
                yield return replacement();
            else
                yield return item;
    }

    public static IEnumerable<T> ReplaceWhere<T>(this IEnumerable<T>? enumerable, Predicate<T> predicate, T replacement)
    {
        if (enumerable == null) yield break;
        foreach (var item in enumerable)
            if (predicate(item))
                yield return replacement;
            else
                yield return item;
    }

    public static IEnumerable<TResult> Select<TSource, TResult>(this IEnumerable<TSource> source,
        Func<TSource, TResult> selector, bool allowNull = true)
    {
        foreach (var item in source)
        {
            var select = selector(item);
            if (allowNull || !Equals(select, default(TSource)))
                yield return select;
        }
    }

    public static IEnumerable<T> SelectMany<T>(this IEnumerable<IEnumerable<T>> source)
    {
        if (source == null)
            throw new ArgumentNullException(nameof(source));
        foreach (var enumeration in source)
            foreach (var item in enumeration)
                yield return item;
    }

    public static IEnumerable<T> SelectMany<T>(this IEnumerable<T[]> source)
    {
        if (source == null)
            throw new ArgumentNullException(nameof(source));
        foreach (var enumeration in source)
            foreach (var item in enumeration)
                yield return item;
    }

    public static IEnumerable<T> SelectManyRecursive<T>(this IEnumerable<T> source, Func<T, IEnumerable<T>?> selector)
    {
        if (source == null) throw new ArgumentNullException(nameof(source));
        if (selector == null) throw new ArgumentNullException(nameof(selector));

        return !source.Any()
            ? source
            : source.Concat(
                source
                    .SelectMany(i => selector(i).EmptyIfNull())
                    .SelectManyRecursive(selector)
            );
    }

    public static IEnumerable<TResult> SelectRecursive<TSource, TResult>(this IEnumerable<TSource> source,
        Func<TSource, IEnumerable<TSource>?>? getChildren, Func<TSource, int, TResult> selector)
    {
        if (source == null) throw new ArgumentNullException(nameof(source));
        if (selector == null) throw new ArgumentNullException(nameof(selector));
        if (getChildren == null) return source.Select(s => selector(s, 0));
        return SelectRecursiveIterator(source, getChildren, selector);
    }

    public static IEnumerable<T> SelectRecursive<T>(this IEnumerable<T> source, Func<T, IEnumerable<T>>? getChildren)
    {
        if (source == null) throw new ArgumentNullException(nameof(source));
        if (getChildren == null) return source;
        return SelectRecursiveIterator(source, getChildren, (s, _) => s);
    }

    public static bool SequenceEqual<T1, T2>(this IEnumerable<T1> left, IEnumerable<T2> right,
        Func<T1, T2, bool> comparer)
    {
        using var leftE = left.GetEnumerator();
        using var rightE = right.GetEnumerator();
        bool leftNext = leftE.MoveNext(), rightNext = rightE.MoveNext();

        while (leftNext && rightNext)
        {
            if (!comparer(leftE.Current, rightE.Current))
                return false;

            leftNext = leftE.MoveNext();
            rightNext = rightE.MoveNext();
        }

        if (leftNext || rightNext)
            return false;

        return true;
    }

    public static bool SequenceSuperset<T>(this IEnumerable<T> enumeration, IEnumerable<T> subset)
    {
        return SequenceSuperset(enumeration, subset, EqualityComparer<T>.Default.Equals);
    }

    public static bool SequenceSuperset<T>(this IEnumerable<T> enumeration, IEnumerable<T> subset,
        Func<T, T, bool> equalityComparer)
    {
        if (enumeration == null)
            throw new ArgumentNullException(nameof(enumeration));
        if (subset == null)
            throw new ArgumentNullException(nameof(subset));
        if (equalityComparer == null)
            throw new ArgumentNullException(nameof(equalityComparer));
        using IEnumerator<T> big = enumeration.GetEnumerator(), small = subset.GetEnumerator();
        big.Reset();
        small.Reset();
        while (big.MoveNext())
        {
            if (!small.MoveNext())
                return true;
            if (!equalityComparer(big.Current, small.Current))
            {
                small.Reset();
                small.MoveNext();
                if (!equalityComparer(big.Current, small.Current))
                    small.Reset();
            }
        }

        if (!small.MoveNext())
            return true;

        return false;
    }

    public static bool SetEqual<T>(this IEnumerable<T>? source, IEnumerable<T>? toCompareWith)
    {
        if (source == null || toCompareWith == null) return false;
        return source.SetEqual(toCompareWith, null);
    }

    public static bool SetEqual<T>(this IEnumerable<T>? source, IEnumerable<T>? toCompareWith,
        IEqualityComparer<T>? comparer)
    {
        if (source == null || toCompareWith == null) return false;
        var countSource = source.Count();
        var countToCompareWith = toCompareWith.Count();
        if (countSource != countToCompareWith) return false;
        if (countSource == 0) return true;
        var comparerToUse = comparer ?? EqualityComparer<T>.Default;
        return source.Intersect(toCompareWith, comparerToUse).Count() == countSource;
    }

    public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> items)
    {
        if (items == null) throw new ArgumentNullException(nameof(items));
        return items.ShuffleIterator();
    }

    public static TSource Single<TSource, TException>(this IEnumerable<TSource> source, Func<TSource, bool> predicate,
        Func<TException> exceptionToThrow)
        where TException : Exception
    {
        switch (source.Count(predicate))
        {
            case 1:
                return source.Single(predicate);

            default:
                throw exceptionToThrow();
        }
    }

    public static TSource SingleOfType<TSource>(this IEnumerable source)
    {
        return source.OfType<TSource>().Single();
    }

    public static TSource? SingleOrDefaultOfType<TSource>(this IEnumerable source)
    {
        return source.OfType<TSource>().SingleOrDefault();
    }

    public static TSource SingleOrThrow<TSource, TException>(this IEnumerable<TSource> source,
        Func<TException> exceptionToThrow)
        where TException : Exception
    {
        switch (source.Take(2).Count())
        {
            case 1:
                return source.First();

            default:
                throw exceptionToThrow();
        }
    }

    public static IEnumerable<T> SkipLast<T>(this IEnumerable<T> source, int count)
    {
        if (count == 0)
            return source;

        var asReadOnlyList = source as IReadOnlyList<T> ?? source.ToArray();

        if (count >= asReadOnlyList.Count)
            return Enumerable.Empty<T>();

        return asReadOnlyList.Slice(0, asReadOnlyList.Count - count);
    }

    public static IEnumerable<T> SkipLastWhile<T>(this IEnumerable<T> source, Func<T, bool> predicate)
    {
        return source.Reverse().SkipWhile(predicate).Reverse();
    }

    public static IEnumerable<T> Slice<T>(this IEnumerable<T> items, int start, int end)
    {
        var index = 0;
        int count;
        if (items == null)
            throw new ArgumentNullException(nameof(items));
        if (items is ICollection<T>)
            count = ((ICollection<T>)items).Count;
        else if (items is ICollection)
            count = ((ICollection)items).Count;
        else
            count = items.Count();
        if (start < 0)
            start += count;
        if (end < 0)
            end += count;
        foreach (var item in items)
        {
            if (index >= end)
                yield break;
            if (index >= start)
                yield return item;
            ++index;
        }
    }

    public static IEnumerable<IEnumerable<T>> Split<T>(this IEnumerable<T> source, int chunkSize)
    {
        var count = 0;
        var chunk = new List<T>(chunkSize);
        foreach (var item in source)
        {
            chunk.Add(item);
            count++;
            if (count == chunkSize)
            {
                yield return chunk.AsEnumerable();
                chunk = new List<T>(chunkSize);
                count = 0;
            }
        }

        if (count > 0) yield return chunk.AsEnumerable();
    }

    public static double StdDev(this IEnumerable<int> source)
    {
        return StdDevLogic(source);
    }

    public static double StdDev(this IEnumerable<double> source)
    {
        return StdDevLogic(source);
    }

    public static float StdDev(this IEnumerable<float> source)
    {
        return StdDevLogic(source);
    }

    public static double StdDevP(this IEnumerable<int> source)
    {
        return StdDevLogic(source, 0);
    }

    public static double StdDevP(this IEnumerable<double> source)
    {
        return StdDevLogic(source, 0);
    }

    public static double StdDevP(this IEnumerable<float> source)
    {
        return StdDevLogic(source, 0);
    }

    public static string StringJoin<T>(this IEnumerable<T> @this, string separator)
    {
        return string.Join(separator, @this);
    }

    public static string StringJoin<T>(this IEnumerable<T> @this, char separator)
    {
        return string.Join(separator.ToString(), @this);
    }

    public static uint Sum(this IEnumerable<uint> source)
    {
        return source.Aggregate(0U, (current, number) => current + number);
    }

    public static ulong Sum(this IEnumerable<ulong> source)
    {
        return source.Aggregate(0UL, (current, number) => current + number);
    }

    public static uint? Sum(this IEnumerable<uint?> source)
    {
        return source.Where(nullable => nullable.HasValue)
            .Aggregate(0U, (current, nullable) => current + nullable.GetValueOrDefault());
    }

    public static ulong? Sum(this IEnumerable<ulong?> source)
    {
        return source.Where(nullable => nullable.HasValue)
            .Aggregate(0UL, (current, nullable) => current + nullable.GetValueOrDefault());
    }

    public static uint Sum<T>(this IEnumerable<T> source, Func<T, uint> selection)
    {
        return ElementsNotNullFrom(source).Select(selection).Sum();
    }

    public static uint? Sum<T>(this IEnumerable<T> source, Func<T, uint?> selection)
    {
        return ElementsNotNullFrom(source).Select(selection).Sum();
    }

    public static ulong Sum<T>(this IEnumerable<T> source, Func<T, ulong> selector)
    {
        return ElementsNotNullFrom(source).Select(selector).Sum();
    }

    public static ulong? Sum<T>(this IEnumerable<T> source, Func<T, ulong?> selector)
    {
        return ElementsNotNullFrom(source).Select(selector).Sum();
    }

    public static IEnumerable<T> TakeEvery<T>(this IEnumerable<T> enumeration, int startAt, int hopLength)
    {
        if (enumeration == null)
            throw new ArgumentNullException(nameof(enumeration));
        var first = 0;
        var count = 0;
        foreach (var item in enumeration)
            if (first < startAt)
            {
                first++;
            }
            else if (first == startAt)
            {
                yield return item;
                first++;
            }
            else
            {
                count++;
                if (count == hopLength)
                {
                    yield return item;
                    count = 0;
                }
            }
    }

    public static IEnumerable<T> TakeLastWhile<T>(this IEnumerable<T> source, Func<T, bool> predicate)
    {
        return source.Reverse().TakeWhile(predicate).Reverse();
    }

    public static IEnumerable<TSource> TakeSkip<TSource>(this IEnumerable<TSource> source, int take, int skip)
    {
        using var enumerator = source.GetEnumerator();

        while (true)
        {
            for (var i = 0; i < take; i++)
            {
                if (!enumerator.MoveNext())
                    yield break;

                yield return enumerator.Current;
            }

            for (var i = 0; i < skip; i++)
                if (!enumerator.MoveNext())
                    yield break;
        }
    }

    public static IEnumerable<T> TakeUntil<T>(this IEnumerable<T> collection, Func<T, bool> endCondition)
    {
        return collection.TakeWhile(item => !endCondition(item));
    }

    public static IEnumerable<T> TakeUntil<T>(this IEnumerable<T> collection, Predicate<T> endCondition)
    {
        return collection.TakeWhile(item => !endCondition(item));
    }

    public static TResult[] ToArray<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, TResult> selector)
    {
        return source.Select(selector).ToArray();
    }

    public static async IAsyncEnumerable<T> ToAsync<T>(this IEnumerable<T> enumerable)
    {
        foreach (var item in enumerable)
            yield return await Task.FromResult(item);
    }

    public static ICollection<T> ToCollection<T>(this IEnumerable<T> enumerable)
    {
        var collection = new Collection<T>();
        foreach (var i in enumerable)
            collection.Add(i);
        return collection;
    }

    public static string? ToCsv<T>(this IEnumerable<T> items, char separator = ',', bool trim = true)
    {
        if (items == null) throw new ArgumentNullException(nameof(items));

        var list = trim ? items.Select(x => x?.ToString()?.Trim()) : items.Select(x => x?.ToString());
        return list.Aggregate((a, b) => a + separator + b);
    }

    public static DataTable ToDataTable<T>(this IEnumerable<T>? varlist)
    {
        var dtReturn = new DataTable();
        PropertyInfo[]? oProps = null;
        if (varlist == null) return dtReturn;
        foreach (var rec in varlist)
        {
            if (oProps == null)
            {
                oProps = rec?.GetType().GetProperties();
                if (oProps != null)
                    foreach (var pi in oProps)
                    {
                        var colType = pi.PropertyType;
                        if (colType.IsGenericType && colType.GetGenericTypeDefinition() == typeof(Nullable<>))
                            colType = colType.GetGenericArguments()[0];
                        dtReturn.Columns.Add(new DataColumn(pi.Name, colType));
                    }
            }

            var dr = dtReturn.NewRow();
            if (oProps != null)
                foreach (var pi in oProps)
                    dr[pi.Name] = pi.GetValue(rec, null) ?? DBNull.Value;
            dtReturn.Rows.Add(dr);
        }

        return dtReturn;
    }

    public static Dictionary<TKey, List<TValue>> ToDictionary<TKey, TValue>(
        this IEnumerable<IGrouping<TKey, TValue>> groupings) where TKey : notnull
    {
        if (groupings == null)
            throw new ArgumentNullException(nameof(groupings));

        return groupings.ToDictionary(group => group.Key, group => group.ToList());
    }

    public static Dictionary<TKey, TValue> ToDictionary<TKey, TValue>(
        this IEnumerable<KeyValuePair<TKey, TValue>> enumeration) where TKey : notnull
    {
        if (enumeration == null)
            throw new ArgumentNullException(nameof(enumeration));

        return enumeration.ToDictionary(item => item.Key, item => item.Value);
    }

    public static IDictionary<TKey, TElement> ToDistinctDictionary<TSource, TKey, TElement>(
        this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector)
        where TKey : notnull
    {
        if (source == null) throw new NullReferenceException("The 'source' cannot be null.");
        if (keySelector == null) throw new ArgumentNullException(nameof(keySelector));
        if (elementSelector == null) throw new ArgumentNullException(nameof(elementSelector));

        var dictionary = new Dictionary<TKey, TElement>();
        foreach (var current in source) dictionary[keySelector(current)] = elementSelector(current);
        return dictionary;
    }

    public static Dictionary<TKey, IEnumerable<TElement>>
        ToGroupedDictionary<TKey, TElement>(this IEnumerable<IGrouping<TKey, TElement>> items) where TKey : notnull
    {
        return items.ToDictionary<IGrouping<TKey, TElement>, TKey, IEnumerable<TElement>>(
            item => item.Key,
            item => item);
    }

    public static List<TResult> ToList<TSource, TResult>(this IEnumerable<TSource> source,
        Func<TSource, TResult> selector)
    {
        return source.Select(selector).ToList();
    }

    public static ObservableCollection<T> ToObservableCollection<T>(this IEnumerable<T> enumerable)
    {
        if (enumerable is null) throw new ArgumentNullException(nameof(enumerable));

        return new ObservableCollection<T>(enumerable);
    }

    public static IEnumerable<T> ToPaged<T>(this IEnumerable<T> query, int pageIndex, int pageSize)
    {
        return query
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
            ;
    }

    public static IPagedList<T> ToPagedList<T>(this IEnumerable<T> source, int pageNumber, int pageSize)
    {
        return new PagedList<T>(source.AsQueryable(), pageNumber, pageSize);
    }

    public static IReadOnlyCollection<TDestination> ToReadOnlyCollection<TDestination>(
        this IEnumerable<TDestination>? source)
    {
        var sourceAsDestination = new List<TDestination>();
        if (source != null)
            foreach (var toAdd in source)
                sourceAsDestination.Add(toAdd);
        return new ReadOnlyCollection<TDestination>(sourceAsDestination);
    }

    public static StringBuilder ToStringBuilder(this IEnumerable<string> strs)
    {
        var sb = new StringBuilder();
        foreach (var str in strs) sb.AppendLine(str);
        return sb;
    }

    public static string ToText(this IEnumerable<string> strs)
    {
        return strs.ToStringBuilder().ToString();
    }

    public static IEnumerable<T> Traverse<T>(this IEnumerable<T> source, Func<T, IEnumerable<T>> selector)
    {
        foreach (var item in source)
        {
            yield return item;
            var children = selector(item);
            foreach (var child in children.Traverse(selector)) yield return child;
        }
    }

    public static bool TryGetNonEnumeratedCount<T>(this IEnumerable<T> enumerable, out int count)
    {
        count = 0;
        if (enumerable is ICollection<T> collection)
        {
            count = collection.Count;
            return true;
        }

        if (enumerable is IReadOnlyCollection<T> roCollection)
        {
            count = roCollection.Count;
            return true;
        }

        return false;
    }

    public static IEnumerable<T>? Union<T>(this IEnumerable<IEnumerable<T>?> enumeration)
    {
        if (enumeration == null)
            throw new ArgumentNullException(nameof(enumeration));

        IEnumerable<T>? returnValue = null;

        foreach (var e in enumeration)
            if (returnValue != null)
                returnValue = e;
            else if (returnValue != null)
                if (e != null)
                    returnValue = returnValue.Union(e);

        return returnValue;
    }

    public static IEnumerable<T> WhereIf<T>(this IEnumerable<T> source, bool condition, Func<T, bool> predicate)
    {
        return condition ? source.Where(predicate) : source;
    }

    public static IEnumerable<T> WhereIf<T>(this IEnumerable<T> source, bool condition, Func<T, int, bool> predicate)
    {
        return condition ? source.Where(predicate) : source;
    }

    public static IEnumerable<TSource> WhereNot<TSource>(this IEnumerable<TSource> source,
        Func<TSource, bool> predicate)
    {
        return source.Where(element => !predicate(element));
    }

    public static IEnumerable<TSource> WhereNot<TSource>(this IEnumerable<TSource> source,
        Func<TSource, int, bool> predicate)
    {
        return source.Where((element, index) => !predicate(element, index));
    }

    public static IEnumerable<T> WhereNotNull<T>(this IEnumerable<T?> source) where T : class
    {
        if (source == null) throw new ArgumentNullException(nameof(source));

        return (IEnumerable<T>)source.Where(x => x != null);
    }

    public static TList With<TList, T>(this TList list, T item) where TList : IList<T>, new()
    {
        var l = new TList();

        foreach (var i in list) l.Add(i);
        l.Add(item);

        return l;
    }

    public static IEnumerable<(T item, int index)> WithIndex<T>(this IEnumerable<T> source)
    {
        return source.Select((item, index) => (item, index));
    }

    public static IEnumerable<TSource> Without<TSource>(this IEnumerable<TSource> source, params TSource[] elements)
    {
        return Without(source, (IEnumerable<TSource>)elements);
    }

    public static IEnumerable<TSource> Without<TSource>(this IEnumerable<TSource> source, IEnumerable<TSource> elements)
    {
        return WithoutIterator(source, elements, EqualityComparer<TSource>.Default);
    }

    public static IEnumerable<TSource> Without<TSource>(this IEnumerable<TSource> source,
        IEqualityComparer<TSource> equalityComparer, params TSource[] elements)
    {
        return Without(source, equalityComparer, (IEnumerable<TSource>)elements);
    }

    public static IEnumerable<TSource> Without<TSource>(this IEnumerable<TSource> source,
        IEqualityComparer<TSource> equalityComparer, IEnumerable<TSource> elements)
    {
        return WithoutIterator(source, elements, equalityComparer);
    }

    public static TList Without<TList, T>(this TList list, T item) where TList : IList<T>, new()
    {
        var l = new TList();

        foreach (var i in list.Where(n => n != null && !n.Equals(item)))
            l.Add(i);

        return l;
    }

    public static IEnumerable<(T1 left, T2 right)> Zip<T1, T2>(this (IEnumerable<T1>, IEnumerable<T2>) seqs)
    {
        using var iterLeft = seqs.Item1.GetEnumerator();
        using var iterRight = seqs.Item2.GetEnumerator();
        bool leftAdv, rightAdv;
        while ((leftAdv = iterLeft.MoveNext()) & (rightAdv = iterRight.MoveNext()))
            yield return (iterLeft.Current, iterRight.Current);

        if (leftAdv != rightAdv)
            throw new InvalidOperationException("Collections should have the same size");
    }

    private static IEnumerable<T> ElementsNotNullFrom<T>(IEnumerable<T> source)
    {
        return source.Where(x => x != null);
    }

    private static IEnumerable<T> RandomSubsetImpl<T>(IEnumerable<T> sequence, int subsetSize, Random rand)
    {
        var seqArray = sequence.ToArray();
        if (seqArray.Length < subsetSize)
            throw new ArgumentOutOfRangeException(nameof(subsetSize), "Subset size must be <= sequence.Count()");
        var m = 0;
        var w = seqArray.Length;
        var g = w - 1;
        while (m < subsetSize)
        {
            var k = g - rand.Next(w);
            (seqArray[k], seqArray[m]) = (seqArray[m], seqArray[k]);
            ++m;
            --w;
        }

        for (var i = 0; i < subsetSize; i++)
            yield return seqArray[i];
    }

    private static IEnumerable<TResult> SelectRecursiveIterator<TSource, TResult>(IEnumerable<TSource> source,
        Func<TSource, IEnumerable<TSource>?> getChildren, Func<TSource, int, TResult> selector)
    {
        var stack = new Stack<IEnumerator<TSource>>();

        try
        {
            stack.Push(source.GetEnumerator());
            while (0 != stack.Count)
            {
                var iter = stack.Peek();
                if (iter.MoveNext())
                {
                    var current = iter.Current;
                    yield return selector(current, stack.Count - 1);

                    var children = getChildren(current);
                    if (children != null) stack.Push(children.GetEnumerator());
                }
                else
                {
                    iter.Dispose();
                    stack.Pop();
                }
            }
        }
        finally
        {
            while (0 != stack.Count)
            {
                stack.Pop().Dispose();
            }
        }
    }

    private static IEnumerable<T> ShuffleIterator<T>(this IEnumerable<T> items)
    {
        var buffer = items.ToList();
        for (var i = 0; i < buffer.Count; i++)
        {
            var j = Rnd.Next(i, buffer.Count);
            yield return buffer[j];
            buffer[j] = buffer[i];
        }
    }

    private static double StdDevLogic(this IEnumerable<double> source, int buffer = 1)
    {
        if (source == null) throw new ArgumentNullException(nameof(source));

        var data = source.ToList();
        var average = data.Average();
        var differences = data.Select(u => Math.Pow(average - u, 2.0)).ToList();
        return Math.Sqrt(differences.Sum() / (differences.Count - buffer));
    }

    private static double StdDevLogic(this IEnumerable<int> source, int buffer = 1)
    {
        return StdDevLogic(source.Select(x => (double)x), buffer);
    }

    private static float StdDevLogic(this IEnumerable<float> source, int buffer = 1)
    {
        if (source == null) throw new ArgumentNullException(nameof(source));
        var data = source.ToList();
        var average = data.Average();
        var differences = data.Select(u => Math.Pow(average - u, 2.0)).ToList();
        return (float)Math.Sqrt(differences.Sum() / (differences.Count - buffer));
    }

    private static IEnumerable<TSource> WithoutIterator<TSource>(IEnumerable<TSource> source,
        IEnumerable<TSource> elementsToRemove, IEqualityComparer<TSource> comparer)
    {
        var elementsToRemoveSet = new HashSet<TSource>(elementsToRemove, comparer);
        return source.Where(elem => !elementsToRemoveSet.Contains(elem));
    }
}