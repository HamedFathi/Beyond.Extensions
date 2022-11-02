// ReSharper disable CheckNamespace
// ReSharper disable UnusedMember.Global

namespace Beyond.Extensions.TextReaderExtended;

public static class TextReaderExtensions
{
    public static IEnumerable<string> ReadLines(this TextReader reader)
    {
        while (reader.ReadLine() is { } line)
            yield return line;
    }

    public static void ReadLines(this TextReader reader, Action<string> action)
    {
        while (reader.ReadLine() is { } line)
            action(line);
    }
}