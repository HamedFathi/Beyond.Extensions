// ReSharper disable CheckNamespace
// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedType.Global

namespace Beyond.Extensions.DelegateExtended;

public static class DelegateExtensions
{
    public static Comparison<T> AsComparison<T>(this Func<T, T, int> function)
    {
        return As<Comparison<T>>(function);
    }

    public static Func<T, bool> AsFunc<T>(this Predicate<T> predicate)
    {
        return As<Func<T, bool>>(predicate);
    }

    public static Func<T, T, int> AsFunc<T>(this Comparison<T> comparison)
    {
        return As<Func<T, T, int>>(comparison);
    }

    public static Predicate<T> AsPredicate<T>(this Func<T, bool> function)
    {
        return As<Predicate<T>>(function);
    }

    public static Delegate Combine(this Delegate a, Delegate b)
    {
        return Delegate.Combine(a, b);
    }

    public static Delegate? Remove(this Delegate source, Delegate value)
    {
        return Delegate.Remove(source, value);
    }

    public static Delegate? RemoveAll(this Delegate source, Delegate value)
    {
        return Delegate.RemoveAll(source, value);
    }

    public static IComparer<T> ToComparer<T>(this Comparison<T> comparison)
    {
        return new DelegateBasedComparer<T>(comparison);
    }

    public static IComparer<T> ToComparer<T>(this Func<T, T, int> function)
    {
        return new DelegateBasedComparer<T>(function.AsComparison());
    }

    private static TDelegate As<TDelegate>(Delegate @delegate)
    {
        var method = @delegate.GetMethodInfo();
        if (method == null)
            throw new ArgumentException("Delegate does not have a method info.", nameof(@delegate));

        return (TDelegate)(object)method.CreateDelegate(typeof(TDelegate), @delegate.Target);
    }

    private class DelegateBasedComparer<T> : IComparer<T>
    {
        private readonly Comparison<T> _comparison;

        public DelegateBasedComparer(Comparison<T> comparison)
        {
            this._comparison = comparison;
        }

        public int Compare(T? x, T? y)
        {
            if (x == null) throw new ArgumentNullException(nameof(x));
            if (y == null) throw new ArgumentNullException(nameof(y));

            return _comparison(x, y);
        }
    }
}