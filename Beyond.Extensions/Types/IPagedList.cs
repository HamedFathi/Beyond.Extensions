using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Beyond.Extensions.Types;

public interface IPagedList<T>
{
    int FirstItemOnPage { get; }
    bool HasNextPage { get; }
    bool HasPreviousPage { get; }
    bool IsFirstPage { get; }
    bool IsLastPage { get; }
    int LastItemOnPage { get; }
    int PageCount { get; }
    int PageNumber { get; }
    int PageSize { get; }
    int TotalCount { get; }
    T this[int index] { get; }
    IList<T> Items { get; }
}
