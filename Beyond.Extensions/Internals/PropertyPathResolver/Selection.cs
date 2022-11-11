﻿namespace Beyond.Extensions.Internals.PropertyPathResolver;

public class Selection : IEnumerable<object>
{
    public IReadOnlyCollection<object?> Entries { get; }

    public Selection(IEnumerable entries)
    {
        var list = new List<object?>();
        foreach (var entry in entries)
            list.Add(entry);
        Entries = list;
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return new SelectionEnumerator(this);
    }

    IEnumerator<object> IEnumerable<object>.GetEnumerator()
    {
        return new SelectionEnumerator(this);
    }
}