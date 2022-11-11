namespace Beyond.Extensions.Internals.PropertyPathResolver.PathElements;

public abstract class PathElementBase : IPathElement
{
    public IEnumerable Apply(Selection target)
    {
        var results = new List<object?>();
        foreach (var entry in target.Entries)
        {
            results.Add(Apply(entry));
        }
        return new Selection(results);
    }

    public abstract object? Apply(object? target);
}