// ReSharper disable CheckNamespace
// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedType.Global
namespace Beyond.Extensions.FuncExtended;

public static class FuncExtensions
{
    public static Expression<Func<T, TU>> ToExpression<T, TU>(this Func<T, TU> func)
    {
        return x => func(x);
    }

    public static Expression<Func<T>> ToExpression<T>(this Func<T> func)
    {
        return () => func();
    }
}