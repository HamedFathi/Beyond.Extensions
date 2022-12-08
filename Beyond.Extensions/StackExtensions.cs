// ReSharper disable CheckNamespace
// ReSharper disable UnusedType.Global
// ReSharper disable UnusedMember.Global

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
}