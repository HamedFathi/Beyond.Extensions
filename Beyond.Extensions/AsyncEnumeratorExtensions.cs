// ReSharper disable CheckNamespace
// ReSharper disable UnusedMember.Global

namespace Beyond.Extensions;

public static class AsyncEnumeratorExtensions
{
    public static async Task<IList<TSource>> ToListAsync<TSource>(this IAsyncEnumerable<TSource> source,
        CancellationToken cancellationToken = default)
    {
        if (source == null) throw new ArgumentNullException(nameof(source));
        var list = new List<TSource>();
        await foreach (var item in source.WithCancellation(cancellationToken).ConfigureAwait(false))
            list.Add(item);

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

[SuppressMessage("Design", "CA1050:Declare types in namespaces")]
public static class AsyncEnumeratorGlobalExtensions
{
    public static IAsyncEnumerator<T> GetAsyncEnumerator<T>(this IAsyncEnumerator<T> enumerator)
    {
        return enumerator;
    }
}