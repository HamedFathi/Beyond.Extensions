namespace Beyond.Extensions.Internals.PropertyPathResolver.PathElements;

internal class DictionaryAccess : PathElementBase
{
    private readonly string _key;

    public DictionaryAccess(string key)
    {
        _key = key;
    }

    public override object? Apply(object? target)
    {
        if (target is IDictionary dictionary)
        {
            foreach (DictionaryEntry de in dictionary)
            {
                if (de.Key.ToString() == _key)
                    return de.Value;
            }
            //if no value is returned by now, it means that the index is too high
            throw new ArgumentException($"The key {_key} does not exist.");
        }

        //if the object is no dictionary, it may have an indexer
        var indexProperties = target?.GetType().GetRuntimeProperties().Where(p => p.GetIndexParameters().Length > 0);
        if (indexProperties != null)
        {
            var appropriateIndexProperty = indexProperties.FirstOrDefault(p => p.GetIndexParameters().Length == 1 && p.GetIndexParameters()[0].ParameterType == typeof(string));
            if (appropriateIndexProperty == null) throw new ArgumentException("The target does not have an indexer that takes exactly 1 string parameter");
            return appropriateIndexProperty.GetValue(target, new object[] { _key });
        }
        return null;
    }
}