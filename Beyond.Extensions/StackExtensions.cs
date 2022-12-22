// ReSharper disable CheckNamespace
// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedType.Global
// ReSharper disable CommentTypo

namespace Beyond.Extensions.StackExtended;

public static class StackExtensions
{
    public static IEnumerable<T> PopAll<T>(this Stack<T> stack)
    {
        var list = new List<T>();
        while (stack.Count > 0)
        {
            list.Add(stack.Pop());
        }

        return list;
    }

    public static IEnumerable<T> PopIf<T>(this Stack<T> stack, Func<T, bool> predicate)
    {
        var list = new List<T>();
        while (stack.Count > 0)
        {
            if (predicate(stack.Peek()))
                list.Add(stack.Pop());
        }

        return list;
    }

    public static bool PushUnique<T>(this Stack<T> stack, T? value, bool checkAllItems = false)
        where T : IComparable
    {
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
}