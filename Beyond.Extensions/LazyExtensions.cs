// ReSharper disable CheckNamespace
// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedType.Global

namespace Beyond.Extensions.LazyExtended;

public static class LazyExtensions
{
    public static void DisposeIfValueCreated<T>(this Lazy<T>? lazy) where T : IDisposable
    {
        if (lazy is { IsValueCreated: true })
            lazy.Value.Dispose();
    }
}