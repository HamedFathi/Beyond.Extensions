namespace Beyond.Extensions.Internals.PropertyPathResolver.PathElements;

internal class SelectionAccess : IPathElement
{
    public object? Apply(object? target)
    {
        if (target is IEnumerable enumerable)
        {
            return new Selection(enumerable);
        }
        return null;
    }

    public IEnumerable Apply(Selection target)
    {
        var results = new List<object?>();
        foreach (var entry in target.Entries)
        {
            if (entry is not IEnumerable enumerable)
                results.Add(entry);
            else
            {
                foreach (var element in enumerable)
                    results.Add(element);
            }
        }
        return new Selection(results);
    }
}