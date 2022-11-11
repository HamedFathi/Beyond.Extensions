namespace Beyond.Extensions.Internals.PropertyPathResolver.PathElements;

public interface IPathElement
{
    object? Apply(object? target);
    IEnumerable? Apply(Selection target);
}