// ReSharper disable CheckNamespace
// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedType.Global

namespace Beyond.Extensions.SocketExtended;

public static class ExtensionsSocket
{
    public static bool IsConnected(this Socket socket)
    {
        var part1 = socket.Poll(1000, SelectMode.SelectRead);
        var part2 = socket.Available == 0;

        return part1 & part2;
    }
}