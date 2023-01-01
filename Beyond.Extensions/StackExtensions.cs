// ReSharper disable CheckNamespace
// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedType.Global
// ReSharper disable CommentTypo

namespace Beyond.Extensions.StackExtended;

public static class StackExtensions
{
    public static void ForEach<T>(this Stack<T> stack, Action<T> action)
    {
        if (stack == null) throw new ArgumentNullException(nameof(stack));
        if (action == null) throw new ArgumentNullException(nameof(action));
        var list = stack.ToList();
        foreach (var l in list)
        {
            action(l);
        }
    }

    public static IEnumerable<T> PopAll<T>(this Stack<T> stack)
    {
        if (stack == null) throw new ArgumentNullException(nameof(stack));
        var list = new List<T>();
        while (stack.Count > 0)
        {
            list.Add(stack.Pop());
        }

        return list;
    }

    public static T? PopIf<T>(this Stack<T> stack, Func<T, bool> predicate)
    {
        if (stack == null) throw new ArgumentNullException(nameof(stack));
        if (predicate(stack.Peek()))
            stack.Pop();

        return default;
    }

    public static IEnumerable<T> PopIfAny<T>(this Stack<T> stack, Func<T, bool> predicate)
    {
        if (stack == null) throw new ArgumentNullException(nameof(stack));
        var list = new List<T>();
        while (stack.Count > 0)
        {
            if (predicate(stack.Peek()))
                list.Add(stack.Pop());
        }

        return list;
    }

    public static T? PopSafe<T>(this Stack<T> stack)
    {
        return stack is { Count: > 0 } ? stack.Pop() : default;
    }

    public static void PushIf<T>(this Stack<T> stack, T value, Func<bool> predicate)
    {
        if (stack == null) throw new ArgumentNullException(nameof(stack));
        if (predicate())
            stack.Push(value);
    }

    public static bool PushUnique<T>(this Stack<T> stack, T? value, bool checkAllItems = false)
        where T : IComparable
    {
        if (stack == null) throw new ArgumentNullException(nameof(stack));
        if (value == null)
        {
            return false;
        }

        if (checkAllItems)
        {
            var statusAny = stack.Any(x => Comparer<T>.Default.Compare(value, x) == 0);
            if (!statusAny)
            {
                stack.Push(value);
            }

            return !statusAny;
        }

        var item = stack.Peek();
        var status = Comparer<T>.Default.Compare(value, item) == 0;
        if (!status)
        {
            stack.Push(value);
        }

        return !status;
    }

    public static void Replace<T>(this Stack<T?> stack, T? value, T? replacement, IEqualityComparer<T>? comparer = null)
    {
        if (stack == null) throw new ArgumentNullException(nameof(stack));

        comparer ??= EqualityComparer<T>.Default;

        var temp = new Stack<T?>();
        while (stack.Count > 0)
        {
            var val = stack.Pop();
            if (comparer.Equals(val, value))
            {
                stack.Push(replacement);
                break;
            }
            temp.Push(val);
        }

        while (temp.Count > 0)
            stack.Push(temp.Pop());
    }

    public static void Update<T>(this Stack<T> stack, T value, out T oldValue)
    {
        if (stack == null) throw new ArgumentNullException(nameof(stack));
        oldValue = stack.Pop();
        stack.Push(value);
    }

    public static void Update<T>(this Stack<T> stack, T value)
    {
        if (stack == null) throw new ArgumentNullException(nameof(stack));
        stack.Update(value, out _);
    }
}