// ReSharper disable CheckNamespace
// ReSharper disable UnusedMember.Global
namespace Beyond.Extensions.FieldInfoExtended;

public static class FieldInfoExtensions
{
    public static Func<TTarget, TField> GetField<TTarget, TField>(this FieldInfo field)
    {
        var target = Expression.Parameter(typeof(TTarget), "target");
        var fieldAccess = Expression.Field(target, field);
        var lambda = field.FieldType == typeof(TField)
            ? Expression.Lambda<Func<TTarget, TField>>(fieldAccess, target)
            : Expression.Lambda<Func<TTarget, TField>>(Expression.Convert(fieldAccess, typeof(TField)), target);
        return lambda.Compile();
    }
    public static Action<TTarget, TField> SetField<TTarget, TField>(this FieldInfo field)
    {
        var target = Expression.Parameter(typeof(TTarget), "target");
        var value = Expression.Parameter(typeof(TField), "value");
        var fieldAccess = Expression.Field(target, field);
        var fieldSetter = Expression.Assign(fieldAccess, value);
        var lambda = Expression.Lambda<Action<TTarget, TField>>(fieldSetter, target, value);
        return lambda.Compile();
    }
}