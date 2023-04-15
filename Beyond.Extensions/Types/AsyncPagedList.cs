namespace Beyond.Extensions.Types;

[Serializable]
public class AsyncPagedList<T> : IPagedList<T>
{
    public AsyncPagedList(IAsyncEnumerable<T> source, int pageNumber, int pageSize)
    {
        PageNumber = pageNumber;
        PageSize = pageSize;
        LoadDataAsync(source).GetAwaiter().GetResult();
    }

    public int FirstItemOnPage => (PageNumber - 1) * PageSize + 1;

    public bool HasNextPage => PageNumber < PageCount;

    public bool HasPreviousPage => PageNumber > 1;

    public bool IsFirstPage => PageNumber == 1;

    public bool IsLastPage => PageNumber == PageCount;

    public IList<T> Items { get; private set; } = new List<T>();

    public int LastItemOnPage => FirstItemOnPage + Items.Count - 1;

    public int PageCount { get; private set; }

    public int PageNumber { get; }

    public int PageSize { get; }

    public int TotalCount { get; private set; }

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

    private async Task LoadDataAsync(IAsyncEnumerable<T> source)
    {
        TotalCount = await source.CountAsync();
        PageCount = (int)Math.Ceiling(TotalCount / (double)PageSize);
        Items = await source.SkipAsync((PageNumber - 1) * PageSize).TakeAsync(PageSize).ToListAsync();
    }
}