// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Beyond.Extensions.Types;

public class Position
{
    private readonly string _value;

    public Position(string value, int position, int line, int column)
    {
        Line = line;
        Column = column;
        _value = value;
        End = position;
    }

    public int Column { get; }
    public int End { get; }
    public bool IsMultiLine => _value.Contains('\n');
    public int Length => _value.Length;
    public int Line { get; }
    public int Start => End - Length;
}