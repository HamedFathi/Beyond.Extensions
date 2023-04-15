namespace Beyond.Extensions.Types;

public interface IPagedList<T>
{
    int FirstItemOnPage { get; }
    bool HasNextPage { get; }
    bool HasPreviousPage { get; }
    bool IsFirstPage { get; }
    bool IsLastPage { get; }
    IList<T> Items { get; }
    int LastItemOnPage { get; }
    int PageCount { get; }
    int PageNumber { get; }
    int PageSize { get; }
    int TotalCount { get; }
    T this[int index] { get; }
}