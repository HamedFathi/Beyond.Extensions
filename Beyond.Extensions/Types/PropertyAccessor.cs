namespace Beyond.Extensions.Types;

public class PropertyAccessor<T>
{
    private readonly Func<T, object> _getter;
    private readonly Action<T, object> _setter;

    public PropertyAccessor(string propertyName)
    {
        var parameterExp = Expression.Parameter(typeof(T), "type");
        var propertyExp = Expression.Property(parameterExp, propertyName);

        var valueExp = Expression.Parameter(typeof(object), "value");
        var convertExp = Expression.Convert(valueExp, propertyExp.Type);

        _getter = Expression.Lambda<Func<T, object>>(Expression.Convert(propertyExp, typeof(object)), parameterExp).Compile();
        _setter = Expression.Lambda<Action<T, object>>(Expression.Assign(propertyExp, convertExp), parameterExp, valueExp).Compile();
    }

    public object Get(T target)
    {
        return _getter(target);
    }

    public void Set(T target, object value)
    {
        _setter(target, value);
    }
}