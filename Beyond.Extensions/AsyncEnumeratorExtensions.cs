// ReSharper disable CheckNamespace
// ReSharper disable UnusedMember.Global

using Beyond.Extensions.Types;

namespace Beyond.Extensions;

public static class AsyncEnumeratorExtensions
{
    public static async Task<int> CountAsync<T>(this IAsyncEnumerable<T> source)
    {
        var count = 0;
        await foreach (var item in source)
        {
            count++;
        }
        return count;
    }

    public static async IAsyncEnumerable<T> SkipAsync<T>(this IAsyncEnumerable<T> source, int count)
    {
        var skipped = 0;
        await foreach (var item in source)
        {
            if (skipped++ < count) continue;
            yield return item;
        }
    }

    public static async IAsyncEnumerable<T> TakeAsync<T>(this IAsyncEnumerable<T> source, int count)
    {
        var taken = 0;
        await foreach (var item in source)
        {
            if (taken++ >= count) break;
            yield return item;
        }
    }

    public static IPagedList<T> ToAsyncPagedList<T>(this IAsyncEnumerable<T> source, int pageNumber, int pageSize)
    {
        return new AsyncPagedList<T>(source, pageNumber, pageSize);
    }

    public static async Task<IList<TSource>> ToListAsync<TSource>(this IAsyncEnumerable<TSource> source,
                        CancellationToken cancellationToken = default)
    {
        if (source == null) throw new ArgumentNullException(nameof(source));
        var list = new List<TSource>();
        await foreach (var item in source.WithCancellation(cancellationToken).ConfigureAwait(false))
            list.Add(item);

        return list;
    }

    public static async Task<List<T>> ToListAsync<T>(this IAsyncEnumerable<T> source)
    {
        var list = new List<T>();
        await foreach (var item in source)
        {
            list.Add(item);
        }
        return list;
    }

    public static async ValueTask<IList<TSource>> ToValueTaskListAsync<TSource>(
            this IAsyncEnumerable<TSource> source,
        CancellationToken cancellationToken = default)
    {
        if (source == null) throw new ArgumentNullException(nameof(source));
        var list = new List<TSource>();
        await foreach (var item in source.WithCancellation(cancellationToken).ConfigureAwait(false))
            list.Add(item);

        return list;
    }
}

public static class AsyncEnumeratorGlobalExtensions
{
    public static IAsyncEnumerator<T> GetAsyncEnumerator<T>(this IAsyncEnumerator<T> enumerator)
    {
        return enumerator;
    }
}