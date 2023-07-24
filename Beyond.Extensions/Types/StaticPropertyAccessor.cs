namespace Beyond.Extensions.Types;

public class StaticPropertyAccessor<T>
{
    private readonly Func<object> _getter;
    private readonly Action<object> _setter;

    public StaticPropertyAccessor(string propertyName)
    {
        var propertyExp = Expression.Property(null, typeof(T), propertyName);

        var valueExp = Expression.Parameter(typeof(object), "value");
        var convertExp = Expression.Convert(valueExp, propertyExp.Type);

        _getter = Expression.Lambda<Func<object>>(Expression.Convert(propertyExp, typeof(object))).Compile();
        _setter = Expression.Lambda<Action<object>>(Expression.Assign(propertyExp, convertExp), valueExp).Compile();
    }

    public object Get()
    {
        return _getter();
    }

    public void Set(object value)
    {
        _setter(value);
    }
}