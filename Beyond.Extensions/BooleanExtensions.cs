// ReSharper disable CheckNamespace
// ReSharper disable IdentifierTypo
// ReSharper disable UnusedMember.Global

namespace Beyond.Extensions.BooleanExtended;

public static class BooleanExtensions
{
    public static bool And(this bool x, bool y)
    {
        return x && y;
    }

    public static void IfFalse(this bool @this, Action action)
    {
        if (action == null) throw new ArgumentNullException(nameof(action));
        if (!@this) action();
    }

    public static TResult? IfFalse<TResult>(this bool value, Func<TResult> expression)
    {
        if (expression == null) throw new ArgumentNullException(nameof(expression));

        return !value ? expression() : default;
    }

    public static TResult? IfFalse<TResult>(this bool value, TResult? content)
    {
        return !value ? content : default;
    }

    public static void IfTrue(this bool @this, Action action)
    {
        if (action == null) throw new ArgumentNullException(nameof(action));
        if (@this) action();
    }

    public static TResult? IfTrue<TResult>(this bool value, Func<TResult> expression)
    {
        if (expression == null) throw new ArgumentNullException(nameof(expression));

        return value ? expression() : default;
    }

    public static TResult? IfTrue<TResult>(this bool value, TResult? content)
    {
        return value ? content : default;
    }

    public static bool Nand(this bool x, bool y)
    {
        return !And(x, y);
    }

    public static bool Nor(this bool x, bool y)
    {
        return !Or(x, y);
    }

    public static bool Not(this bool x)
    {
        return !x;
    }

    public static bool Or(this bool x, bool y)
    {
        return x || y;
    }

    public static byte ToBinary(this bool @this)
    {
        return Convert.ToByte(@this);
    }

    public static char ToSpecificChar(this bool @this, char trueValue, char falseValue)
    {
        return @this ? trueValue : falseValue;
    }

    public static string ToSpecificString(this bool @this, string trueValue, string falseValue)
    {
        return @this ? trueValue : falseValue;
    }

    public static bool Xnor(this bool x, bool y)
    {
        return !Xor(x, y);
    }

    public static bool Xor(this bool x, bool y)
    {
        return x ^ y;
    }
}