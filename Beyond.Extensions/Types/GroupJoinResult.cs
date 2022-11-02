namespace Beyond.Extensions.Types;

public class GroupJoinResult<TKey, TValue>
{
    public TKey? Key { get; set; }
    public IEnumerable<TValue>? Values { get; set; }
}