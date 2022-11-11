namespace Beyond.Extensions.Internals.PropertyPathResolver.PathElements;

internal class Property : PathElementBase
{
    private readonly string _property;

    public Property(string property)
    {
        _property = property;
    }

    public override object? Apply(object? target)
    {
        var p = target?.GetType().GetRuntimeProperty(_property);
        if (p == null)
            throw new ArgumentException($"The property {_property} could not be found.");

        return p.GetValue(target);
    }
}