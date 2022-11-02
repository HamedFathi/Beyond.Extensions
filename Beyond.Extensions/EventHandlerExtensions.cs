// ReSharper disable CheckNamespace
// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedType.Global

namespace Beyond.Extensions.EventHandlerExtended;

public static class EventHandlerExtensions
{
    public static void HandleEvent<T>(this EventHandler<T> eventHandler, object sender, T e) where T : EventArgs
    {
        eventHandler(sender, e);
    }

    public static EventHandler HandleOnce(this EventHandler handler, Action<EventHandler> remover)
    {
        // ReSharper disable once ConvertToLocalFunction
        EventHandler? wrapper = null;
        wrapper = (sender, e) =>
        {
            if (wrapper != null) remover(wrapper);
            handler(sender, e);
        };
        return wrapper;
    }

    public static void Raise(this EventHandler? handler, object sender, EventArgs e)
    {
        handler?.Invoke(sender, e);
    }

    public static void Raise(this EventHandler? handler, object sender)
    {
        handler.Raise(sender, EventArgs.Empty);
    }

    public static void Raise<TEventArgs>(this EventHandler<TEventArgs>? handler, object sender)
        where TEventArgs : EventArgs
    {
        if (handler != null) handler.Raise(sender, Activator.CreateInstance<TEventArgs>());
    }

    public static void Raise<TEventArgs>(this EventHandler<TEventArgs>? handler, object sender, TEventArgs e)
        where TEventArgs : EventArgs
    {
        handler?.Invoke(sender, e);
    }

    public static void RaiseEvent(this EventHandler? @this, object sender)
    {
        @this?.Invoke(sender, EventArgs.Empty);
    }

    public static void RaiseEvent<TEventArgs>(this EventHandler<TEventArgs>? @this, object sender)
        where TEventArgs : EventArgs
    {
        @this?.Invoke(sender, Activator.CreateInstance<TEventArgs>());
    }

    public static void RaiseEvent<TEventArgs>(this EventHandler<TEventArgs>? @this, object sender, TEventArgs e)
        where TEventArgs : EventArgs
    {
        @this?.Invoke(sender, e);
    }

    public static void RaiseEvent(this PropertyChangedEventHandler? @this, object sender, string propertyName)
    {
        @this?.Invoke(sender, new PropertyChangedEventArgs(propertyName));
    }
}