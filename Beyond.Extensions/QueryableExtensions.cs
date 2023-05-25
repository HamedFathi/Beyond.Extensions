// ReSharper disable CheckNamespace
// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedType.Global

using Beyond.Extensions.EnumerableExtended;
using Beyond.Extensions.ExpressionExtended;
using Beyond.Extensions.Types;

namespace Beyond.Extensions.QueryableExtended;

public static class QueryableExtensions
{
    public static IQueryable<TSource> FallbackIfEmpty<TSource>(this IQueryable<TSource> source, IQueryable<TSource> fallback)
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

    public static IQueryable<TSource> FallbackIfEmpty<TSource>(this IQueryable<TSource> source, IEnumerable<TSource> fallback)
    {
        if (source == null)
        {
            throw new ArgumentNullException(nameof(source));
        }

        if (fallback == null)
        {
            throw new ArgumentNullException(nameof(fallback));
        }

        return source.Any() ? source : fallback.AsQueryable();
    }

    public static IQueryable<TSource> FallbackIfEmpty<TSource>(this IQueryable<TSource> source, params TSource[] fallback)
    {
        return source.FallbackIfEmpty((IEnumerable<TSource>)fallback);
    }

    public static IQueryable<TSource> FallbackIfNull<TSource>(this IQueryable<TSource> source, IQueryable<TSource> fallback)
    {
        if (fallback == null)
        {
            throw new ArgumentNullException(nameof(fallback));
        }

        return source ?? fallback;
    }

    public static IQueryable<TSource> FallbackIfNull<TSource>(this IQueryable<TSource> source, IEnumerable<TSource> fallback)
    {
        if (fallback == null)
        {
            throw new ArgumentNullException(nameof(fallback));
        }

        return source ?? fallback.AsQueryable();
    }

    public static IQueryable<TSource> FallbackIfNull<TSource>(this IQueryable<TSource> source, params TSource[] fallback)
    {
        return source.FallbackIfNull((IEnumerable<TSource>)fallback);
    }

    public static IQueryable<TSource> FallbackIfNullOrEmpty<TSource>(this IQueryable<TSource> source, IQueryable<TSource> fallback)
    {
        return source.FallbackIfNull(fallback).FallbackIfEmpty(fallback);
    }

    public static IQueryable<TSource> FallbackIfNullOrEmpty<TSource>(this IQueryable<TSource> source, IEnumerable<TSource> fallback)
    {
        return source.FallbackIfNull(fallback).FallbackIfEmpty(fallback);
    }

    public static IQueryable<TSource> FallbackIfNullOrEmpty<TSource>(this IQueryable<TSource> source, params TSource[] fallback)
    {
        return source.FallbackIfNullOrEmpty((IEnumerable<TSource>)fallback);
    }

    public static IQueryable<TResult> FullOuterJoin<TLeft, TRight, TKey, TResult>(
        this IQueryable<TLeft> left,
        IQueryable<TRight> right,
        Expression<Func<TLeft, TKey>> leftKeySelector,
        Expression<Func<TRight, TKey>> rightKeySelector,
        Expression<Func<JoinResult<TLeft, TRight>, TResult>> resultSelector)
    {
        var leftOuterJoinResult = left.LeftOuterJoin(right, leftKeySelector, rightKeySelector, resultSelector);
        var rightOuterJoinResult = right
            .LeftOuterJoin(
                left,
                rightKeySelector,
                leftKeySelector,
                jr => new JoinResult<TLeft, TRight>
                {
                    Left = jr.Right,
                    Right = jr.Left
                })
            .Select(resultSelector);

        return leftOuterJoinResult.Union(rightOuterJoinResult);
    }

    public static IQueryable<T> If<T>(
            this IQueryable<T> query,
        bool should,
        params Func<IQueryable<T>, IQueryable<T>>[] transforms)
    {
        return should
            ? transforms.Aggregate(query,
                (current, transform) => transform.Invoke(current))
            : query;
    }

    public static IQueryable<TResult> LeftOuterJoin<TLeft, TRight, TKey, TResult>(
        this IQueryable<TLeft> left,
        IEnumerable<TRight> right,
        Expression<Func<TLeft, TKey>> leftKeySelector,
        Expression<Func<TRight, TKey>> rightKeySelector,
        Expression<Func<JoinResult<TLeft, TRight>, TResult>> resultSelector)
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
                x => x.Values.Select(c => new JoinResult<TLeft, TRight>
                {
                    Left = x.Key,
                    Right = c
                }))
            .Select(resultSelector);
    }

    public static ICollection<T> ToCollection<T>(this IQueryable<T> queryable)
    {
        return queryable.AsEnumerable().ToCollection();
    }

    public static HashSet<T> ToHashSet<T>(this IQueryable<T> queryable)
    {
        return queryable.AsEnumerable().ToHashSet();
    }

    public static ObservableCollection<T> ToObservableCollection<T>(this IQueryable<T> queryable)
    {
        return queryable.AsEnumerable().ToObservableCollection();
    }

    public static IEnumerable<TEntity> ToPaged<TEntity>(this IQueryable<TEntity> query, int pageIndex, int pageSize)
    {
        return query.Skip((pageIndex - 1) * pageSize).Take(pageSize);
    }

    public static IPagedList<T> ToPagedList<T>(this IQueryable<T> source, int pageNumber, int pageSize)
    {
        return new PagedList<T>(source, pageNumber, pageSize);
    }

    public static IReadOnlyCollection<T> ToReadOnlyCollection<T>(this IQueryable<T> queryable)
    {
        return queryable.AsEnumerable().ToReadOnlyCollection();
    }

    public static IQueryable<T> WhereNot<T>(this IQueryable<T> queryable, Expression<Func<T, bool>> predicate)
    {
        return queryable.Where(predicate.Not());
    }
}