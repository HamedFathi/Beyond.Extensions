// ReSharper disable CheckNamespace
// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedType.Global

namespace Beyond.Extensions.ActionExtended;

public static class ActionExtensions
{
    public static TimeSpan GetExecutionTime(this Action action)
    {
        if (action == null) throw new ArgumentNullException(nameof(action));

        var start = new Stopwatch();
        start.Start();
        action();
        start.Stop();
        return start.Elapsed;
    }

    public static Action NeutralizeException(this Action action, Action? @finally = null)
    {
        if (action == null) throw new ArgumentNullException(nameof(action));

        return () =>
        {
            try
            {
                action();
            }
            catch
            {
                // ignored
            }
            finally
            {
                @finally?.Invoke();
            }
        };
    }

    public static Action<object>? ToActionObject<T>(this Action<T>? actionT)
    {
        return actionT == null ? null : new Action<object>(o => actionT((T)o));
    }
}