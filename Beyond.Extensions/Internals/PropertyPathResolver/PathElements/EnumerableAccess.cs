namespace Beyond.Extensions.Internals.PropertyPathResolver.PathElements;

internal class EnumerableAccess : PathElementBase
{
    private readonly int _index;

    public EnumerableAccess(int index)
    {
        _index = index;
    }

    public override object? Apply(object? target)
    {
        //index lower than 0 doesn't have to be checked, because the IsApplicable check doesn't apply to negative values

        if (target is IEnumerable enumerable)
        {
            var i = 0;
            foreach (var value in enumerable)
            {
                if (i == _index)
                    return value;
                i++;
            }
            //if no value is returned by now, it means that the index is too high
            throw new IndexOutOfRangeException($"The index {_index} is too high. Maximum index is {i - 1}.");
        }

        //if the object is no enumerable, it may have an indexer
        var indexProperties = target?.GetType().GetRuntimeProperties().Where(p => p.GetIndexParameters().Length > 0);
        if (indexProperties != null)
        {
            var appropriateIndexProperty = indexProperties.FirstOrDefault(p => p.GetIndexParameters().Length == 1 && p.GetIndexParameters()[0].ParameterType == typeof(int));
            if (appropriateIndexProperty == null) throw new ArgumentException("The target does not have an indexer that takes exactly 1 int parameter");
            return appropriateIndexProperty.GetValue(target, new object[] { _index });
        }
        return null;
    }
}