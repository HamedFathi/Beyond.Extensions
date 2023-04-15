namespace Beyond.Extensions.Types;

[Serializable]
public class PagedList<T> : IPagedList<T>
{
    public PagedList(IQueryable<T> source, int pageNumber, int pageSize)
    {
        PageNumber = pageNumber;
        PageSize = pageSize;
        TotalCount = source.Count();
        PageCount = (int)Math.Ceiling(TotalCount / (double)PageSize);
        Items = source.Skip((PageNumber - 1) * PageSize).Take(PageSize).ToList();
    }
    public T this[int index]
    {
        get
        {
            if (index < 0 || index >= Items.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(index), "Index is out of range.");
            }
            return Items[index];
        }
    }
    public int FirstItemOnPage => (PageNumber - 1) * PageSize + 1;
    public bool HasNextPage => PageNumber < PageCount;
    public bool HasPreviousPage => PageNumber > 1;
    public bool IsFirstPage => PageNumber == 1;
    public bool IsLastPage => PageNumber == PageCount;
    public int LastItemOnPage => FirstItemOnPage + Items.Count - 1;
    public int PageCount { get; }
    public int PageNumber { get; }
    public int PageSize { get; }
    public int TotalCount { get; }
    public IList<T> Items { get; }
}
