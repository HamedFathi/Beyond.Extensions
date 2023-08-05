using Beyond.Extensions.Enums;

namespace Beyond.Extensions.Types;

public class DiffChange
{
    // Constructor to create a new Change instance with the provided values.
    public DiffChange(DiffChangeType type, string text, int lineNumber, DiffChangeOrigin origin)
    {
        Type = type;
        Text = text;
        LineNumber = lineNumber;
        Origin = origin;
    }

    public int LineNumber { get; set; }

    public DiffChangeOrigin Origin { get; set; }

    public string Text { get; set; }

    // Properties to store the type, text, line number, and origin of the change.
    public DiffChangeType Type { get; set; }
}