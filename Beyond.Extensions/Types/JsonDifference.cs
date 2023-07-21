using Beyond.Extensions.Enums;

namespace Beyond.Extensions.Types;

public class JsonDifference
{
    public JsonChangeType ChangeType { get; set; }
    public string? NewValue { get; set; }
    public string? OldValue { get; set; }
    public string Path { get; set; } = null!;
}