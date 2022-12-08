// ReSharper disable CheckNamespace
// ReSharper disable UnusedMember.Global
namespace Beyond.Extensions.PropertyInfoExtended;

public static class PropertyInfoExtensions
{
    public static Func<TTarget, TProperty> GetProperty<TTarget, TProperty>(this PropertyInfo property)
    {
        var target = Expression.Parameter(property.DeclaringType!, "target");
        var method = property.GetGetMethod()!;
        var callGetMethod = Expression.Call(target, method);
        var lambda = method.ReturnType == typeof(TProperty)
            ? Expression.Lambda<Func<TTarget, TProperty>>(callGetMethod, target)
            : Expression.Lambda<Func<TTarget, TProperty>>(Expression.Convert(callGetMethod, typeof(TProperty)),
                target);
        return lambda.Compile();
    }

    public static Action<TTarget, TProperty>? SetProperty<TTarget, TProperty>(this PropertyInfo property)
    {
        var target = Expression.Parameter(property.DeclaringType!, "target");
        var value = Expression.Parameter(property.PropertyType, "value");
        var method = property.SetMethod;
        if (method == null) return null;
        var callSetMethod = Expression.Call(target, method, value);
        var lambda = Expression.Lambda<Action<TTarget, TProperty>>(callSetMethod, target, value);
        return lambda.Compile();
    }
}