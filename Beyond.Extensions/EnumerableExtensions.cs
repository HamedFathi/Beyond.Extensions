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

    public static IEnumerable<T> Acquire<T>(this IEnumerable<T> source, Action acquire, Action release)
    {
        acquire();
        foreach (var item in source)
        {
            yield return item;
        }
        release();
    }

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

    public static TSource AggregateRight<TSource>(this IEnumerable<TSource> source, Func<TSource, TSource, TSource> func)
    {
        var reversed = source.Reverse().ToList();
        return reversed.Aggregate((a, b) => func(b, a));
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

    public static IEnumerable<T> Assert<T>(this IEnumerable<T> source, Func<T, bool> predicate, string message)
    {
        foreach (var item in source)
        {
            Debug.Assert(predicate(item), message);
            yield return item;
        }
    }

    public static IEnumerable<T> AssertCount<T>(this IEnumerable<T> source, int count, string message)
    {
        Debug.Assert(source.Count() == count, message);
        return source;
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

    public static async Task<IEnumerable<TResult>> Await<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, Task<TResult>> selector)
    {
        var tasks = source.Select(selector).ToList();
        var results = await Task.WhenAll(tasks);
        return results;
    }

    public static async Task AwaitCompletion(this IEnumerable<Task> tasks)
    {
        await Task.WhenAll(tasks);
    }

    public static IEnumerable<T> Backsert<T>(this IEnumerable<T> source, T item)
    {
        foreach (var element in source)
        {
            yield return element;
        }
        yield return item;
    }

    public static IEnumerable<IEnumerable<T>> Batch<T>(this IEnumerable<T> source, int batchSize)
    {
        using (var enumerator = source.GetEnumerator())
        {
            while (enumerator.MoveNext())
            {
                yield return YieldBatchElements(enumerator, batchSize - 1);
            }
        }
    }

    public static IEnumerable<IEnumerable<TResult>> BatchSelect<TFirst, TSecond, TResult>(
         this IEnumerable<TFirst> first,
         IEnumerable<TSecond> second,
         Func<TFirst, TSecond, TResult> resultSelector,
         int batchSize)
    {
        var batch = new List<TResult>(batchSize);
        using (var e1 = first.GetEnumerator())
        using (var e2 = second.GetEnumerator())
        {
            while (e1.MoveNext() && e2.MoveNext())
            {
                batch.Add(resultSelector(e1.Current, e2.Current));
                if (batch.Count == batchSize)
                {
                    yield return batch;
                    batch = new List<TResult>(batchSize);
                }
            }
        }
        if (batch.Count > 0)
        {
            yield return batch;
        }
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

    public static IEnumerable<TResult> Choose<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, TResult?> selector) where TResult : struct
    {
        foreach (var element in source)
        {
            var result = selector(element);
            if (result.HasValue)
            {
                yield return result.Value;
            }
        }
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

    public static int CompareCount<T>(this IEnumerable<T> first, IEnumerable<T> second)
    {
        return first.Count().CompareTo(second.Count());
    }

    public static IEnumerable<T> Concat<T>(IEnumerable<T> target, T element)
    {
        foreach (var e in target) yield return e;
        yield return element;
    }

    public static IEnumerable<T> Concat<T>(this IEnumerable<IEnumerable<T>> sequences)
    {
        foreach (var sequence in sequences)
        {
            foreach (var item in sequence)
            {
                yield return item;
            }
        }
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

    public static void Consume<T>(this IEnumerable<T> source)
    {
        foreach (var _ in source) { }
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

    public static Dictionary<TKey, int> CountBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
    where TKey : notnull
    {
        var countsByKey = new Dictionary<TKey, int>();
        foreach (var x in source)
        {
            var key = keySelector(x);
            if (!countsByKey.ContainsKey(key))
                countsByKey[key] = 0;
            countsByKey[key] += 1;
        }
        return countsByKey;
    }

    public static int CountDuplicates<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
    {
        return source.GroupBy(keySelector)
                     .Where(group => group.Count() > 1)
                     .Sum(group => group.Count() - 1);
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

    public static bool EndsWith<T>(this IEnumerable<T> first, IEnumerable<T> second)
    {
        return first.Reverse().Take(second.Count()).SequenceEqual(second.Reverse());
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

    public static IEnumerable<TResult> EquiZip<TFirst, TSecond, TResult>(this IEnumerable<TFirst> first, IEnumerable<TSecond> second, Func<TFirst, TSecond, TResult> resultSelector)
    {
        using (var e1 = first.GetEnumerator())
        using (var e2 = second.GetEnumerator())
        {
            while (e1.MoveNext() && e2.MoveNext())
            {
                yield return resultSelector(e1.Current, e2.Current);
            }
        }
    }

    public static bool Exactly<T>(this IEnumerable<T> source, int count)
    {
        return source.Count() == count;
    }

    public static bool Exactly<T>(this IEnumerable<T> source, Func<T, bool> query, int count)
    {
        return source.Count(query) == count;
    }

    public static bool Exactly<T>(this IEnumerable<T> source, int count, Func<T, bool> predicate)
    {
        return source.Count(predicate) == count;
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

    public static IEnumerable<TSource> ExceptBy<TSource, TKey>(this IEnumerable<TSource> first, IEnumerable<TSource> second, Func<TSource, TKey> keySelector)
    {
        var set = new HashSet<TKey>(second.Select(keySelector));
        foreach (var element in first)
        {
            if (set.Add(keySelector(element)))
            {
                yield return element;
            }
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

    public static IEnumerable<T> Exclude<T>(this IEnumerable<T> source, IEnumerable<T> other)
    {
        return source.Except(other);
    }

    public static IEnumerable<T> Exclude<T>(this IEnumerable<T> source, int index)
    {
        return source.Where((item, idx) => idx != index);
    }

    public static bool Exists<T>(this IEnumerable<T> list, Func<T, bool> predicate)
    {
        return list.Index(predicate) > -1;
    }

    public static IEnumerable<TSource> FallbackIfEmpty<TSource>(this IEnumerable<TSource> source,
            IEnumerable<TSource> fallback)
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

    public static IEnumerable<TSource> FallbackIfEmpty<TSource>(this IEnumerable<TSource> source,
            params TSource[] fallback)
    {
        if (source == null)
        {
            throw new ArgumentNullException(nameof(source));
        }

        return source.Any() ? source : fallback;
    }

    public static IEnumerable<T> FallbackIfEmpty<T>(this IEnumerable<T> source, T fallback)
    {
        return source.Any() ? source : new[] { fallback };
    }

    public static IEnumerable<TSource> FallbackIfNull<TSource>(this IEnumerable<TSource> source,
                IEnumerable<TSource> fallback)
    {
        if (fallback == null)
        {
            throw new ArgumentNullException(nameof(fallback));
        }

        return source ?? fallback;
    }

    public static IEnumerable<TSource> FallbackIfNull<TSource>(this IEnumerable<TSource> source,
            params TSource[] fallback)
    {
        return source ?? fallback;
    }

    public static IEnumerable<TSource> FallbackIfNullOrEmpty<TSource>(this IEnumerable<TSource> source,
            IEnumerable<TSource> fallback)
    {
        return source.FallbackIfNull(fallback).FallbackIfEmpty(fallback);
    }

    public static IEnumerable<TSource> FallbackIfNullOrEmpty<TSource>(this IEnumerable<TSource> source,
            params TSource[] fallback)
    {
        return source.FallbackIfNull(fallback).FallbackIfEmpty(fallback);
    }

    public static IEnumerable<T?> FillBackward<T>(this IEnumerable<T?> source) where T : struct
    {
        T? last = null;
        foreach (var item in source.Reverse())
        {
            if (item.HasValue)
            {
                last = item;
            }
            yield return last;
        }
    }

    public static IEnumerable<T?> FillForward<T>(this IEnumerable<T?> source) where T : struct
    {
        T? last = null;
        foreach (var item in source)
        {
            if (item.HasValue)
            {
                last = item;
            }
            yield return last;
        }
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

    public static IEnumerable<TU> FindDuplicates<T, TU>(this IEnumerable<T> list, Func<T, TU> keySelector)
    {
        return list.GroupBy(keySelector)
            .Where(group => group.Count() > 1)
            .Select(group => group.Key).ToList();
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

    public static IEnumerable<T> Flatten<T>(this IEnumerable<IEnumerable<T>> source)
    {
        return source.SelectMany(x => x);
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

    public static void ForEachIndexed<T>(this IEnumerable<T> source, Action<T, int> action)
    {
        int index = 0;
        foreach (T element in source)
        {
            action(element, index);
            index++;
        }
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

    public static void ForEachReverseIndexed<T>(this IEnumerable<T> source, Action<int, T> action)
    {
        var list = source.ToList();
        for (int i = list.Count - 1; i >= 0; i--)
        {
            action(i, list[i]);
        }
    }

    public static IEnumerable<TResult> FullGroupJoin<TOuter, TInner, TKey, TResult>(
            this IEnumerable<TOuter> outer,
            IEnumerable<TInner> inner,
            Func<TOuter, TKey> outerKeySelector,
            Func<TInner, TKey> innerKeySelector,
            Func<TKey, IEnumerable<TOuter>, IEnumerable<TInner>, TResult> resultSelector)
    {
        var outerLookup = outer.ToLookup(outerKeySelector);
        var innerLookup = inner.ToLookup(innerKeySelector);

        var keys = outerLookup.Select(g => g.Key).Union(innerLookup.Select(g => g.Key));

        return keys.Select(key => resultSelector(key, outerLookup[key], innerLookup[key]));
    }

    public static IEnumerable<TResult> FullJoin<TOuter, TInner, TKey, TResult>(
            this IEnumerable<TOuter> outer,
            IEnumerable<TInner> inner,
            Func<TOuter, TKey> outerKeySelector,
            Func<TInner, TKey> innerKeySelector,
            Func<TOuter, TInner, TResult> resultSelector)
    {
        return outer.GroupJoin(
                inner,
                outerKeySelector,
                innerKeySelector,
                (o, ie) => new { o, ie }
            )
            .SelectMany(
                t => t.ie.DefaultIfEmpty(),
                (t, i) => resultSelector(t.o, i)
            )
            .Concat(
                inner.Where(i => !outer.Any(o => outerKeySelector(o).Equals(innerKeySelector(i))))
                      .Select(i => resultSelector(default, i))
            );
    }

    public static IEnumerable<T> Generate<T>(T seed, Func<T, T> generator)
    {
        var current = seed;
        while (true)
        {
            yield return current;
            current = generator(current);
        }
    }

    public static IEnumerable<TResult> GenerateByIndex<TResult>(int start, Func<int, TResult> generator)
    {
        for (var i = start; ; i++)
        {
            yield return generator(i);
        }
    }

    public static IEnumerable<TSource> GetDuplicates<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
    {
        return source.GroupBy(keySelector)
                     .Where(group => group.Count() > 1)
                     .SelectMany(group => group.Skip(1));
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

    public static IEnumerable<IGrouping<TKey, TSource>> GroupAdjacent<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
    {
        TKey key = default;
        TKey comparer = default;
        var list = new List<TSource>();
        foreach (var item in source)
        {
            var k = keySelector(item);
            if (list.Count > 0 && !EqualityComparer<TKey>.Default.Equals(k, key))
            {
                yield return new Grouping<TKey, TSource>(comparer, list);
                list = new List<TSource> { item };
                key = k;
            }
            else
            {
                if (list.Count == 0) key = k;
                list.Add(item);
            }
            comparer = key;
        }
        if (list.Count > 0)
            yield return new Grouping<TKey, TSource>(comparer, list);
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

    public static bool HasDuplicates<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
    {
        return source.GroupBy(keySelector).Any(group => group.Count() > 1);
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

    public static IEnumerable<IEnumerable<T>> InBatchesOf<T>(this IEnumerable<T> source, int batchSize)
    {
        var batch = new List<T>(batchSize);
        foreach (T item in source)
        {
            batch.Add(item);
            if (batch.Count == batchSize)
            {
                yield return batch.ToArray();
                batch.Clear();
            }
        }

        if (batch.Count > 0)
        {
            yield return batch.ToArray();
        }
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

    public static IEnumerable<(int index, T value)> Index<T>(this IEnumerable<T> source)
    {
        return source.Select((value, index) => (index, value));
    }

    public static IEnumerable<(TKey key, int index, TSource element)> IndexBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
    {
        return source.Select((element, index) => (keySelector(element), index, element));
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

    public static int IndexOf<T>(this IEnumerable<T> source, Func<T, bool> predicate)
    {
        int i = 0;
        foreach (var item in source)
        {
            if (predicate(item))
                return i;
            i++;
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

    public static IEnumerable<T> Insert<T>(this IEnumerable<T> source, int index, T item)
    {
        if (index < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(index));
        }

        var i = 0;
        foreach (var current in source)
        {
            if (i == index)
            {
                yield return item;
            }

            yield return current;
            i++;
        }

        if (index >= i)
        {
            yield return item;
        }
    }

    public static IEnumerable<T> Interleave<T>(this IEnumerable<T> first, IEnumerable<T> second)
    {
        using (var e1 = first.GetEnumerator())
        using (var e2 = second.GetEnumerator())
        {
            while (e1.MoveNext() && e2.MoveNext())
            {
                yield return e1.Current;
                yield return e2.Current;
            }
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

    public static IEnumerable<(TSource element, TSource lag)> Lag<TSource>(this IEnumerable<TSource> source, TSource defaultValue = default)
    {
        var lag = defaultValue;
        foreach (var element in source)
        {
            yield return (element, lag);
            lag = element;
        }
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

    public static IEnumerable<(TSource element, TSource lead)> Lead<TSource>(this IEnumerable<TSource> source, TSource defaultValue = default)
    {
        using var e = source.GetEnumerator();
        if (!e.MoveNext())
        {
            yield break;
        }

        var lead = e.Current;
        while (e.MoveNext())
        {
            yield return (lead, e.Current);
            lead = e.Current;
        }
        yield return (lead, defaultValue);
    }

    public static IEnumerable<TResult> LeftJoin<TOuter, TInner, TKey, TResult>(
        this IEnumerable<TOuter> outer,
        IEnumerable<TInner> inner,
        Func<TOuter, TKey> outerKeySelector,
        Func<TInner, TKey> innerKeySelector,
        Func<TOuter, TInner, TResult> resultSelector)
    {
        return outer.GroupJoin(
            inner,
            outerKeySelector,
            innerKeySelector,
            (outerElement, innerElements) => innerElements
                .Select(innerElement => resultSelector(outerElement, innerElement))
                .DefaultIfEmpty(resultSelector(outerElement, default)))
            .SelectMany(result => result);
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

    public static TSource? MaxBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> selector)
    {
        return source.OrderByDescending(selector).FirstOrDefault();
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

    public static IEnumerable<T> Memoize<T>(this IEnumerable<T> source)
    {
        if (source == null) throw new ArgumentNullException(nameof(source));
        if (source is ICollection<T> collection) return new List<T>(collection);
        return new Memoizer<T>(source.GetEnumerator());
    }

    public static IEnumerable<T> Merge<T>(this IEnumerable<IEnumerable<T>> sequences)
    {
        var enumerators = sequences.Select(s => s.GetEnumerator()).ToList();
        try
        {
            while (enumerators.Count > 0)
            {
                for (int i = 0; i < enumerators.Count; i++)
                {
                    var enumerator = enumerators[i];
                    if (!enumerator.MoveNext())
                    {
                        enumerators.RemoveAt(i--);
                        enumerator.Dispose();
                    }
                    else
                    {
                        yield return enumerator.Current;
                    }
                }
            }
        }
        finally
        {
            foreach (var enumerator in enumerators)
            {
                enumerator.Dispose();
            }
        }
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

    public static TSource? MinBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> selector)
    {
        return source.OrderBy(selector).FirstOrDefault();
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

    public static (T Min, T Max) MinMax<T>(this IEnumerable<T> source) where T : IComparable<T>
    {
        if (!source.Any())
            throw new InvalidOperationException("Sequence contains no elements");

        T min = source.First();
        T max = source.First();
        foreach (var item in source)
        {
            if (item.CompareTo(min) < 0) min = item;
            if (item.CompareTo(max) > 0) max = item;
        }
        return (min, max);
    }

    public static IEnumerable<TSource>? Mins<TSource, TResult>(this IEnumerable<TSource> source,
                Func<TSource, TResult> selector)
    {
        return source.ToLookup(selector).Min(n => n.Key);
    }

    public static IEnumerable<T> Move<T>(this IEnumerable<T> source, int oldIndex, int newIndex)
    {
        var item = source.ElementAt(oldIndex);
        var list = source.ToList();
        list.RemoveAt(oldIndex);
        list.Insert(newIndex, item);
        return list;
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

    public static IEnumerable<T?> OrDefaultIfEmpty<T>(this IEnumerable<T> source)
    {
        return source.DefaultIfEmpty(default(T));
    }

    public static IOrderedEnumerable<TSource> OrderBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, bool ascending = true)
    {
        return ascending ? source.OrderBy(keySelector) : source.OrderByDescending(keySelector);
    }

    public static IEnumerable<T> OrderedMerge<T>(this IEnumerable<T> first, IEnumerable<T> second, IComparer<T> comparer = null)
    {
        using (var firstEnumerator = first.GetEnumerator())
        using (var secondEnumerator = second.GetEnumerator())
        {
            var fHasValue = firstEnumerator.MoveNext();
            var sHasValue = secondEnumerator.MoveNext();
            while (fHasValue && sHasValue)
            {
                if ((comparer ?? Comparer<T>.Default).Compare(firstEnumerator.Current, secondEnumerator.Current) <= 0)
                {
                    yield return firstEnumerator.Current;
                    fHasValue = firstEnumerator.MoveNext();
                }
                else
                {
                    yield return secondEnumerator.Current;
                    sHasValue = secondEnumerator.MoveNext();
                }
            }

            while (fHasValue)
            {
                yield return firstEnumerator.Current;
                fHasValue = firstEnumerator.MoveNext();
            }

            while (sHasValue)
            {
                yield return secondEnumerator.Current;
                sHasValue = secondEnumerator.MoveNext();
            }
        }
    }

    // Pad extension methods
    public static IEnumerable<T> Pad<T>(this IEnumerable<T> source, int width)
    {
        return source.Pad(width, default(T));
    }

    public static IEnumerable<T> Pad<T>(this IEnumerable<T> source, int width, T value)
    {
        return source.Concat(Enumerable.Repeat(value, Math.Max(0, width - source.Count())));
    }

    public static IEnumerable<T> PadStart<T>(this IEnumerable<T> source, int width)
    {
        return source.PadStart(width, default(T));
    }

    public static IEnumerable<T> PadStart<T>(this IEnumerable<T> source, int width, T value)
    {
        return Enumerable.Repeat(value, Math.Max(0, width - source.Count())).Concat(source);
    }

    public static IEnumerable<TResult> Pairwise<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, TSource, TResult> selector)
    {
        TSource previous = default;
        bool isFirst = true;

        foreach (TSource element in source)
        {
            if (!isFirst)
            {
                yield return selector(previous, element);
            }

            isFirst = false;
            previous = element;
        }
    }

    public static IEnumerable<T> PartialSort<T>(this IEnumerable<T> source, int count) where T : IComparable<T>
    {
        return source.OrderBy(x => x).Take(count);
    }

    public static IEnumerable<T> PartialSort<T>(this IEnumerable<T> source, int count, IComparer<T> comparer)
    {
        return source.OrderBy(x => x, comparer).Take(count);
    }

    public static IEnumerable<T> PartialSortBy<T, TKey>(this IEnumerable<T> source, int count, Func<T, TKey> keySelector) where TKey : IComparable<TKey>
    {
        return source.OrderBy(keySelector).Take(count);
    }

    public static IEnumerable<T> PartialSortBy<T, TKey>(this IEnumerable<T> source, int count, Func<T, TKey> keySelector, IComparer<TKey> comparer)
    {
        return source.OrderBy(keySelector, comparer).Take(count);
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

    public static IEnumerable<IEnumerable<T>> Partition<T>(this IEnumerable<T> source, int size)
    {
        var partition = new List<T>(size);
        foreach (var item in source)
        {
            partition.Add(item);
            if (partition.Count == size)
            {
                yield return partition;
                partition = new List<T>(size);
            }
        }
        if (partition.Count > 0)
        {
            yield return partition;
        }
    }

    public static (IEnumerable<T> True, IEnumerable<T> False) PartitionBlock<T>(this IEnumerable<T> source, Func<T, bool> predicate)
    {
        var trueList = new List<T>();
        var falseList = new List<T>();

        foreach (var element in source)
        {
            if (predicate(element))
            {
                trueList.Add(element);
            }
            else
            {
                falseList.Add(element);
            }
        }

        return (trueList, falseList);
    }

    public static IEnumerable<IGrouping<TKey, TSource>> PartitionBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
    {
        TKey key = default;
        TKey comparer = default;
        var list = new List<TSource>();
        foreach (var item in source)
        {
            var k = keySelector(item);
            if (list.Count > 0 && !EqualityComparer<TKey>.Default.Equals(k, key))
            {
                yield return new Grouping<TKey, TSource>(comparer, list);
                list = new List<TSource> { item };
                key = k;
            }
            else
            {
                if (list.Count == 0) key = k;
                list.Add(item);
            }
            comparer = key;
        }
        if (list.Count > 0)
            yield return new Grouping<TKey, TSource>(comparer, list);
    }

    public static string PathCombine(this IEnumerable<string> enumerable)
    {
        if (enumerable is null) throw new ArgumentNullException(nameof(enumerable));

        return Path.Combine(enumerable.ToArray());
    }

    public static decimal Percentile(this IEnumerable<decimal> seq, decimal percentile)
    {
        var sorted = seq.OrderBy(x => x).ToList();
        var index = percentile * (sorted.Count - 1);
        var whole = decimal.Floor(index);
        var fraction = index - whole;

        if (whole == sorted.Count - 1)
            return sorted[(int)whole];

        var lower = sorted[(int)whole];
        var upper = sorted[(int)whole + 1];

        return lower + ((upper - lower) * fraction);
    }

    public static IEnumerable<T> Perform<T>(this IEnumerable<T> collection, Func<T, T> func)
    {
        if (collection == null) throw new ArgumentNullException(nameof(collection));
        if (func == null) throw new ArgumentNullException(nameof(func));
        foreach (var item in collection) yield return func(item);
    }

    public static IEnumerable<T> Pipe<T>(this IEnumerable<T> source, Action<T> action)
    {
        if (source == null)
            throw new ArgumentNullException(nameof(source));
        if (action == null)
            throw new ArgumentNullException(nameof(action));

        foreach (T item in source)
        {
            action(item);
            yield return item;
        }
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

    public static IEnumerable<T> PreScan<T>(this IEnumerable<T> source, Func<T, T, T> function)
    {
        if (source == null)
            throw new ArgumentNullException(nameof(source));
        if (function == null)
            throw new ArgumentNullException(nameof(function));

        T runningTotal = default;
        foreach (T item in source)
        {
            runningTotal = function(runningTotal, item);
            yield return runningTotal;
        }
    }

    public static T Random<T>(this IEnumerable<T> source)
    {
        var asReadOnlyList = source as IReadOnlyList<T> ?? source.ToArray();

        if (!asReadOnlyList.Any())
            throw new InvalidOperationException("Sequence contains no elements.");

        return asReadOnlyList[Rnd.Next(0, asReadOnlyList.Count)];
    }

    public static T? RandomElement<T>(this IEnumerable<T> sequence)
    {
        if (!sequence.Any())
        {
            return default;
        }
        return sequence.ElementAt(Rnd.Next(0, sequence.Count()));
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

    public static IEnumerable<(T item, int rank)> Rank<T>(this IEnumerable<T> source) where T : IComparable<T>
    {
        if (source == null)
            throw new ArgumentNullException(nameof(source));

        return source.Select(item => (item, rank: 1))
                     .OrderBy(tuple => tuple.item)
                     .Select((tuple, index) => (tuple.item, rank: index + 1));
    }

    public static IEnumerable<(T item, int rank)> RankBy<T, TKey>(this IEnumerable<T> source, Func<T, TKey> keySelector) where TKey : IComparable<TKey>
    {
        if (source == null)
            throw new ArgumentNullException(nameof(source));
        if (keySelector == null)
            throw new ArgumentNullException(nameof(keySelector));

        return source.Select(item => (item, key: keySelector(item)))
                     .OrderBy(tuple => tuple.key)
                     .Select((tuple, index) => (tuple.item, rank: index + 1));
    }

    public static T ReachAt<T>(this IEnumerable<T> collection, int index)
    {
        if (collection == null)
            throw new ArgumentNullException(nameof(collection));

        int count = collection.Count();

        if (index >= 0)
        {
            if (index >= count)
                throw new ArgumentOutOfRangeException(nameof(index));
            return collection.ElementAt(index);
        }
        else
        {
            if (-index > count)
                throw new ArgumentOutOfRangeException(nameof(index));
            return collection.ElementAt(count + index);
        }
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

    public static IEnumerable<T> RemoveWhile<T>(this IEnumerable<T> source, Func<T, bool> predicate)
    {
        var yieldStarted = false;

        foreach (var element in source)
        {
            if (!yieldStarted && !predicate(element))
            {
                yieldStarted = true;
            }

            if (yieldStarted)
            {
                yield return element;
            }
        }
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

    public static IEnumerable<T> Repeat<T>(this T item, int count)
    {
        for (int i = 0; i < count; i++)
        {
            yield return item;
        }
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

    public static IEnumerable<T> Return<T>(this T item)
    {
        yield return item;
    }

    public static IEnumerable<TResult> RightJoin<TOuter, TInner, TKey, TResult>(
            this IEnumerable<TOuter> outer,
            IEnumerable<TInner> inner,
            Func<TOuter, TKey> outerKeySelector,
            Func<TInner, TKey> innerKeySelector,
            Func<TOuter, TInner, TResult> resultSelector)
    {
        return inner.GroupJoin(
            outer,
            innerKeySelector,
            outerKeySelector,
            (innerElement, outerElements) => outerElements
                .Select(outerElement => resultSelector(outerElement, innerElement))
                .DefaultIfEmpty(resultSelector(default, innerElement)))
            .SelectMany(result => result);
    }

    public static IEnumerable<(TKey key, int count)> RunLengthEncode<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
    {
        using var e = source.GetEnumerator();
        if (!e.MoveNext())
        {
            yield break;
        }

        var key = keySelector(e.Current);
        var count = 1;
        while (e.MoveNext())
        {
            var nextKey = keySelector(e.Current);
            if (EqualityComparer<TKey>.Default.Equals(nextKey, key))
            {
                count++;
            }
            else
            {
                yield return (key, count);
                key = nextKey;
                count = 1;
            }
        }

        yield return (key, count);
    }

    public static IEnumerable<T> Scan<T>(this IEnumerable<T> source, Func<T, T, T> func)
    {
        using (var enumerator = source.GetEnumerator())
        {
            if (!enumerator.MoveNext())
            {
                yield break;
            }

            var accumulator = enumerator.Current;
            yield return accumulator;

            while (enumerator.MoveNext())
            {
                accumulator = func(accumulator, enumerator.Current);
                yield return accumulator;
            }
        }
    }

    public static IEnumerable<TResult> ScanBy<TSource, TKey, TAccumulate, TResult>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TAccumulate> seedSelector, Func<TKey, TAccumulate, TSource, TAccumulate> func, Func<TKey, TAccumulate, TResult> resultSelector)
    {
        var seedKey = default(TKey);
        var seed = default(TAccumulate);
        bool seedInitialized = false;

        foreach (var item in source)
        {
            var key = keySelector(item);

            if (!seedInitialized)
            {
                seedKey = key;
                seed = seedSelector(item);
                seedInitialized = true;
            }
            else if (!EqualityComparer<TKey>.Default.Equals(seedKey, key))
            {
                yield return resultSelector(seedKey, seed);
                seedKey = key;
                seed = seedSelector(item);
            }
            else
            {
                seed = func(seedKey, seed, item);
            }
        }

        if (seedInitialized)
        {
            yield return resultSelector(seedKey, seed);
        }
    }

    public static IEnumerable<T> ScanRight<T>(this IEnumerable<T> source, Func<T, T, T> func)
    {
        var buffer = source.ToList();
        if (!buffer.Any())
        {
            yield break;
        }

        var accumulator = buffer.Last();
        yield return accumulator;

        for (int i = buffer.Count - 2; i >= 0; i--)
        {
            accumulator = func(accumulator, buffer[i]);
            yield return accumulator;
        }
    }

    public static IEnumerable<IEnumerable<T>> Segment<T>(this IEnumerable<T> source, int size)
    {
        var segment = new List<T>(size);
        foreach (var item in source)
        {
            segment.Add(item);
            if (segment.Count == size)
            {
                yield return segment.ToList();
                segment.Clear();
            }
        }

        if (segment.Any())
        {
            yield return segment;
        }
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

    public static IEnumerable<TResult> SelectIndexed<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, int, TResult> selector)
    {
        int index = 0;
        foreach (TSource element in source)
        {
            yield return selector(element, index);
            index++;
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

    public static IEnumerable<TResult> SelectWhere<TSource, TResult>(this IEnumerable<TSource> source,
        Func<TSource, bool> predicate,
        Func<TSource, TResult> selector)
    {
        foreach (var item in source)
        {
            if (predicate(item))
            {
                yield return selector(item);
            }
        }
    }

    public static IEnumerable<TResult> SelectWhere<TSource, TResult>(this IEnumerable<TSource> source,
        Func<TSource, bool> predicate,
        Func<TSource, TResult> selector,
        TResult defaultValue)
    {
        foreach (var item in source)
        {
            yield return predicate(item) ? selector(item) : defaultValue;
        }
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

    public static bool SequenceEqualBy<TSource, TKey>(this IEnumerable<TSource> source, IEnumerable<TSource> other, Func<TSource, TKey> keySelector, IEqualityComparer<TKey> comparer = null)
    {
        comparer = comparer ?? EqualityComparer<TKey>.Default;
        return source.Select(keySelector).SequenceEqual(other.Select(keySelector), comparer);
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

    public static TSource Single<TSource, TException>(this IEnumerable<TSource> source,
        Func<TSource, bool> predicate,
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

    public static IEnumerable<T> SkipUntil<T>(this IEnumerable<T> source, Func<T, bool> predicate)
    {
        bool yield = false;
        foreach (var item in source)
        {
            if (!yield && predicate(item))
            {
                yield = true;
            }
            if (yield)
            {
                yield return item;
            }
        }
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

    public static IEnumerable<T> Slice<T>(this IEnumerable<T> source, int start, int? count = null)
    {
        if (count == null)
        {
            return source.Skip(start);
        }
        else
        {
            return source.Skip(start).Take(count.Value);
        }
    }

    public static IEnumerable<T> SortedMerge<T>(this IEnumerable<T> first, IEnumerable<T> second, IComparer<T> comparer = null)
    {
        using (var firstEnumerator = first.GetEnumerator())
        using (var secondEnumerator = second.GetEnumerator())
        {
            var fHasValue = firstEnumerator.MoveNext();
            var sHasValue = secondEnumerator.MoveNext();
            while (fHasValue && sHasValue)
            {
                if ((comparer ?? Comparer<T>.Default).Compare(firstEnumerator.Current, secondEnumerator.Current) <= 0)
                {
                    yield return firstEnumerator.Current;
                    fHasValue = firstEnumerator.MoveNext();
                }
                else
                {
                    yield return secondEnumerator.Current;
                    sHasValue = secondEnumerator.MoveNext();
                }
            }

            while (fHasValue)
            {
                yield return firstEnumerator.Current;
                fHasValue = firstEnumerator.MoveNext();
            }

            while (sHasValue)
            {
                yield return secondEnumerator.Current;
                sHasValue = secondEnumerator.MoveNext();
            }
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

    public static IEnumerable<IEnumerable<T>> Split<T>(this IEnumerable<T> source, T separator)
    {
        var list = new List<T>();
        foreach (var item in source)
        {
            if (EqualityComparer<T>.Default.Equals(item, separator))
            {
                if (list.Count > 0)
                {
                    yield return list;
                    list = new List<T>();
                }
            }
            else
            {
                list.Add(item);
            }
        }

        if (list.Count > 0)
        {
            yield return list;
        }
    }

    public static bool StartsWith<T>(this IEnumerable<T> source, IEnumerable<T> prefix)
    {
        using (var sourceIter = source.GetEnumerator())
        using (var prefixIter = prefix.GetEnumerator())
        {
            while (prefixIter.MoveNext())
            {
                if (!(sourceIter.MoveNext() && EqualityComparer<T>.Default.Equals(sourceIter.Current, prefixIter.Current)))
                {
                    return false;
                }
            }
            return true;
        }
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

    public static IEnumerable<IEnumerable<T>> Subsets<T>(this IEnumerable<T> source)
    {
        var list = source.ToList();
        var count = 1 << list.Count;
        for (var i = 0; i < count; i++)
        {
            yield return list.Where((item, j) => ((i >> j) & 1) != 0);
        }
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

    public static IEnumerable<(T item, bool isFirst, bool isLast)> TagFirstLast<T>(this IEnumerable<T> source)
    {
        using (var iter = source.GetEnumerator())
        {
            if (!iter.MoveNext())
            {
                yield break;
            }

            var first = iter.Current;
            if (!iter.MoveNext())
            {
                yield return (first, true, true);
                yield break;
            }

            yield return (first, true, false);
            var prev = iter.Current;
            while (iter.MoveNext())
            {
                yield return (prev, false, false);
                prev = iter.Current;
            }

            yield return (prev, false, true);
        }
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

    public static IEnumerable<T> TakeEvery<T>(this IEnumerable<T> source, int step)
    {
        return source.Where((item, index) => index % step == 0);
    }

    public static IEnumerable<T> TakeLast<T>(this IEnumerable<T> source, int n)
    {
        var enumerable = source as T[] ?? source.ToArray();
        return enumerable.Skip(Math.Max(0, enumerable.Length - n));
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

    public static IOrderedEnumerable<T> ThenBy<T>(this IEnumerable<T> source, Comparison<T> comparison)
    {
        return (source as IOrderedEnumerable<T>)?.ThenBy(x => x, Comparer<T>.Create(comparison)) ?? source.OrderBy(x => x, Comparer<T>.Create(comparison));
    }

    public static TResult[] ToArray<TSource, TResult>(this IEnumerable<TSource> source,
            Func<TSource, TResult> selector)
    {
        return source.Select(selector).ToArray();
    }

    public static T[] ToArrayByIndex<T>(this IEnumerable<T> source, Func<T, int> indexSelector)
    {
        var arr = new T[source.Select(indexSelector).DefaultIfEmpty(0).Max() + 1];
        foreach (var item in source)
        {
            arr[indexSelector(item)] = item;
        }
        return arr;
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

    public static string ToDelimitedString<T>(this IEnumerable<T> source, string delimiter)
    {
        return string.Join(delimiter, source);
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

    public static Dictionary<TKey, TElement> ToDictionary<TSource, TKey, TElement>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, IEqualityComparer<TKey> comparer, bool overwriteOnDuplicate)
        where TKey : notnull
    {
        var dictionary = new Dictionary<TKey, TElement>(comparer);
        foreach (var element in source)
        {
            var key = keySelector(element);
            var value = elementSelector(element);
            if (overwriteOnDuplicate || !dictionary.ContainsKey(key))
            {
                dictionary[key] = value;
            }
        }
        return dictionary;
    }

    public static Dictionary<TKey, TElement> ToDictionarySafe<TSource, TKey, TElement>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, IEqualityComparer<TKey>? comparer = null)
        where TKey : notnull
    {
        var dictionary = new Dictionary<TKey, TElement>(comparer);
        foreach (var element in source)
        {
            var key = keySelector(element);
            if (!dictionary.ContainsKey(key))
            {
                dictionary[key] = elementSelector(element);
            }
        }
        return dictionary;
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

    public static HashSet<T> ToHashSet<T>(this IEnumerable<T> source, IEqualityComparer<T>? comparer = null)
    {
        return new HashSet<T>(source, comparer);
    }

    public static List<TResult> ToList<TSource, TResult>(this IEnumerable<TSource> source,
            Func<TSource, TResult> selector)
    {
        return source.Select(selector).ToList();
    }

    public static ILookup<TKey, TElement> ToLookup<TSource, TKey, TElement>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, IEqualityComparer<TKey> comparer, bool excludeNulls)
    {
        return excludeNulls
            ? source.Where(x => keySelector(x) != null).ToLookup(keySelector, elementSelector, comparer)
            : source.ToLookup(keySelector, elementSelector, comparer);
    }

    public static ObservableCollection<T> ToObservableCollection<T>(this IEnumerable<T> enumerable)
    {
        if (enumerable is null) throw new ArgumentNullException(nameof(enumerable));

        return new ObservableCollection<T>(enumerable);
    }

    public static IEnumerable<T> Top<T>(this IEnumerable<T> source, int count, IComparer<T> comparer)
    {
        return source.OrderByDescending(t => t, comparer).Take(count);
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

    public static IEnumerable<T> Trace<T>(this IEnumerable<T> source, string message)
    {
        foreach (var item in source)
        {
            Debug.WriteLine($"{message}: {item}");
            yield return item;
        }
    }

    public static IEnumerable<IEnumerable<T>> Transpose<T>(this IEnumerable<IEnumerable<T>> source)
    {
        var enumerators = source.Select(x => x.GetEnumerator()).ToList();
        try
        {
            while (enumerators.All(x => x.MoveNext()))
            {
                yield return enumerators.Select(x => x.Current).ToList();
            }
        }
        finally
        {
            foreach (var enumerator in enumerators)
            {
                enumerator.Dispose();
            }
        }
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

    public static IEnumerable<T> TraverseBreadthFirst<T>(this IEnumerable<T> source, Func<T, IEnumerable<T>> childSelector)
    {
        var queue = new Queue<T>(source);
        while (queue.Count > 0)
        {
            var next = queue.Dequeue();
            yield return next;
            foreach (var child in childSelector(next))
            {
                queue.Enqueue(child);
            }
        }
    }

    public static IEnumerable<T> TraverseDepthFirst<T>(this IEnumerable<T> source, Func<T, IEnumerable<T>> childSelector)
    {
        var stack = new Stack<T>(source);
        while (stack.Count > 0)
        {
            var next = stack.Pop();
            yield return next;
            foreach (var child in childSelector(next))
            {
                stack.Push(child);
            }
        }
    }

    public static bool TryFirst<T>(this IEnumerable<T> source, out T? value)
    {
        foreach (var item in source)
        {
            value = item;
            return true;
        }

        value = default;
        return false;
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

    public static bool TrySingle<T>(this IEnumerable<T> source, out T value)
    {
        var enumerator = source.GetEnumerator();
        if (!enumerator.MoveNext())
        {
            value = default;
            return false;
        }
        value = enumerator.Current;
        return !enumerator.MoveNext();
    }

    public static IEnumerable<TResult> Unfold<TState, TResult>(TState seed, Func<TState, (TResult result, TState next)> generator)
    {
        var state = seed;
        while (true)
        {
            var (result, next) = generator(state);
            yield return result;
            state = next;
        }
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

    public static IEnumerable<T> WhereIf<T>(this IEnumerable<T> source, bool condition,
        Func<T, int, bool> predicate)
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

    public static IEnumerable<IEnumerable<T>> Window<T>(this IEnumerable<T> source, int size)
    {
        var queue = new Queue<T>(size);
        foreach (var item in source)
        {
            if (queue.Count == size)
            {
                queue.Dequeue();
            }
            queue.Enqueue(item);
            if (queue.Count == size)
            {
                yield return queue.ToList();
            }
        }
    }

    public static IEnumerable<(IEnumerable<T> left, T right)> WindowLeft<T>(this IEnumerable<T> source, int size)
    {
        var queue = new Queue<T>(size);
        foreach (var item in source)
        {
            if (queue.Count == size)
            {
                queue.Dequeue();
            }
            yield return (queue.ToList(), item);
            queue.Enqueue(item);
        }
    }

    public static IEnumerable<(T left, IEnumerable<T> right)> WindowRight<T>(this IEnumerable<T> source, int size)
    {
        var queue = new Queue<T>(size + 1);
        foreach (var item in source)
        {
            queue.Enqueue(item);
            if (queue.Count == size + 1)
            {
                yield return (queue.Dequeue(), queue.ToList());
            }
        }
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

    public static IEnumerable<TSource> Without<TSource>(this IEnumerable<TSource> source,
        IEnumerable<TSource> elements)
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

    public static IEnumerable<TResult> ZipLongest<TFirst, TSecond, TResult>(this IEnumerable<TFirst> first, IEnumerable<TSecond> second, Func<TFirst, TSecond, TResult> resultSelector, TFirst defaultFirst = default, TSecond defaultSecond = default)
    {
        using (var iterFirst = first.GetEnumerator())
        using (var iterSecond = second.GetEnumerator())
        {
            while (iterFirst.MoveNext() || iterSecond.MoveNext())
            {
                yield return resultSelector(
                    iterFirst.Current ?? defaultFirst,
                    iterSecond.Current ?? defaultSecond);
            }
        }
    }

    public static IEnumerable<TResult> ZipShortest<TFirst, TSecond, TResult>(this IEnumerable<TFirst> first, IEnumerable<TSecond> second, Func<TFirst, TSecond, TResult> resultSelector)
    {
        using (var iterFirst = first.GetEnumerator())
        using (var iterSecond = second.GetEnumerator())
        {
            while (iterFirst.MoveNext() && iterSecond.MoveNext())
            {
                yield return resultSelector(iterFirst.Current, iterSecond.Current);
            }
        }
    }

    public static IEnumerable<TResult> ZipThree<T1, T2, T3, TResult>(
    this IEnumerable<T1> source,
    IEnumerable<T2> second,
    IEnumerable<T3> third,
    Func<T1, T2, T3, TResult> resultSelector)
    {
        using (var e1 = source.GetEnumerator())
        using (var e2 = second.GetEnumerator())
        using (var e3 = third.GetEnumerator())
        {
            while (e1.MoveNext() && e2.MoveNext() && e3.MoveNext())
            {
                yield return resultSelector(e1.Current, e2.Current, e3.Current);
            }
        }
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

    private static IEnumerable<T> YieldBatchElements<T>(IEnumerator<T> source, int batchSize)
    {
        yield return source.Current;
        for (int i = 0; i < batchSize && source.MoveNext(); i++)
        {
            yield return source.Current;
        }
    }

    private class Grouping<TKey, TElement> : IGrouping<TKey, TElement>
    {
        private readonly IEnumerable<TElement> elements;
        private readonly TKey key;

        public Grouping(TKey key, IEnumerable<TElement> elements)
        {
            this.key = key;
            this.elements = elements;
        }

        public TKey Key => this.key;

        public IEnumerator<TElement> GetEnumerator() => this.elements.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
    }

    private class Memoizer<T> : IEnumerable<T>
    {
        private readonly List<T> _cache = new List<T>();
        private readonly IEnumerator<T> _source;

        public Memoizer(IEnumerator<T> source)
        {
            _source = source ?? throw new ArgumentNullException(nameof(source));
        }

        public IEnumerator<T> GetEnumerator()
        {
            int i = 0;
            while (true)
            {
                if (_cache.Count <= i)
                {
                    if (!_source.MoveNext()) yield break;
                    _cache.Add(_source.Current);
                }
                yield return _cache[i++];
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}