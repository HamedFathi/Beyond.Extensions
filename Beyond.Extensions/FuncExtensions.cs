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

    public static Func<T2, T1, TR> Swap<T1, T2, TR>(this Func<T1, T2, TR> f)
    {
        return (t2, t1) => f(t1, t2);
    }
}